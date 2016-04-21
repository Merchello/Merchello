namespace Merchello.Web.Workflow.Notification
{
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A very simple unresolved controller that is used for rendering views for such things as Notification email templates.
    /// </summary>
    internal class RazorFormatterController : SurfaceController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RazorFormatterController"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        /// <param name="umbracoHelper">
        /// The umbraco helper.
        /// </param>
        public RazorFormatterController(UmbracoContext umbracoContext, UmbracoHelper umbracoHelper)
            : base(umbracoContext, umbracoHelper)
        {
        }
    }
}