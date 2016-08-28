namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a Tax Method
    /// </summary>
    public interface ITaxMethod : IGatewayProviderMethod
    {
        /// <summary>
        /// The key associated with the gateway provider for the tax rate data
        /// </summary>
        [DataMember]
        Guid ProviderKey { get; }

        /// <summary>
        /// The name assoicated with the tax method (eg. VAT)
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The two digit ISO Country code
        /// </summary>
        [DataMember]
        string CountryCode { get; }

        /// <summary>
        /// Percentage tax rate
        /// </summary>
        [DataMember]
        decimal PercentageTaxRate { get; set; }

        /// <summary>
        /// Stores province adjustments (if any) for the tax country
        /// </summary>
        [DataMember]
        ProvinceCollection<ITaxProvince> Provinces { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product tax method.
        /// </summary>
        [DataMember]
        bool ProductTaxMethod { get; set; }

        /// <summary>
        /// True/false indicating whether or not the CountryTaxRate has provinces
        /// </summary>
        [IgnoreDataMember]
        bool HasProvinces { get; }
    }
}