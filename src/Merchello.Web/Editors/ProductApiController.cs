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

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class ProductApiController : MerchelloApiController
    {
        private IProductService _productService;

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
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ProductApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _productService = MerchelloContext.Services.ProductService;
        }

        /// <summary>
        /// Returns Product by id (key)
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProduct?key={guid}
        /// </summary>
        /// <param name="key"></param>
        public Product GetProduct(Guid key)
        {
            if (key != null)
            {
                var product = _productService.GetByKey(key) as Product;
                if (product == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                return product;
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
        /// GET /umbraco/Merchello/ProductApi/GetProducts?keys={guid}&keys={guid}
        /// </summary>
        /// <param name="keys"></param>
        public IEnumerable<Product> GetProducts([FromUri]IEnumerable<Guid> keys)
        {
            if (keys != null)
            {
                var products = _productService.GetByKeys(keys);
                if (products == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                foreach(IProduct product in products)
                {
                    yield return product as Product;
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
        public Product NewProduct(string sku, string name, decimal price)
        {
            Product newProduct = null;

            try
            {
                newProduct = _productService.CreateProduct(sku, name, price) as Product;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return newProduct;
        }

        /// <summary>
        /// Updates an existing product
        ///
        /// PUT /umbraco/Merchello/ProductApi/SaveProduct
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        [AcceptVerbs("PUT")]
        public HttpResponseMessage PutProduct(Product item)
        {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            try
            {
                _productService.Save(item);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(String.Format("{0}", ex.Message)),
                    ReasonPhrase = "Internal Error"
                };
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
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.NoContent);
            try
            {
                var productToDelete = _productService.GetByKey(key);
                _productService.Delete(productToDelete);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(String.Format("{0}", ex.Message)),
                    ReasonPhrase = "Internal Error"
                };
            }

            return response;
        }
    }
}
