namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using global::Examine;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.WebApi;

    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The product api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class ProductApiController : MerchelloApiController
    {
        /// <summary>
        /// The product service.
        /// </summary>
        private readonly IProductService _productService;

        /// <summary>
        /// The warehouse service.
        /// </summary>
        private readonly IWarehouseService _warehouseService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductApiController"/> class. 
        /// Constructor
        /// </summary>
        public ProductApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductApiController"/> class. 
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        public ProductApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _productService = MerchelloContext.Services.ProductService;
            _warehouseService = MerchelloContext.Services.WarehouseService;
            _merchello = new MerchelloHelper(MerchelloContext.Services);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductApiController"/> class. 
        /// This is a helper constructor for unit testing
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
        internal ProductApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _productService = MerchelloContext.Services.ProductService;
            _warehouseService = MerchelloContext.Services.WarehouseService;
            _merchello = new MerchelloHelper(MerchelloContext.Services);
        }

        /// <summary>
        /// Returns Product by id (key)
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProduct/{guid}
        /// </summary>
        /// <param name="id">
        /// The product key
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        public ProductDisplay GetProduct(Guid id)
        {
            return _merchello.Query.Product.GetByKey(id);
        }

        /// <summary>
        /// Searches all products with an optional term.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        /// <remarks>
        /// Valid sortBy parameters  "sku", "name", "price" 
        /// </remarks>
        [HttpPost]
        public QueryResultDisplay SearchProducts(QueryDisplay query)
        {
            var term = query.Parameters.FirstOrDefault(x => x.FieldName == "term");

            return term != null && !string.IsNullOrEmpty(term.Value)
              ?
               _merchello.Query.Product.Search(
                  term.Value,
                  query.CurrentPage + 1,
                  query.ItemsPerPage,
                  query.SortBy,
                  query.SortDirection)
              :
              _merchello.Query.Product.Search(
                  query.CurrentPage + 1,
                  query.ItemsPerPage,
                  query.SortBy,
                  query.SortDirection);
        }

        /// <summary>
        /// Returns Product by keys separated by a comma
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProducts?keys={guid}&keys={guid}
        /// </summary>
        /// <param name="keys"></param>
        public IEnumerable<ProductDisplay> GetProducts([FromUri]IEnumerable<Guid> keys)
        {
            if (keys != null)
            {
                var products = _productService.GetByKeys(keys);
                if (products == null)
                {
                    //throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                foreach(IProduct product in products)
                {
                    yield return product.ToProductDisplay();
                }
            }
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Parameter keys is null")),
                    ReasonPhrase = "Invalid Parameter"
                };
                throw new HttpResponseException(resp);
            }
        }

        /// <summary>
        /// Creates a product from Sku, Name, Price
        ///
        /// GET /umbraco/Merchello/ProductApi/NewProduct?sku=SKU&name=NAME&price=PRICE
        /// </summary>
        /// <param name="item"></param>
        [AcceptVerbs("GET","POST")]
        public ProductDisplay NewProduct(string name, string sku, decimal price)
        {
            Product newProduct = null;

            try
            {
                newProduct = _productService.CreateProductWithKey(name, sku, price) as Product;
                _productService.Save(newProduct);

                newProduct.AddToCatalogInventory(_warehouseService.GetDefaultWarehouse().DefaultCatalog());
                _productService.Save(newProduct);
            }
            catch (Exception ex)
            {
                 throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return newProduct.ToProductDisplay();
        }

        /// <summary>
        /// Creates a product from ProductDisplay
        ///
        /// POST /umbraco/Merchello/ProductApi/NewProduct
        /// </summary>
        /// <param name="product">POSTed JSON product model</param>
        [AcceptVerbs("GET", "POST")]
        public ProductDisplay NewProductFromProduct(ProductDisplay product)
        {
            IProduct newProduct = null;

            try
            {
                newProduct = _productService.CreateProduct(product.Name, product.Sku, product.Price);
                _productService.Save(newProduct);

                newProduct.AddToCatalogInventory(_warehouseService.GetDefaultWarehouse().DefaultCatalog());

                newProduct = product.ToProduct(newProduct);
                _productService.Save(newProduct);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }

            return newProduct.ToProductDisplay();
        }

        /// <summary>
        /// Updates an existing product
        ///
        /// PUT /umbraco/Merchello/ProductApi/PutProduct
        /// </summary>
        /// <param name="product">ProductDisplay object serialized from WebApi</param>
        [AcceptVerbs("POST", "PUT")]
        public HttpResponseMessage PutProduct(ProductDisplay product)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
                        
            try
            {
                IProduct merchProduct = _productService.GetByKey(product.Key);
                merchProduct = product.ToProduct(merchProduct);

                _productService.Save(merchProduct);
            }
            catch (Exception ex) 
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Deletes an existing product
        ///
        /// DELETE /umbraco/Merchello/ProductApi/{guid}
        /// </summary>
        /// <param name="id"></param>
        [AcceptVerbs("POST", "DELETE")]
        public HttpResponseMessage DeleteProduct(Guid id)
        {
            var productToDelete = _productService.GetByKey(id);
            if (productToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _productService.Delete(productToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
