namespace Merchello.Web.Models.MapperResolvers
{
    using System.Linq;
    using AutoMapper;    
    using ContentEditing;
    using Core.Gateways;
    using Umbraco.Core;
    using Umbraco.Core.IO;

    /// <summary>
    /// Resolves the custom DialogEditorView property from the <see cref="GatewayProviderEditorAttribute"/> for AutoMapper mappings
    /// </summary>
    public class GatewayMethodDialogEditorViewResolver : ValueResolver<IGatewayMethod, DialogEditorViewDisplay>
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