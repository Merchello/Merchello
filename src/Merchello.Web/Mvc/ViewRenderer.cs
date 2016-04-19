namespace Merchello.Web.Mvc
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.UI.WebControls;

    using Merchello.Core;
    using Merchello.Core.Logging;

    using Umbraco.Core;
    using Umbraco.Core.Configuration;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.Routing;
    using Umbraco.Web.Security;

    /// <summary>
    /// Class that renders MVC views to a string using the
    /// standard MVC View Engine to render the view. 
    /// </summary>
    /// <remarks>
    /// Based off of Rick Stahl's (http://weblog.west-wind.com/) posts adapted to use UmbracoContext and surface controllers
    /// </remarks>
    /// <seealso cref="http://weblog.west-wind.com/posts/2012/May/30/Rendering-ASPNET-MVC-Views-to-String"/>
    internal class ViewRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRenderer"/> class. 
        /// </summary>
        /// <param name="controllerContext">
        /// If you are running within the context of an ASP.NET MVC request pass in
        /// the controller's context. 
        /// Only leave out the context if no context is otherwise available.
        /// </param>
        public ViewRenderer(ControllerContext controllerContext = null)
        {
            Initialize(controllerContext);
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        protected ControllerContext Context { get; set; }

        /// <summary>
        /// Renders a view.
        /// </summary>
        /// <param name="viewPath">
        /// The view path.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="routeData">
        /// The route data.
        /// </param>
        /// <typeparam name="TController">
        /// The type of the controller
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RenderView<TController>(string viewPath, object model, RouteData routeData = null)
            where TController : SurfaceController
        {
            var controller = CreateSurfaceController<TController>(routeData);

            try
            {
                return RenderView(viewPath, model, controller.ControllerContext);
            }
            catch (Exception ex)
            {
                MultiLogHelper.WarnWithException<ViewRenderer>("Failed to render partial view", ex, MultiLogger.GetBaseLoggingData());

                return ex.Message;
            }
        }

        /// <summary>
        /// Renders a partial view.
        /// </summary>
        /// <param name="viewPath">
        /// The view path.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="routeData">
        /// The route data.
        /// </param>
        /// <typeparam name="TController">
        /// The type of the surface controller
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RenderPartialView<TController>(string viewPath, object model, RouteData routeData = null)
            where TController : SurfaceController
        {
            var controller = CreateSurfaceController<TController>(routeData);

            try
            {
                return RenderPartialView(viewPath, model, controller.ControllerContext);
            }
            catch (Exception ex)
            {
                MultiLogHelper.WarnWithException<ViewRenderer>("Failed to render partial view", ex, MultiLogger.GetBaseLoggingData());

                return ex.Message;
            }
        }

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
                data.AddCategory("Rendering");
                MultiLogHelper.Error<ViewRenderer>("Failed to create render controller", attempt.Exception, data);
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
        /// Renders a partial MVC view to string. Use this method to render
        /// a partial view that doesn't merge with _Layout and doesn't fire
        /// _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="controllerContext">Active Controller context</param>
        /// <returns>String of the rendered view or null on error</returns>
        internal static string RenderView(string viewPath, object model, ControllerContext controllerContext)
        {
            var renderer = new ViewRenderer(controllerContext);
            return renderer.RenderView(viewPath, model);
        }

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render
        /// a partial view that doesn't merge with _Layout and doesn't fire
        /// _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="controllerContext">Active controller context</param>
        /// <returns>String of the rendered view or null on error</returns>
        internal static string RenderPartialView(string viewPath, object model, ControllerContext controllerContext)
        {
            var renderer = new ViewRenderer(controllerContext);
            return renderer.RenderPartialView(viewPath, model);
        }

        /// <summary>
        /// Renders a full MVC view to a string. Will render with the full MVC
        /// View engine including running _ViewStart and merging into _Layout        
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to render the view with</param>
        /// <returns>String of the rendered view or null on error</returns>
        internal string RenderView(string viewPath, object model)
        {
            return RenderViewToStringInternal(viewPath, model, false);
        }

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render
        /// a partial view that doesn't merge with _Layout and doesn't fire
        /// _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <returns>String of the rendered view or null on error</returns>
        internal string RenderPartialView(string viewPath, object model)
        {
            return RenderViewToStringInternal(viewPath, model, true);
        }

        /// <summary>
        /// Internal method that handles rendering of either partial or 
        /// or full views.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">Model to render the view with</param>
        /// <param name="partial">Determines whether to render a full or partial view</param>
        /// <returns>String of the rendered view</returns>
        protected string RenderViewToStringInternal(string viewPath, object model, bool partial = false)
        {
            var viewEngineResult = partial ? 
                ViewEngines.Engines.FindPartialView(this.Context, viewPath) : 
                ViewEngines.Engines.FindView(this.Context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            Context.Controller.ViewData.Model = model;

            string result;
 
            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(Context, view, Context.Controller.ViewData, Context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
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

        /// <summary>
        /// The initializes the ViewRenderer.
        /// </summary>
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        private void Initialize(ControllerContext controllerContext)
        {
            // Create a known controller from HttpContext if no context is passed
            if (controllerContext == null)
            {
                var invalidOp = new InvalidOperationException(
                    "ViewRenderer must run in the context of an ASP.NET " +
                    "Application and requires HttpContext.Current to be present.");
            }

            Context = controllerContext;
        }
    }
}