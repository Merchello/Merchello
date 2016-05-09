namespace Merchello.QuickMart.Models
{
    using System.ComponentModel.DataAnnotations;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// The checkout billing address model.
    /// </summary>
    public class CheckoutBillingAddressModel : CheckoutAddressModel, ICustomerMembershipProfile
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public new string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use billing address for the shipping address.
        /// </summary>
        public bool UseForShipping { get; set; }
    }
}