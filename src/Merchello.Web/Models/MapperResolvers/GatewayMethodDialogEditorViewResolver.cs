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
    public class GatewayMethodDialogEditorViewResolver : ValueResolver<IGatewayMethod, DialogEditorViewDisplay>
    {
        protected override DialogEditorViewDisplay ResolveCore(IGatewayMethod source)
        {
            // Check for custom attribute
            var editorAtt = source.GetType()
                .GetCustomAttributes<GatewayMethodEditorAttribute>(false).FirstOrDefault();

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