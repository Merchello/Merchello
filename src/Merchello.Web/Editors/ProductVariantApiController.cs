using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.WebApi;
using Merchello.Web.Models.ContentEditing;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class ProductVariantApiController : MerchelloApiController
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IProductService _productService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProductVariantApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public ProductVariantApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _productService = MerchelloContext.Services.ProductService;
            _productVariantService = MerchelloContext.Services.ProductVariantService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ProductVariantApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _productService = MerchelloContext.Services.ProductService;
            _productVariantService = MerchelloContext.Services.ProductVariantService;
        }

        /// <summary>
        /// Returns Product by id (key)
        /// 
        /// GET /umbraco/Merchello/ProductVariantApi/GetProductVariant?key={Guid}
        /// </summary>
        /// <param name="key"></param>
        public ProductVariantDisplay GetProductVariant(Guid key)
        {
            var productVariant = _productVariantService.GetByKey(key);
            if (productVariant == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return productVariant.ToProductVariantDisplay();
        }

        /// <summary>
        /// Returns ProductVariants by Product key
        /// 
        /// GET /umbraco/Merchello/ProductVariantApi/GetByProduct?key={guid}
        /// </summary>
        /// <param name="id"></param>
        public IEnumerable<ProductVariantDisplay> GetByProduct(Guid id)
        {
            if (id != Guid.Empty)
            {
                var productVariants = _productVariantService.GetByProductKey(id);
                if (productVariants == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                foreach (IProductVariant productVariant in productVariants)
                {
                    yield return productVariant.ToProductVariantDisplay();
                }
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
        /// Returns Product by keys separated by a comma
        /// 
        /// GET /umbraco/Merchello/ProductVariantApi/GetProductVariants?ids={int}&ids={int}
        /// </summary>
        /// <param name="ids"></param>
        public IEnumerable<ProductVariantDisplay> GetProductVariants([FromUri]IEnumerable<Guid> ids)
        {
            if (ids != null)
            {
                var productVariants = _productVariantService.GetByKeys(ids);
                if (productVariants == null)
                {
                    //throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                foreach (IProductVariant productVariant in productVariants)
                {
                    yield return productVariant.ToProductVariantDisplay();
                }
            }
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Parameter ids is null")),
                    ReasonPhrase = "Invalid Parameter"
                };
                throw new HttpResponseException(resp);
            }
        }

        ///  <summary>
        ///  Creates a product variant from Product & Attributes
        /// 
        ///  POST /umbraco/Merchello/ProductVariantApi/NewProductVariant
        ///  </summary>
        /// <param name="productVariant"></param>
        [AcceptVerbs("GET", "POST")]
        public ProductVariantDisplay NewProductVariant(ProductVariantDisplay productVariant)
        {
            IProductVariant newProductVariant = null;

            try
            {
                Product product = _productService.GetByKey(productVariant.ProductKey) as Product;

                if (product != null)
                {
                    ProductAttributeCollection productAttributes = new ProductAttributeCollection();
                    foreach (var attribute in productVariant.Attributes)
                    {
                        // TODO: This should be refactored into an extension method
                        ProductOption productOption = product.ProductOptions.FirstOrDefault(x => x.Key == attribute.OptionKey) as ProductOption;
                        // TODO: This should be refactored into an extension method
                        IProductAttribute productAttribute = null;
                        foreach (var attr in productOption.Choices)
                        {
                            if (attr.Name == attribute.Name)
                            {
                                productAttribute = attr;
                                break;
                            }
                        }
                        productAttributes.Add(attribute.ToProductAttribute(productAttribute));
                    }

                    newProductVariant = _productVariantService.CreateProductVariantWithKey(product, productVariant.Name, productVariant.Sku, productVariant.Price, productAttributes, true);
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return newProductVariant.ToProductVariantDisplay();
        }

        /// <summary>
        /// Updates an existing product
        ///
        /// PUT /umbraco/Merchello/ProductVariantApi/PutProductVariant
        /// </summary>
        /// <param name="productVariant">ProductVariantDisplay object serialized from WebApi</param>
        [AcceptVerbs("PUT")]
        public HttpResponseMessage PutProductVariant(ProductVariantDisplay productVariant)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                IProductVariant merchProductVariant = _productVariantService.GetByKey(productVariant.Key);
                merchProductVariant = productVariant.ToProductVariant(merchProductVariant);

                _productVariantService.Save(merchProductVariant);
            }
            catch (Exception ex) // I think this is not required as the server will create the error response message anyway
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Deletes an existing product
        ///
        /// DELETE /umbraco/Merchello/ProductVariantApi/{key}
        /// </summary>
        /// <param name="key"></param>
        public HttpResponseMessage Delete(Guid key)
        {
            var productVariantToDelete = _productVariantService.GetByKey(key);
            if (productVariantToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _productVariantService.Delete(productVariantToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
