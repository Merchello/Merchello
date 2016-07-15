namespace Merchello.Web.Editors
{
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.WebApi;

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
    }
}