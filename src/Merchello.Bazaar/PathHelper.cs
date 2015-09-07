namespace Merchello.Bazaar
{
    using Merchello.Bazaar.Models.ViewModels;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// Helper class to assist in starter kit specific path resolution.
    /// </summary>
    /// <remarks>
    /// Based on the PathHelper class in the Articulate package
    /// https://github.com/Shazwazza/Articulate
    /// </remarks>
    public static class PathHelper
    {
        /// <summary>
        /// Gets the path to the currently assigned theme.
        /// </summary>
        /// <param name="model">
        /// The <see cref="IMasterModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representing the path to the starter kit theme folder.
        /// </returns>
        public static string GetThemePath(IPublishedContent model)
        {
            const string Path = "~/App_Plugins/Merchello.Bazaar/Themes/{0}/";
            return model.HasProperty("theme") && model.HasValue("theme") ? 
                string.Format(Path, model.GetPropertyValue<string>("theme")) :
                string.Empty;
        }

        /// <summary>
        /// The get theme account path.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetThemeAccountPath(IMasterModel model)
        {
            const string Path = "~/App_Plugins/Merchello.Bazaar/Themes/{0}/Account/";
            return string.Format(Path, model.Theme);
        }


        /// <summary>
        /// The get theme partial view path.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetThemePartialViewPath(IMasterModel model, string viewName)
        {
            return GetThemePartialViewPath(model.Theme, viewName);
        }       

        /// <summary>
        /// The get theme partial view path.
        /// </summary>
        /// <param name="theme">
        /// The theme.
        /// </param>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetThemePartialViewPath(string theme, string viewName)
        {
            var path = string.Format("{0}{1}", GetThemePath(theme), "Views/Partials/{0}.cshtml");
            return string.Format(path, viewName);
        }

        /// <summary>
        /// Gets the theme path.
        /// </summary>
        /// <param name="theme">
        /// The theme.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetThemePath(string theme)
        {
            const string Path = "~/App_Plugins/Merchello.Bazaar/Themes/{0}/";
            return string.Format(Path, theme);
        }
    }
}