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
    public class ProductVariantApiController : MerchelloApiController
    {
        private IProductVariantService _productVariantService;

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
            _productVariantService = MerchelloContext.Services.ProductVariantService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ProductVariantApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _productVariantService = MerchelloContext.Services.ProductVariantService;
        }

        /// <summary>
        /// Returns Product by id (key)
        /// 
        /// GET /umbraco/Merchello/ProductVariantApi/GetProductVariant?id={int}
        /// </summary>
        /// <param name="key"></param>
        public IProductVariant GetProductVariant(int id)
        {
            var productVariant = _productVariantService.GetById(id);
            if (productVariant == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return productVariant;
        }

        /// <summary>
        /// Returns ProductVariants by Product key
        /// 
        /// GET /umbraco/Merchello/ProductVariantApi/GetByProduct?key={guid}
        /// </summary>
        /// <param name="key"></param>
        public IEnumerable<IProductVariant> GetByProduct(Guid key)
        {
            if (key != null)
            {
                var productVariants = _productVariantService.GetByProductKey(key);
                if (productVariants == null || productVariants.Count() == 0)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                return productVariants;
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
        public IEnumerable<IProductVariant> GetProductVariants([FromUri]IEnumerable<int> ids)
        {
            if (ids != null)
            {
                var productVariants = _productVariantService.GetByIds(ids);
                if (productVariants == null)
                {
                    //throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                return productVariants;
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


        /// <summary>
        /// Updates an existing product
        ///
        /// PUT /umbraco/Merchello/ProductVariantApi/PutProductVariant
        /// </summary>
        /// <param name="product">ProductDisplay object serialized from WebApi</param>
        [AcceptVerbs("PUT")]
        public HttpResponseMessage PutProduct(IProductVariant productVariant)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                IProductVariant merchProductVariant = _productVariantService.GetById(productVariant.Id);

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
        /// DELETE /umbraco/Merchello/ProductVariantApi/{id}
        /// </summary>
        /// <param name="key"></param>
        public HttpResponseMessage Delete(int id)
        {
            var productVariantToDelete = _productVariantService.GetById(id);
            if (productVariantToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _productVariantService.Delete(productVariantToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
