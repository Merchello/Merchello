namespace Merchello.Web.Workflow.Notification
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Formatters;
    using Merchello.Web.Mvc;

    using Umbraco.Core;

    /// <summary>
    /// A formatter to for razor based messages.
    /// </summary>
    public class RazorFormatter : IFormatter
    {
        /// <summary>
        /// The model to be passed to the view.
        /// </summary>
        private readonly object _model;


        /// <summary>
        /// Initializes a new instance of the <see cref="RazorFormatter"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public RazorFormatter(object model)
        {
            Ensure.ParameterNotNull(model, "model");

            _model = model;
        }

        /// <summary>
        /// The format.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the partial view
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Format(string viewPath)
        {
            if (viewPath.StartsWith("~/views")) viewPath = viewPath.Replace("~/views", string.Empty);

            var renderer = GetViewRenderer();
            return renderer.RenderView(viewPath, _model);
        }

        /// <summary>
        /// Gets an instance of the view renderer.
        /// </summary>
        /// <returns>
        /// The <see cref="ViewRenderer"/>.
        /// </returns>
        private ViewRenderer GetViewRenderer()
        {
            var controller = SurfaceControllerActivationHelper.CreateSurfaceController<RazorFormatterController>();

            return new ViewRenderer(controller.ControllerContext);
        }
    }
}