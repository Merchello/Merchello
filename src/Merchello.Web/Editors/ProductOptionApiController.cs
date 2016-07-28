namespace Merchello.Web.Editors
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.ModelBinding;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Counting;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.WebApi;
    using Merchello.Web.WebApi.Binders;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The API controller responsible for product option configurations.
    /// </summary>
    [PluginController("Merchello")]
    public class ProductOptionApiController : MerchelloApiController
    {
        /// <summary>
        /// The <see cref="IProductOptionService"/>.
        /// </summary>
        private readonly IProductOptionService _productOptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionApiController"/> class.
        /// </summary>
        public ProductOptionApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ProductOptionApiController(IMerchelloContext merchelloContext)
             : base(merchelloContext)
        {
            _productOptionService = merchelloContext.Services.ProductOptionService;
        }

        /// <summary>
        /// Gets the <see cref="ProductOptionDisplay"/> by it's key.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ProductOptionDisplay"/>.
        /// </returns>
        [HttpGet]
        public ProductOptionDisplay GetByKey(Guid id)
        {
            var option = _productOptionService.GetByKey(id);
            return option.ToProductOptionDisplay();
        }

        /// <summary>
        /// Gets the use count for the product option.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ProductOptionUseCount"/>.
        /// </returns>
        [HttpGet]
        public ProductOptionUseCount GetProductOptionUseCount(Guid id)
        {
            var option = _productOptionService.GetByKey(id);

            return _productOptionService.GetProductOptionUseCount(option) as ProductOptionUseCount;
        }

        /// <summary>
        /// Provides a paged listing of product options.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        [HttpPost]
        public QueryResultDisplay SearchOptions(QueryDisplay query)
        {
            var termParam = query.Parameters.FirstOrDefault(x => x.FieldName == "term");
            var term = termParam == null ? string.Empty : termParam.Value;

            var sharedOnlyParam = query.Parameters.FirstOrDefault(x => x.FieldName == "sharedOnly");

            var sharedOnly = sharedOnlyParam == null;

            var page = _productOptionService.GetPage(term, query.CurrentPage + 1, query.ItemsPerPage, query.SortBy, query.SortDirection, sharedOnly);

            return page.ToQueryResultDisplay(AutoMapper.Mapper.Map<IProductOption, ProductOptionDisplay>);
        }

        /// <summary>
        /// Creates a new <see cref="IProductOption"/>.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="ProductOptionDisplay"/>.
        /// </returns>
        [HttpPost]
        public ProductOptionDisplay PostProductOption(ProductOptionDisplay option)
        {
            var productOption = option.ToProductOption(new ProductOption(option.Name));
            _productOptionService.Save(productOption);

            return productOption.ToProductOptionDisplay();
        }

        /// <summary>
        /// Puts (saves) a product option.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// The <see cref="ProductOptionDisplay"/>.
        /// </returns>
        [HttpPut, HttpPost]
        public ProductOptionDisplay PutProductOption(ProductOptionDisplay option)
        {
            var destination = _productOptionService.GetByKey(option.Key);
            
            destination = option.ToProductOption(destination);
            _productOptionService.Save(destination);

            return destination.ToProductOptionDisplay();
        }

        public ProductAttributeDisplay PutProductAttributeDetachedContent(
            [ModelBinder(typeof(ProductAttributeContentSaveBinder))] 
            ProductAttributeContentSave attributeContentItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a product option.
        /// </summary>
        /// <param name="id">
        /// The option key.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet, HttpDelete]
        public HttpResponseMessage DeleteProductOption(Guid id)
        {
            try
            {
                var option = _productOptionService.GetByKey(id);
                _productOptionService.Delete(option);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}