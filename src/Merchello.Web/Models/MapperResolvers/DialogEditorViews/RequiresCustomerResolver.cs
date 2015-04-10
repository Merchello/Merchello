namespace Merchello.Web.Models.MapperResolvers
{
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Gateways.Payment;

    using Umbraco.Core;

    /// <summary>
    /// The requires customer resolver.
    /// </summary>
    internal class RequiresCustomerResolver : ValueResolver<IPaymentGatewayMethod, bool>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// Returns a bool indicating whether or not this payment method can only be used by known customers.
        /// </returns>
        protected override bool ResolveCore(IPaymentGatewayMethod source)
        {
            // Check for custom attribute
            var editorAtt = source.GetType().GetCustomAttributes<PaymentGatewayMethodAttribute>(false).FirstOrDefault();

            return editorAtt == null || editorAtt.RequiresCustomer;
        }
    }
}