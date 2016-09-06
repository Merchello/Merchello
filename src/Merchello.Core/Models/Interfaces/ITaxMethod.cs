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
        /// Gets the key associated with the gateway provider for the tax rate data
        /// </summary>
        [DataMember]
        Guid ProviderKey { get; }

        /// <summary>
        /// Gets or sets the name associated with the tax method (e.g. VAT)
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets the two digit ISO Country code
        /// </summary>
        [DataMember]
        string CountryCode { get; }

        /// <summary>
        /// Gets or sets the percentage tax rate
        /// </summary>
        [DataMember]
        decimal PercentageTaxRate { get; set; }

        /// <summary>
        /// Gets or sets the collection that stores province adjustments (if any) for the tax country
        /// </summary>
        [DataMember]
        ProvinceCollection<ITaxProvince> Provinces { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product tax method.
        /// </summary>
        [DataMember]
        bool ProductTaxMethod { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the country has provinces
        /// </summary>
        [IgnoreDataMember]
        bool HasProvinces { get; }
    }
}