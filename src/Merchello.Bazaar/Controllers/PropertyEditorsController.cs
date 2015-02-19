namespace Merchello.Bazaar.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Umbraco.Core.IO;
    using Umbraco.Web.Editors;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The merchello starter kit property editors controller.
    /// </summary>
    [PluginController("Bazaar")]
    public class PropertyEditorsController : UmbracoAuthorizedJsonController
    {
        /// <summary>
        /// Gets a list of the theme folders.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> representation of the themes folder.
        /// </returns>
        public IEnumerable<string> GetThemes()
        {
            var dir = IOHelper.MapPath("~/App_Plugins/Merchello.Bazaar/Themes");
            return Directory.GetDirectories(dir).Select(x => new DirectoryInfo(x).Name);
        }
    }
}