using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.WebApi;
using Umbraco.Web;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class ProductApiController : MerchelloApiController
    {
        private readonly IProductService _productService;

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

            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ProductApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _productService = MerchelloContext.Services.ProductService;

            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();
        }

        /// <summary>
        /// Returns Product by id (key)
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProduct?key={guid}
        /// </summary>
        /// <param name="key"></param>
        public ProductDisplay GetProduct(Guid key)
        {
            if (key != null)
            {
                var product = _productService.GetByKey(key) as Product;
                if (product == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                return AutoMapper.Mapper.Map<ProductDisplay>(product);
            }
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Parameter key is null")),
                    ReasonPhrase = "Invalid Parameter"
                };
                throw new HttpResponseException(resp);
            }
        }

        /// <summary>
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProducts
        /// </summary>
        /// <param name="keys"></param>
        public IEnumerable<ProductDisplay> GetAllProducts()
        {
            ProductService tempProductService = _productService as ProductService;
            var products = tempProductService.GetAll();
            if (products == null)
            {
                // ?
            }

            foreach (IProduct product in products)
            {
                yield return AutoMapper.Mapper.Map<ProductDisplay>(product);
            }
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
                    yield return AutoMapper.Mapper.Map<ProductDisplay>(product);
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
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return AutoMapper.Mapper.Map<ProductDisplay>(newProduct);
        }

        /// <summary>
        /// Updates an existing product
        ///
        /// PUT /umbraco/Merchello/ProductApi/PutProduct
        /// </summary>
        /// <param name="product">ProductDisplay object serialized from WebApi</param>
        [AcceptVerbs("PUT")]
        public HttpResponseMessage PutProduct(ProductDisplay product)
        {
            // Using AutoMapper (http://automapper.org/)

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
        /// <param name="key"></param>
        public HttpResponseMessage Delete(Guid key)
        {
            var productToDelete = _productService.GetByKey(key);
            if (productToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _productService.Delete(productToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates a product variant from Sku, Name, Price
        ///
        /// GET /umbraco/Merchello/ProductApi/NewProductVariant?key={guid}&attributes=[]
        /// </summary>
        /// <param name="item"></param>
        //[AcceptVerbs("GET", "POST")]
        //public IProductVariant NewProductVariant(IProductVariant productVariant)
        //{
        //    IProductVariant newProductVariant = null;

        //    try
        //    {
        //        var product = _productService.GetByKey(productVariant.ProductKey);
        //        var productAttributes = new ProductAttributeCollection();
        //        foreach (var attribute in productVariant.Attributes)
        //        {
        //            productAttributes.Add(attribute);
        //        }
        //        newProductVariant = _productVariantService.CreateProductVariantWithId(product, productAttributes, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpResponseException(HttpStatusCode.InternalServerError);
        //    }

        //    return newProductVariant;
        //}
    }
}
