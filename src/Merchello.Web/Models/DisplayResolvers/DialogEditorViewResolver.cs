using System;
using System.Linq;
using AutoMapper;
using Merchello.Core.Gateways;
using Merchello.Core.Models;
using Merchello.Web.Models.ContentEditing;
using Umbraco.Core;
using Umbraco.Core.IO;

namespace Merchello.Web.Models.DisplayResolvers
{
    /// <summary>
    /// Resolves the custom DialogEditorView property from the <see cref="GatewayProviderEditorAttribute"/> for AutoMapper mappings
    /// </summary>
    public class DialogEditorViewResolver : ValueResolver<IGatewayProvider, DialogEditorViewDisplay>
    {
        protected override DialogEditorViewDisplay ResolveCore(IGatewayProvider source)
        {
            // Check for custom attribute
            var editorAtt = Type.GetType(source.TypeFullName)
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