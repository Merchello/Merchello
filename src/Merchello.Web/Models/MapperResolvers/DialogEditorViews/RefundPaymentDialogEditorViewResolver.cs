namespace Merchello.Web.Models.MapperResolvers
{
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Gateways.Payment;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;
    using Umbraco.Core.IO;

    /// <summary>
    /// The refund payment dialog editor view resolver.
    /// </summary>
    internal class RefundPaymentDialogEditorViewResolver : ValueResolver<IPaymentGatewayMethod, DialogEditorViewDisplay>
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
            var editorAtt = source.GetType().GetCustomAttributes<PaymentGatewayMethodAttribute>(false).FirstOrDefault();

            if (editorAtt != null)
                return new DialogEditorViewDisplay()
                           {
                               Title = editorAtt.Title,
                               Description = string.Empty,
                               EditorView =
                                   editorAtt.RefundPaymentEditorView.StartsWith("~/")
                                       ? IOHelper.ResolveUrl(editorAtt.RefundPaymentEditorView)
                                       : editorAtt.RefundPaymentEditorView
                           };

            return null;
        }
    }
}