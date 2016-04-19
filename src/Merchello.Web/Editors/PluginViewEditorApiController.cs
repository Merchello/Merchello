namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Configuration;
    using Merchello.Web.IO;
    using Merchello.Web.Models.ContentEditing.Templates;
    using Merchello.Web.WebApi;

    using Umbraco.Core;
    using Umbraco.Core.IO;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller for partial view editors.
    /// </summary>
    [PluginController("Merchello")]
    public class PluginViewEditorApiController : MerchelloApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginViewEditorApiController"/> class.
        /// </summary>
        public PluginViewEditorApiController()
            : this(Core.MerchelloContext.Current)
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginViewEditorApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public PluginViewEditorApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }


        /// <summary>
        /// Gets all of the App Plugin Views.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{AppPluginViewEditorContent}"/>.
        /// </returns>
        public IEnumerable<PluginViewEditorContent> GetAllAppPluginsViews()
        {
            var path = MerchelloConfiguration.Current.GetSetting("MerchelloTemplatesBasePath");
            return PluginViewHelper.GetAllViews(path);
        }

        /// <summary>
        /// The get all notification views.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{AppPluginViewEditorContent}"/>.
        /// </returns>
        public IEnumerable<PluginViewEditorContent> GetAllNotificationViews()
        {
            var path = MerchelloConfiguration.Current.GetSetting("NotificationTemplateBasePath");
            var views = PluginViewHelper.GetAllViews(path);
            return views;
        }


    }
}