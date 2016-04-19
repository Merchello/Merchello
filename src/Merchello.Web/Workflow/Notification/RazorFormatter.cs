namespace Merchello.Web.Workflow.Notification
{
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
            Mandate.ParameterNotNull(model, "model");

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
            return ViewRenderer.RenderPartialView<PartialViewFormatterController>(viewPath, _model);
        }
    }
}