namespace Merchello.Plugin.Taxation.Avalara.Models.Tax
{
    using System.Collections.Generic;

    /// <summary>
    /// A base class to implement common properties in the AvaTax API Tax Result.
    /// </summary>
    public class GeoTaxResult
    {
        /// <summary>
        /// Gets or sets the tax rate.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets the tax amount.
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// Gets or sets the collection of tax details.
        /// </summary>
        public IEnumerable<TaxDetail> TaxDetails { get; set; }

        /// <summary>
        /// Gets or sets the result code indicating varying levels of success or failure in the tax request operation.
        /// </summary>
        public SeverityLevel ResultCode { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="IApiResponseMessage"/> returned from the API.
        /// </summary>
        public IEnumerable<ApiResponseMessage> Messages { get; set; }
    }
}