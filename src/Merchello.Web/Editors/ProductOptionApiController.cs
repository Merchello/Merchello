namespace Merchello.Web.Editors
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.ModelBinding;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Counting;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.WebApi;
    using Merchello.Web.WebApi.Binders;
    using Merchello.Web.WebApi.Filters;

    using Umbraco.Core;
    using Umbraco.Core.Services;
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
        /// The <see cref="IContentTypeService"/>.
        /// </summary>
        private readonly IContentTypeService _contentTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionApiController"/> class.
        /// </summary>
        public ProductOptionApiController()
            : this(Core.MerchelloContext.Current, ApplicationContext.Current.Services.ContentTypeService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="contentTypeService">
        /// Umbraco's <see cref="IContentTypeService"/>
        /// </param>
        public ProductOptionApiController(IMerchelloContext merchelloContext, IContentTypeService contentTypeService)
             : base(merchelloContext)
        {
            Mandate.ParameterNotNull(contentTypeService, "contentTypeService");
            _contentTypeService = contentTypeService;
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

            var debug = page.Items.Last();

            foreach (var att in debug.Choices)
            {
                var ta = att.ToProductAttributeDisplay();
                var temp = "";
            }

            var debugMapped = debug.ToProductOptionDisplay();

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

        /// <summary>
        /// Puts (saves) a product option attribute with content.
        /// </summary>
        /// <param name="attributeContentItem">
        /// The attribute content item.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeDisplay"/>.
        /// </returns>
        [FileUploadCleanupFilter]
        [HttpPost, HttpPut]
        public ProductAttributeDisplay PutProductAttributeDetachedContent(
            [ModelBinder(typeof(ProductAttributeContentSaveBinder))] 
            ProductAttributeContentSave attributeContentItem)
        {
            var contentTypeAlias = attributeContentItem.DetachedContentType.UmbContentType.Alias;
            var contentType = _contentTypeService.GetContentType(contentTypeAlias);

            if (contentType == null)
            {
                var nullRef = new NullReferenceException("Could not find ContentType with alias: " + contentTypeAlias);
                MultiLogHelper.Error<ProductOptionApiController>("Failed to find content type", nullRef);
                throw nullRef;
            }

            var attribute = attributeContentItem.Display;

            attribute.DetachedDataValues = DetachedContentHelper.GetUpdatedValues<ProductAttributeContentSave, ProductAttributeDisplay>(contentType, attributeContentItem);
            var destination = _productOptionService.GetProductAttributeByKey(attribute.Key);
            destination = attribute.ToProductAttribute(destination);

            ((ProductOptionService)_productOptionService).Save(destination);

            return destination.ToProductAttributeDisplay();
            
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