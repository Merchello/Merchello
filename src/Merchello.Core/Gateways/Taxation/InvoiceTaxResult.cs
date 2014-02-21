using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Represents an invoice tax calculation result
    /// </summary>
    public class InvoiceTaxResult : IInvoiceTaxResult
    {
        public InvoiceTaxResult(decimal taxRate, decimal taxAmount)
            : this(taxRate, taxAmount, new ExtendedDataCollection())
        { }

        public InvoiceTaxResult(decimal taxRate, decimal taxAmount, ExtendedDataCollection extendedData)
        {
            Mandate.ParameterNotNull(extendedData, "extendedData");
           
            TaxRate = taxRate;
            TaxAmount = taxAmount;
            ExtendedData = extendedData;
        }

        public decimal TaxRate { get; private set; }
        public decimal TaxAmount { get; set; }
        public ExtendedDataCollection ExtendedData { get; private set; }
    }
}