namespace Merchello.Web.Mvc
{
    using System.Web.Mvc;

    /// <summary>
    /// Similar to ChildActionOnly but for AJAX returned partial views
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/questions/4361742/prevent-partial-view-from-loading"/>
    public class CheckAjaxRequestAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Http header for AJAX requests.
        /// </summary>
        private const string AJAXHEADER = "X-Requested-With";

        /// <summary>
        /// Performs the filtering.
        /// </summary>
        /// <param name="filterContext">
        /// The filter context.
        /// </param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isAjaxRequest = filterContext.HttpContext.Request.Headers[AJAXHEADER] != null;
            if (!isAjaxRequest)
            {
                filterContext.Result = new ViewResult { ViewName = "Unauthorized" };
            }
        }
    }
}