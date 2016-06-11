namespace Merchello.Web.Mvc
{
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Merchello.Core;
    using Merchello.Core.Logging;

    using Umbraco.Core;
    using Umbraco.Core.Configuration;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.Routing;
    using Umbraco.Web.Security;

    /// <summary>
    /// A helper to activate SurfaceControllers.
    /// </summary>
    /// <remarks>
    /// This is used for instances where we want to render views outside of MVC
    /// </remarks>
    internal class SurfaceControllerActivationHelper
    {
        /// <summary>
        /// Creates an instance of an Umbraco Surface controller from scratch 
        /// when no existing ControllerContext is present       
        /// </summary>
        /// <param name="routeData">
        /// The route Data.
        /// </param>
        /// <typeparam name="T">
        /// Type of the controller to create
        /// </typeparam>
        /// <returns>
        /// A surface controller of type T
        /// </returns>
        public static T CreateSurfaceController<T>(RouteData routeData = null)
                    where T : SurfaceController
        {
            // Create an MVC Controller Context
            var umbracoContext = GetUmbracoContext();

            var umbracoHelper = new UmbracoHelper(umbracoContext);

            var attempt = ActivatorHelper.CreateInstance<T>(typeof(T), new object[] { umbracoContext, umbracoHelper });

            if (!attempt.Success)
            {
                var data = MultiLogger.GetBaseLoggingData();
                data.AddCategory("SurfaceController");
                MultiLogHelper.Error<SurfaceControllerActivationHelper>("Failed to create render controller", attempt.Exception, data);
                throw attempt.Exception;
            }

            var controller = attempt.Result;

            if (routeData == null)
                routeData = new RouteData();

            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add(
                    "controller",
                    controller.GetType().Name.ToLower().Replace("controller", string.Empty));

            controller.ControllerContext = new ControllerContext(umbracoContext.HttpContext, routeData, controller);
            return controller;
        }

        /// <summary>
        /// The get umbraco context.
        /// </summary>
        /// <returns>
        /// The <see cref="UmbracoContext"/>.
        /// </returns>
        private static UmbracoContext GetUmbracoContext()
        {
            // If there is an existing UmbracoContext, return it
            if (UmbracoContext.Current != null) return UmbracoContext.Current;

            var request = new SimpleWorkerRequest(string.Empty, string.Empty, string.Empty, null, new StringWriter());

            var dummyHttpContext = new HttpContextWrapper(new HttpContext(request));

            // Create a brand new UmbracoContext so that we don't disturb any other threads that may be using UmbracoContext.Current
            return UmbracoContext.CreateContext(
                dummyHttpContext,
                ApplicationContext.Current,
                new WebSecurity(dummyHttpContext, ApplicationContext.Current),
                UmbracoConfig.For.UmbracoSettings(),
                UrlProviderResolver.Current.Providers,
                false);
        }
    }
}