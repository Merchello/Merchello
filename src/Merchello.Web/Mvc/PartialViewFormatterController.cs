namespace Merchello.Web.Mvc
{
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    internal class PartialViewFormatterController : SurfaceController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialViewFormatterController"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        /// <param name="umbracoHelper">
        /// The umbraco helper.
        /// </param>
        public PartialViewFormatterController(UmbracoContext umbracoContext, UmbracoHelper umbracoHelper)
            : base(umbracoContext, umbracoHelper)
        {
        }
    }
}