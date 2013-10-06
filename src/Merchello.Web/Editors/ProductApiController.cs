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
        public ProductActual GetProduct(Guid key)
        {
            if (key != null)
            {
                var product = _productService.GetByKey(key) as ProductActual;
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
        /// Returns All Products
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProducts
        /// </summary>
        /// <param name="keys"></param>
        public IEnumerable<Product> GetAllProducts()
        {
            ProductService tempProductService = _productService as ProductService;
            var products = tempProductService.GetAll();
            if (products == null)
            {
                // ?
            }

            foreach (IProduct product in products)
            {
                yield return product as Product;
            }
        }

        /// <summary>
        /// Returns Product by keys separated by a comma
        /// 
        /// GET /umbraco/Merchello/ProductApi/GetProducts?keys={guid}&keys={guid}
        /// </summary>
        /// <param name="keys"></param>
        public IEnumerable<ProductActual> GetProducts([FromUri]IEnumerable<Guid> keys)
        {
            if (keys != null)
            {
                var products = _productService.GetByKeys(keys);
                if (products == null)
                {
                    //throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                foreach(IProductActual product in products)
                {
                    yield return product as ProductActual;
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
        public ProductActual NewProduct(string sku, string name, decimal price)
        {
            ProductActual newProductActual = null;

            try
            {
                newProductActual = _productService.CreateProduct(sku, name, price) as ProductActual;
                _productService.Save(newProductActual);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return newProductActual;
        }

        /// <summary>
        /// Updates an existing product
        ///
        /// PUT /umbraco/Merchello/ProductApi/PutProduct
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        [AcceptVerbs("PUT")]
        public HttpResponseMessage PutProduct(ProductActual productActual)
        {
            // I think we should consider having a specific objects in .Web to pass back and forth
            // via the Api like this so that we can take advantage of the Model.IsValid.  Umbraco does this with
            // their various models in : Umbraco.Web.Models.ContentEditing

            // Mapping between the models will be pretty straight forward with the AutoMapper stuff (http://automapper.org/)


            var response = Request.CreateResponse(HttpStatusCode.OK);
                        
            try
            {
				_productService.Save(productActual);
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
    }
}
