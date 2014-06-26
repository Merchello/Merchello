namespace Merchello.Plugin.Taxation.Avalara.Provider
{
    using Core.Gateways.Taxation;
    using Core.Models;
    
    /// <summary>
    /// Represents the Avalara taxation gateway method
    /// </summary>
    public class AvalaraTaxationGatewayMethod : TaxationGatewayMethodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvalaraTaxationGatewayMethod"/> class.
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        public AvalaraTaxationGatewayMethod(ITaxMethod taxMethod) : base(taxMethod)
        {
        }

        /// <summary>
        /// The calculate tax for invoice.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>        
        public override ITaxCalculationResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress)
        {
            throw new System.NotImplementedException();
        }
    }
}