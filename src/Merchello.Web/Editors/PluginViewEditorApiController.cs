namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    using Merchello.Core;
    using Merchello.Core.Configuration;
    using Merchello.Core.Logging;
    using Merchello.Web.Exceptions;
    using Merchello.Web.Models.ContentEditing.Templates;
    using Merchello.Web.Pluggable;
    using Merchello.Web.WebApi;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller for partial view editors.
    /// </summary>
    [PluginController("Merchello")]
    public class PluginViewEditorApiController : MerchelloApiController
    {
        /// <summary>
        /// The base log data.
        /// </summary>
        private IExtendedLoggerData _logData;

        /// <summary>
        /// The <see cref="PluginViewEditorProvider"/>.
        /// </summary>
        private PluginViewEditorProvider _provider;

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
            Initialize();
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
            return _provider.GetAllViews(path);
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
            var views = _provider.GetAllViews(path);
            return views;
        }

        /// <summary>
        /// Adds a new view.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        public PluginViewEditorContent AddNewView(PluginViewEditorContent content)
        {
            try
            {
                return _provider.CreateNewView(content.FileName, content.PluginViewType, content.ModelTypeName, content.ViewBody);
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<PluginViewEditorApiController>("View creation failed", ex, _logData);
                throw;
            }
        }

        /// <summary>
        /// Saves the view.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="PluginViewEditorContent"/>.
        /// </returns>
        public PluginViewEditorContent SaveView(PluginViewEditorContent content)
        {
            if (_provider.SaveView(content.FileName, content.PluginViewType, content.ViewBody))
            {
                return content;
            }

            var ex = new MerchelloApiException("Failed to save view");
            MultiLogHelper.Error<PluginViewEditorApiController>("View save failed", ex, _logData);
            throw ex;
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            _logData = MultiLogger.GetBaseLoggingData();
            _logData.AddCategory("Pluggable");

            try
            {
                _provider = PluggableObjectHelper.GetInstance<PluginViewEditorProvider>("PluginViewEditorProvider");
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<PluginViewEditorApiController>("Failed to instantiate PlugViewEditorProvider", ex, _logData);
                throw;
            }
        }
    }
}