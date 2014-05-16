using System.Linq;
using AutoMapper;
using Merchello.Core.Gateways;
using Merchello.Core.Models;
using Merchello.Web.Models.ContentEditing;
using Umbraco.Core;
using Umbraco.Core.IO;

namespace Merchello.Web.Models.MapperResolvers
{
    /// <summary>
    /// Resolves the custom DialogEditorView property from the <see cref="GatewayProviderEditorAttribute"/> for AutoMapper mappings
    /// </summary>
    public class GatewayProviderDialogEditorViewResolver : ValueResolver<IGatewayProviderSettings, DialogEditorViewDisplay>
    {
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