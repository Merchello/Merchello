namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
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
        /// The product variant service.
        /// </summary>
        private readonly IProductVariantService _productVariantService;

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
            _productVariantService = MerchelloContext.Services.ProductVariantService;
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
            _productVariantService = MerchelloContext.Services.ProductVariantService;
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
        /// Returns a Product Variant by id (key)
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProductVariant/{guid}
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        public ProductVariantDisplay GetProductVariant(Guid id)
        {
            var variant = _merchello.Query.Product.GetProductVariantByKey(id);
            return variant;
        }

        /// <summary>
        /// Returns a Product by id (key) directly from the service
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProduct/{guid}
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        public ProductDisplay GetProductFromService(Guid id)
        {
            return _productService.GetByKey(id).ToProductDisplay();
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
        /// Creates a new product with variants
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        [HttpPost]
        public ProductDisplay AddProduct(ProductDisplay product)
        {
            var merchProduct = _productService.CreateProduct(product.Name, product.Sku, product.Price);

            merchProduct = product.ToProduct(merchProduct);
            _productService.Save(merchProduct);

            // special case where a catalog was associated before the creation of the product
            if (product.CatalogInventories.Any())
            {
                foreach (var cat in product.CatalogInventories)
                {
                    ((Product)merchProduct).MasterVariant.AddToCatalogInventory(cat.CatalogKey);
                }
            }

            _productService.Save(merchProduct);

            if (!merchProduct.ProductOptions.Any()) return merchProduct.ToProductDisplay();

            var attributeLists = merchProduct.GetPossibleProductAttributeCombinations();

            foreach (var list in attributeLists)
            {
                _productVariantService.CreateProductVariantWithKey(merchProduct, list.ToProductAttributeCollection());
            }

            return merchProduct.ToProductDisplay();
        }

        /// <summary>
        /// Updates an existing product
        /// 
        /// PUT /umbraco/Merchello/ProductApi/PutProduct
        /// </summary>
        /// <param name="product">
        /// ProductDisplay object serialized from WebApi
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        [HttpPost, HttpPut]
        public ProductDisplay PutProduct(ProductDisplay product)
        {            
            var merchProduct = _productService.GetByKey(product.Key);         
            merchProduct = product.ToProduct(merchProduct);
            _productService.Save(merchProduct);

            if (!merchProduct.ProductOptions.Any()) return merchProduct.ToProductDisplay();



            // verify that all attributes have been created
            var attributeLists = merchProduct.GetPossibleProductAttributeCombinations().ToArray();
            foreach (var list in from list in attributeLists let variant = merchProduct.GetProductVariantForPurchase(list) where variant == null select list)
            {
                _productVariantService.CreateProductVariantWithKey(merchProduct, list.ToProductAttributeCollection());
            }

            return merchProduct.ToProductDisplay();
        }

        //[HttpPost]
        //public ProductVariantDisplay AddProductVariant(ProductVariantDisplay productVariant)
        //{

        //}

        /// <summary>
        /// The put product variant.
        /// </summary>
        /// <param name="productVariant">
        /// The product variant.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        [HttpPost, HttpPut]
        public ProductVariantDisplay PutProductVariant(ProductVariantDisplay productVariant)
        {
            var variant = _productVariantService.GetByKey(productVariant.Key);
            variant = productVariant.ToProductVariant(variant);

            _productVariantService.Save(variant);

            return variant.ToProductVariantDisplay();
        }

        /// <summary>
        /// Deletes an existing product
        /// 
        /// DELETE /umbraco/Merchello/ProductApi/{guid}
        /// </summary>
        /// <param name="id">
        /// The key of the product to delete
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost, HttpDelete, HttpGet]
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

        //[HttpPost, HttpDelete]
        //public HttpResponseMessage DeleteProductVariant(Guid id)
        //{

        //}
    }
}
