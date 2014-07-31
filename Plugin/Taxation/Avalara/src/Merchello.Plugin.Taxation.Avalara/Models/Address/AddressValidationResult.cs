namespace Merchello.Plugin.Taxation.Avalara.Models.Address
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an address validation result.
    /// </summary>
    public class AddressValidationResult : IAddressValidationResult
    {
        /// <summary>
        /// Gets or sets the <see cref="IValidatedAddress"/>.
        /// </summary>
        public ValidatedAddress Address { get; set; }

        /// <summary>
        /// Gets or sets the result code indicating varying levels of success or failure in validating the address.
        /// </summary>
        public SeverityLevel ResultCode { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="IApiResponseMessage"/> returned from the API.
        /// </summary>
        public IEnumerable<ApiResponseMessage> Messages { get; set; }
    }
}