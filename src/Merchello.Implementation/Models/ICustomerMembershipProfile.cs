namespace Merchello.Implementation.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines an Umbraco Membership Profile.
    /// </summary>
    public interface ICustomerMembershipProfile
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <remarks>
        /// This is the login / username
        /// </remarks>
        [Required]
        [EmailAddress]
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the Umbraco member first name.
        /// </summary>
        [Required]
        string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Umbraco member last name.
        /// </summary>
        [Required, Display(Name = "Last Name *")]
        string LastName { get; set; }
    }
}