namespace Merchello.Web.Models.MapperResolvers
{
    using AutoMapper;

    using Merchello.Core.EntityCollections;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;
    using Umbraco.Core.IO;

    /// <summary>
    /// Resolves the DialogEditorView for the provider.
    /// </summary>
    public class EntityCollectionProviderDialogEditorViewResolver : ValueResolver<EntityCollectionProviderAttribute, DialogEditorViewDisplay>
    {
        /// <summary>
        /// Resolves the <see cref="DialogEditorViewDisplay"/> for a <see cref="EntityCollectionProviderAttribute"/>.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="DialogEditorViewDisplay"/>.
        /// </returns>
        protected override DialogEditorViewDisplay ResolveCore(EntityCollectionProviderAttribute source)
        {
            if (source.EditorView.IsNullOrWhiteSpace()) return null;

            return new DialogEditorViewDisplay
                       {
                           Title = source.Name,
                           Description = source.Description,
                           EditorView =
                               source.EditorView.StartsWith("~/")
                                   ? IOHelper.ResolveUrl(source.EditorView)
                                   : source.EditorView
                       };

        }
    }
}