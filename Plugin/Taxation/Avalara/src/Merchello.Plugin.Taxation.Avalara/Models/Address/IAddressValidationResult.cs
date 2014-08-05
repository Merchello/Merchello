namespace Merchello.Plugin.Taxation.Avalara.Models.Address
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the validated tax address.
    /// </summary>
    public interface IAddressValidationResult
    {
        /// <summary>
        /// Gets or sets the <see cref="IValidatedAddress"/>.
        /// </summary>
        ValidatedAddress Address { get; set; }

        /// <summary>
        /// Gets or sets the result code indicating varying levels of success or failure in validating the address.
        /// </summary>
        SeverityLevel ResultCode { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="IApiResponseMessage"/> returned from the API.
        /// </summary>
        IEnumerable<ApiResponseMessage> Messages { get; set; } 
    }
}