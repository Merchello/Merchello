namespace Merchello.Web.Mvc
{
    using System;
    using System.IO;
    using System.Web.Mvc;

    using Merchello.Core.Logging;

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
        public ViewRenderer(ControllerContext controllerContext)
        {
            Initialize(controllerContext);
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        protected ControllerContext Context { get; set; }

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
        public string RenderView(string viewPath, object model)
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
        public string RenderPartialView(string viewPath, object model)
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
        public string RenderViewToStringInternal(string viewPath, object model, bool partial = false)
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

                var logData = MultiLogger.GetBaseLoggingData();

                MultiLogHelper.Error<ViewRenderer>("ControllerContext was null", invalidOp, logData);
                throw invalidOp;
            }

            Context = controllerContext;
        }
    }
}