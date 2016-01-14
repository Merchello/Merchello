namespace Merchello.Bazaar
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using ClientDependency.Core.Mvc;

    using Merchello.Bazaar.Models.ViewModels;

    using Umbraco.Core.Models;

    /// <summary>
    /// Html Helper extension methods.
    /// </summary>
    /// <remarks>
    /// This is a paired down version of the Articule class of the same name
    /// https://github.com/Shazwazza/Articulate
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public static class HtmlHelperExtensions
    {       
        #region CSS & JS
        /// <summary>
        /// Adds Starter Kit Asset CSS file.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="theme">
        /// The current theme.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="HtmlHelper"/>.
        /// </returns>
        public static HtmlHelper RequiresPackageCss(this HtmlHelper html, string theme, string fileName)
        {
            return html.RequiresCss(string.Format("{0}Assets/css/{1}", PathHelper.GetThemePath(theme), fileName));
        }

        /// <summary>
        /// Adds Starter Kit Asset CSS file.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="theme">
        /// The current theme.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="HtmlHelper"/>.
        /// </returns>
        public static HtmlHelper RequiresPackageJs(this HtmlHelper html, string theme, string fileName)
        {
            return html.RequiresJs(string.Format("{0}Assets/js/{1}", PathHelper.GetThemePath(theme), fileName));
        } 

        #endregion

        #region Partial Views

        /// <summary>
        /// The themed partial.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="partialName">
        /// The partial name.
        /// </param>
        /// <param name="viewData">
        /// The view data.
        /// </param>
        /// <returns>
        /// The <see cref="IHtmlString"/>.
        /// </returns>
        public static IHtmlString ThemedPartial(this HtmlHelper html, IMasterModel model, string partialName, ViewDataDictionary viewData = null)
        {
            var path = PathHelper.GetThemePartialViewPath(model, partialName);
            return html.Partial(path, viewData);
        }

        /// <summary>
        /// The themed partial.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="partialName">
        /// The partial name.
        /// </param>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="viewData">
        /// The view data.
        /// </param>
        /// <returns>
        /// The <see cref="IHtmlString"/>.
        /// </returns>
        public static IHtmlString ThemedPartial(this HtmlHelper html, IMasterModel model, string partialName, object viewModel, ViewDataDictionary viewData = null)
        {
            var path = PathHelper.GetThemePartialViewPath(model, partialName);
            return html.Partial(path, viewModel, viewData);
        }

        /// <summary>
        /// The themed partial.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="themeName">
        /// The theme name.
        /// </param>
        /// <param name="partialName">
        /// The partial name.
        /// </param>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="viewData">
        /// The view data.
        /// </param>
        /// <returns>
        /// The <see cref="IHtmlString"/>.
        /// </returns>
        public static IHtmlString ThemedPartial(this HtmlHelper html, string themeName, string partialName, object viewModel, ViewDataDictionary viewData = null)
        {
            var path = PathHelper.GetThemePartialViewPath(themeName, partialName);
            return html.Partial(path, viewModel, viewData);
        }
        #endregion
    }
}