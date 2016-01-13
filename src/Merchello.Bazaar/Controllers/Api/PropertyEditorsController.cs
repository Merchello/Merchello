namespace Merchello.Bazaar.Controllers.Api
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core.Configuration;

    using Umbraco.Core.IO;
    using Umbraco.Web.Editors;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The merchello starter kit property editors controller.
    /// </summary>
    [PluginController("Bazaar")]
    public partial class PropertyEditorsController : UmbracoAuthorizedJsonController
    {
        /// <summary>
        /// Gets a list of the theme folders.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> representation of the themes folder.
        /// </returns>
        [HttpGet]
        public IEnumerable<string> GetThemes()
        {
            var dir = IOHelper.MapPath("~/App_Plugins/Merchello.Bazaar/Themes");
            return Directory.GetDirectories(dir).Select(x => new DirectoryInfo(x).Name);
        }

        /// <summary>
        /// The get member types.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<string> GetMemberTypes()
        {
            //// Get all of the Umbraco Member Types
            var memberTypes = this.Services.MemberTypeService.GetAll();

            //// We need to filter by those configured in the merchello.config file
            var allowed = MerchelloConfiguration.Current.CustomerMemberTypes;

            return memberTypes.Where(x => allowed.Contains(x.Name)).Select(x => x.Name);

        }
    }
}