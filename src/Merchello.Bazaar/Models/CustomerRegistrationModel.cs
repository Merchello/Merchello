namespace Merchello.Bazaar.Models
{
    using System.ComponentModel.DataAnnotations;

    using Merchello.Bazaar.Models.ViewModels;

    /// <summary>
    /// A model to store Member Login Information.
    /// </summary>
    /// <remarks>
    /// Membership information gets copied to the Merchello Customer by a series of events
    /// </remarks>
    public partial class CustomerRegistrationModel
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Required(ErrorMessage = "First name is required."), Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required(ErrorMessage = "Last name is required."), Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required(ErrorMessage = "A valid email address is required."), Display(Name = "Email Address")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        [Required(ErrorMessage = "Password is required"), Display(Name = "Password"), DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        [Required, Display(Name = "Confirm Password"), DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the member type name.
        /// </summary>
        public string MemberTypeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to persist login after registration.
        /// </summary>
        public bool PersistLogin { get; set; }
    }
}