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
    internal class IncludeInPaymentSelectionResolver : ValueResolver<IPaymentGatewayMethod, bool>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// A bool indicating whether or not this payment method should be included in payment method selection.
        /// </returns>
        protected override bool ResolveCore(IPaymentGatewayMethod source)
        {
            // Check for custom attribute
            var editorAtt = source.GetType().GetCustomAttributes<PaymentGatewayMethodAttribute>(false).FirstOrDefault();

            return editorAtt == null || editorAtt.IncludeInPaymentSelection;

        }
    }
}