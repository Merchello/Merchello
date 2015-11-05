namespace Merchello.Web.Models.MapperResolvers
{
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Gateways.Payment;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;
    using Umbraco.Core.IO;

    /// <summary>
    /// Resolves the custom DialogEditorView property from the <see cref="PaymentGatewayMethodAttribute"/> for AutoMapper mappings
    /// </summary>
    internal class AuthorizePaymentDialogEditorViewResolver : ValueResolver<IPaymentGatewayMethod, DialogEditorViewDisplay>
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
        protected override DialogEditorViewDisplay ResolveCore(IPaymentGatewayMethod source)
        {
            // Check for custom attribute
            var editorAtt = source.GetType()
                .GetCustomAttributes<PaymentGatewayMethodAttribute>(false).FirstOrDefault();

            if (editorAtt != null)
                return new DialogEditorViewDisplay()
                {
                    Title = editorAtt.Title,
                    Description = string.Empty,
                    EditorView = editorAtt.AuthorizeCaptureEditorView.StartsWith("~/") ? IOHelper.ResolveUrl(editorAtt.AuthorizeEditorView) : editorAtt.AuthorizeEditorView
                };

            return null;
        }
    }
}
