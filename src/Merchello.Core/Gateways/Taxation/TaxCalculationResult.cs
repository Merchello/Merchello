using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    using Umbraco.Core;

    /// <summary>
    /// Represents an invoice tax calculation result
    /// </summary>
    public class TaxCalculationResult : ITaxCalculationResult
    {
        public TaxCalculationResult(decimal taxRate, decimal taxAmount)
            : this(string.Empty, taxRate, taxAmount)
        {}

        public TaxCalculationResult(string name, decimal taxRate, decimal taxAmount)
            : this(name, taxRate, taxAmount, new ExtendedDataCollection())
        { }

        public TaxCalculationResult(string name, decimal taxRate, decimal taxAmount, ExtendedDataCollection extendedData)
        {
            Mandate.ParameterNotNull(extendedData, "extendedData");

            Name = string.IsNullOrEmpty(name) ? "Tax" : name;
            TaxRate = taxRate;
            TaxAmount = taxAmount;
            ExtendedData = extendedData;
        }

        public string Name { get; private set; }
        public decimal TaxRate { get; private set; }
        public decimal TaxAmount { get; set; }
        public ExtendedDataCollection ExtendedData { get; private set; }
    }
}