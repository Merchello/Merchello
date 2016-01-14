namespace Merchello.Bazaar.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The customer profile model.
    /// </summary>
    public partial class CustomerProfileModel
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
    }
}