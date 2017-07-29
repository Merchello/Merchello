namespace Merchello.Core.Models
{
    using System;
    

    /// <summary>
    /// Represents a taxation method.
    /// </summary>
    public interface ITaxMethod : IGatewayProviderMethod
    {
        /// <summary>
        /// Gets the key associated with the gateway provider for the tax rate data
        /// </summary>
        
        Guid ProviderKey { get; }

        /// <summary>
        /// Gets or sets the name associated with the tax method (e.g. VAT)
        /// </summary>
        
        string Name { get; set; }

        /// <summary>
        /// Gets the two digit ISO Country code
        /// </summary>
        
        string CountryCode { get; }

        /// <summary>
        /// Gets or sets the percentage tax rate
        /// </summary>
        
        decimal PercentageTaxRate { get; set; }

        /// <summary>
        /// Gets or sets the collection that stores province adjustments (if any) for the tax country
        /// </summary>
        
        //ProvinceCollection<ITaxProvince> Provinces { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product tax method.
        /// </summary>
        
        bool ProductTaxMethod { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the country has provinces
        /// </summary>
        bool HasProvinces { get; }
    }
}