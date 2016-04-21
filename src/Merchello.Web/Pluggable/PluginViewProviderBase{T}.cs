namespace Merchello.Web.Pluggable
{
    using System.Collections.Generic;
    using System.IO;

    using Merchello.Core.Configuration;
    using Merchello.Web.Models.ContentEditing.Templates;

    using Umbraco.Core.IO;

    /// <summary>
    /// The plugin view service base.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of model to represents a view
    /// </typeparam>
    internal abstract class PluginViewProviderBase<TModel>
        where TModel : class, new()
    {
        /// <summary>
        /// Gets all views at or below the virtual path.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TModel}"/>.
        /// </returns>
        public abstract IEnumerable<TModel> GetAllViews(string virtualPath);

        /// <summary>
        /// Creates a new view.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="viewType">
        /// The view type.
        /// </param>
        /// <param name="modelName">
        /// The model name.
        /// </param>
        /// <param name="viewBody">
        /// The view body.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the create was successful.
        /// </returns>
        public abstract bool CreateNewView(string fileName, PluginViewType viewType, string modelName, string viewBody);

        /// <summary>
        /// Saves an existing view.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="viewType">
        /// The view type.
        /// </param>
        /// <param name="viewBody">
        /// The view body.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the save was successful.
        /// </returns>
        public abstract bool SaveView(string fileName, PluginViewType viewType, string viewBody);


        /// <summary>
        /// Ensures that the mapped path exists.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected static string EnsureMappedPath(string virtualPath)
        {
            var mapped = IOHelper.MapPath(virtualPath);
            if (!Directory.Exists(mapped))
            {
                Directory.CreateDirectory(mapped);
            }

            return mapped;
        }

        /// <summary>
        /// Gets the virtual path by view type.
        /// </summary>
        /// <param name="viewType">
        /// The view type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected static string GetVirtualPathByPlugViewType(PluginViewType viewType)
        {
            switch (viewType)
            {
                default:
                    return MerchelloConfiguration.Current.GetSetting("NotificationTemplateBasePath");
            }
        }
    }
}