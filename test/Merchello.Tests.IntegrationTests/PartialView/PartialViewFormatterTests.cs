using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Merchello.Core.Models;
using Umbraco.Core;
using Umbraco.Web;

namespace Merchello.Tests.IntegrationTests.PartialView
{
    public class PartialViewFormatterTests
    {
        public static string RenderPartialViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            try
            {
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                    var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    public class NotificationFormatterController : Controller
    {
        public virtual ActionResult Index(INotificationMessage message)
        {
            throw new NotImplementedException();
        }
    }

    public interface IFormatter
    {

        string Format(string message);
    }


    public class NotificationFormatter
    {
        private HttpContextBase _httpContext;

        public NotificationFormatter()
        {
            
        }
    }
}