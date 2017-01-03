namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.ModelBinding;

    using Merchello.Core;
    using Merchello.Core.Chains.CopyEntity.Product;
    using Merchello.Core.Services;
    using Merchello.Core.ValueConverters;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.WebApi;
    using Merchello.Web.WebApi.Binders;
    using Merchello.Web.WebApi.Filters;

    using Umbraco.Core;
    using Umbraco.Core.Models;
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
        /// The collection of all languages.
        /// </summary>
        private Lazy<ILanguage[]> _allLanguages;

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

            _merchello = new MerchelloHelper(MerchelloContext, false, DetachedValuesConversionType.Editor);

            Initialize();
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
            _merchello = new MerchelloHelper(MerchelloContext, false, DetachedValuesConversionType.Editor);
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
        [HttpGet]
        public ProductDisplay GetProduct(Guid id)
        {            
            var product = _merchello.Query.Product.GetByKey(id);
            return product;
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
        [HttpGet]
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
        [HttpGet]
        public ProductDisplay GetProductFromService(Guid id)
        {
            return _productService.GetByKey(id).ToProductDisplay(DetachedValuesConversionType.Editor);
        }

        /// <summary>
        /// Gets a value indicating whether or not a SKU exists.
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The value indicating whether the SKU exists.
        /// </returns>
        [HttpGet]
        public bool GetSkuExists(string sku)
        {
            return _productService.SkuExists(sku);
        }

        /// <summary>
        /// The get by ids.
        /// </summary>
        /// <param name="keys">
        /// The product keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductDisplay}"/>.
        /// </returns>
        [HttpPost]
        public IEnumerable<ProductDisplay> GetByKeys(IEnumerable<Guid> keys)
        {
            return _productService.GetByKeys(keys).Select(x => x.ToProductDisplay(DetachedValuesConversionType.Editor));
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
        /// Valid sortBy parameters  "SKU", "name", "price" 
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
        [Obsolete("AddProduct is being superceded by CreateProduct so we can attach content at time of creation")]
        public ProductDisplay AddProduct(ProductDisplay product)
        {
            var merchProduct = _productService.CreateProduct(product.Name, product.Sku, product.Price);

            merchProduct = product.ToProduct(merchProduct);
            _productService.Save(merchProduct);
            return merchProduct.ToProductDisplay(DetachedValuesConversionType.Editor);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        [HttpPost]
        public ProductDisplay CreateProduct(ProductDisplay product)
        {
            // we need to remove the detached content to generate the product to begin with due to db foreign keys
            var detachedContents = product.DetachedContents.ToArray();
            product.DetachedContents = Enumerable.Empty<ProductVariantDetachedContentDisplay>();

            // First create the product record and save it
            var merchProduct = _productService.CreateProduct(product.Name, product.Sku, product.Price);
            merchProduct = product.ToProduct(merchProduct);

            // we don't want to raise events here since we will be saving again and there is no sense
            // in having examine index it twice. Use the detached contents to determine whether we need to fire event
            _productService.Save(merchProduct, !detachedContents.Any());

            if (!detachedContents.Any()) return merchProduct.ToProductDisplay(DetachedValuesConversionType.Editor);

            // convert the product back so we can reassociate the detached content.
            product = merchProduct.ToProductDisplay();

            // asscociate the product variant key (master variant) with the detached content
            foreach (var pvdc in detachedContents)
            {
                pvdc.ProductVariantKey = merchProduct.ProductVariantKey;
            }

            // add the detached contents back
            product.DetachedContents = detachedContents;

            // this adds the detached content to the product
            merchProduct = product.ToProduct(merchProduct);
            _productService.Save(merchProduct);

            return merchProduct.ToProductDisplay(DetachedValuesConversionType.Editor);
        }

        /// <summary>
        /// The post copy product.
        /// </summary>
        /// <param name="productCopySave">
        /// The product copy save.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the original product is not found
        /// </exception>
        /// <exception cref="Exception">
        /// Throws an exception if the copy attempt failed.
        /// </exception>
        [HttpPost]
        public ProductDisplay PostCopyProduct(ProductCopySave productCopySave)
        {
            var original = _productService.GetByKey(productCopySave.Product.Key);
            
            if (original == null) throw new NullReferenceException("Product was not found");

            var taskChain = new CopyProductTaskChain(original, productCopySave.Name, productCopySave.Sku);

            var attempt = taskChain.Copy();

            if (!attempt.Success) throw attempt.Exception;

            return attempt.Result.ToProductDisplay(DetachedValuesConversionType.Editor);
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

            if (product.DetachedContents.Any())
            {
                foreach (var c in product.DetachedContents.Select(x => x.CultureName))
                {
                    var pcs = new ProductContentSave { CultureName = c, Display = product };
                    ProductVariantDetachedContentHelper<ProductContentSave, ProductDisplay>.MapDetachedProperties(pcs);
                }
            }

            merchProduct = product.ToProduct(merchProduct);

            _productService.Save(merchProduct);

            return merchProduct.ToProductDisplay(DetachedValuesConversionType.Editor);
        }

        /// <summary>
        /// The put product with detached content.
        /// </summary>
        /// <param name="detachedContentItem">
        /// The product save.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        [FileUploadCleanupFilter]
        [HttpPost, HttpPut]
        public ProductDisplay PutProductWithDetachedContent(
            [ModelBinder(typeof(ProductContentSaveBinder))]
            ProductContentSave detachedContentItem)
        {
            ProductVariantDetachedContentHelper<ProductContentSave, ProductDisplay>.MapDetachedProperties(detachedContentItem);

            var merchProduct = _productService.GetByKey(detachedContentItem.Display.Key);

            merchProduct = detachedContentItem.Display.ToProduct(merchProduct);

            _productService.Save(merchProduct);

            return merchProduct.ToProductDisplay(DetachedValuesConversionType.Editor);
        }

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

            if (productVariant.DetachedContents.Any())
            {
                foreach (var c in productVariant.DetachedContents.Select(x => x.CultureName))
                {
                    var pcs = new ProductVariantContentSave { CultureName = c, Display = productVariant };
                    ProductVariantDetachedContentHelper<ProductVariantContentSave, ProductVariantDisplay>.MapDetachedProperties(pcs);
                }
            }

            variant = productVariant.ToProductVariant(variant);

            _productVariantService.Save(variant);

            return variant.ToProductVariantDisplay(DetachedValuesConversionType.Editor);
        }

        /// <summary>
        /// The put product variant content.
        /// </summary>
        /// <param name="detachedContentItem">
        /// The product variant save.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        [FileUploadCleanupFilter]
        [HttpPost, HttpPut]
        public ProductVariantDisplay PutProductVariantWithDetachedContent(
            [ModelBinder(typeof(ProductVariantContentSaveBinder))]
            ProductVariantContentSave detachedContentItem)
        {
            ProductVariantDetachedContentHelper<ProductVariantContentSave, ProductVariantDisplay>.MapDetachedProperties(detachedContentItem);

            var variant = _productVariantService.GetByKey(detachedContentItem.Display.Key);
            variant = detachedContentItem.Display.ToProductVariant(variant);

            _productVariantService.Save(variant);

            return variant.ToProductVariantDisplay(DetachedValuesConversionType.Editor);
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

        /// <summary>
        /// Removes detached content from a product variant
        /// </summary>
        /// <param name="productVariant">
        /// The product variant.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost, HttpDelete]
        public HttpResponseMessage DeleteDetachedContent(ProductVariantDisplay productVariant)
        {
            var product = _productService.GetByKey(productVariant.ProductKey);
            if (product == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            if (product.ProductVariants.Any() && product.ProductVariants.FirstOrDefault(x => x.Key == productVariant.Key) != null)
            {
                var variant = product.ProductVariants.FirstOrDefault(x => x.Key == productVariant.Key);
                if (variant != null) variant.DetachedContents.Clear();
                //// TODO need to walk this through better, we should not need to save the variant and then the product  
                //// as the product save should take care of it, but somewhere in the service the runtime cache is resetting
                //// the variant's detached content in the productvariant collection.  Probably just need to rearrange some of the
                //// calls in the service - suspect EnsureProductVariants.
                _productVariantService.Save(variant);
            }
            else
            {
                product.DetachedContents.Clear();
            }

            _productService.Save(product);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Gets a list of all current manufacturers.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> (Manufacturer names).
        /// </returns>
        [HttpGet]
        public IEnumerable<string> GetAllManufacturers()
        {
            return _productService.GetAllManufacturers();
        }

        /// <summary>
        /// Initializes the controller
        /// </summary>
        private void Initialize()
        { 
            _allLanguages = new Lazy<ILanguage[]>(() => ApplicationContext.Current.Services.LocalizationService.GetAllLanguages().ToArray());
        }
    }
}
