namespace Merchello.Web.Models.MapperResolvers
{
    using System.Linq;
    using AutoMapper;    
    using ContentEditing;
    using Core.Gateways;
    using Core.Models;    
    using Umbraco.Core;
    using Umbraco.Core.IO;

    /// <summary>
    /// Resolves the custom DialogEditorView property from the <see cref="GatewayProviderEditorAttribute"/> for AutoMapper mappings
    /// </summary>
    public class GatewayProviderDialogEditorViewResolver : ValueResolver<IGatewayProviderSettings, DialogEditorViewDisplay>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="DialogEditorViewDisplay"/>.
        /// </returns>
        protected override DialogEditorViewDisplay ResolveCore(IGatewayProviderSettings source)
        {
            // Check for custom attribute
            var provider = GatewayProviderResolver.Current.GetAllProviders().FirstOrDefault(x => x.Key == source.Key);

            if (provider == null) return null;

            var editorAtt = provider.GetType()
                .GetCustomAttributes<GatewayProviderEditorAttribute>(false).FirstOrDefault();

            if (editorAtt != null)
                return new DialogEditorViewDisplay()
                {
                    Title = editorAtt.Title,
                    Description = editorAtt.Description,
                    EditorView = editorAtt.EditorView.StartsWith("~/") ? IOHelper.ResolveUrl(editorAtt.EditorView) : editorAtt.EditorView
                };

            return null;
        }
    }
}