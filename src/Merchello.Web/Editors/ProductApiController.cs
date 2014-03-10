using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using Umbraco.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.WebApi;
using Umbraco.Web;
using Merchello.Web.Models.ContentEditing;
using Examine;


namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class ProductApiController : MerchelloApiController
    {
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProductApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public ProductApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _productService = MerchelloContext.Services.ProductService;
            _warehouseService = MerchelloContext.Services.WarehouseService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ProductApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _productService = MerchelloContext.Services.ProductService;
            _warehouseService = MerchelloContext.Services.WarehouseService;
        }

        /// <summary>
        /// Returns Product by id (key)
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProduct/{guid}
        /// </summary>
        /// <param name="id"></param>
        public ProductDisplay GetProduct(Guid id)
        {
           
            var product = _productService.GetByKey(id) as Product;
            if (product == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return product.ToProductDisplay();
           
        }

        /// <summary>
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProducts
        /// </summary>
        /// <param name="keys"></param>
        public IEnumerable<ProductDisplay> GetAllProducts()
        {
            MerchelloHelper merchello = new MerchelloHelper();

            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.Field("master", "True");

            return merchello.SearchProducts(criteria);
        }

        /// <summary>
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProducts
        /// </summary>
        /// <param name="keys"></param>
        public IEnumerable<ProductDisplay> GetAllProducts(int page, int perPage)
        {
            MerchelloHelper merchello = new MerchelloHelper();

            var criteria = ExamineManager.Instance.CreateSearchCriteria();
            criteria.Field("master", "True");

            var products = merchello.SearchProducts(criteria);

            return products.Skip((page-1) * perPage).Take(perPage);
        }

        /// <summary>
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetFilteredProducts
        /// </summary>
        /// <param name="term"></param>
        public IEnumerable<ProductDisplay> GetFilteredProducts(string term)
        {
            MerchelloHelper merchello = new MerchelloHelper();

            return merchello.SearchProducts(term);
        }


        /// <summary>
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetFilteredProducts
        /// </summary>
        /// <param name="term"></param>
        public IEnumerable<ProductDisplay> GetFilteredProducts(string term, int page, int perPage)
        {
            MerchelloHelper merchello = new MerchelloHelper();

            return merchello.SearchProducts(term).Skip((page - 1) * perPage).Take(perPage);
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
