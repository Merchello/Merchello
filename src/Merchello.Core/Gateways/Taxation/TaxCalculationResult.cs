namespace Merchello.Core.Gateways.Taxation
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents an invoice tax calculation result
    /// </summary>
    public class TaxCalculationResult : ITaxCalculationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxCalculationResult"/> class.
        /// </summary>
        /// <param name="taxRate">
        /// The tax rate.
        /// </param>
        /// <param name="taxAmount">
        /// The tax amount.
        /// </param>
        public TaxCalculationResult(decimal taxRate, decimal taxAmount)
            : this(string.Empty, taxRate, taxAmount)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxCalculationResult"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="taxRate">
        /// The tax rate.
        /// </param>
        /// <param name="taxAmount">
        /// The tax amount.
        /// </param>
        public TaxCalculationResult(string name, decimal taxRate, decimal taxAmount)
            : this(name, taxRate, taxAmount, new ExtendedDataCollection())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxCalculationResult"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="taxRate">
        /// The tax rate.
        /// </param>
        /// <param name="taxAmount">
        /// The tax amount.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        public TaxCalculationResult(string name, decimal taxRate, decimal taxAmount, ExtendedDataCollection extendedData)
        {
            Ensure.ParameterNotNull(extendedData, "extendedData");

            Name = string.IsNullOrEmpty(name) ? "Tax" : name;
            TaxRate = taxRate;
            TaxAmount = taxAmount;
            ExtendedData = extendedData;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the tax rate.
        /// </summary>
        public decimal TaxRate { get; private set; }

        /// <summary>
        /// Gets or sets the tax amount.
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Gets the extended data.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; private set; }
    }
}