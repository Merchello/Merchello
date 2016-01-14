namespace Merchello.Bazaar.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The account profile model.
    /// </summary>
    public partial class AccountProfileModel
    {
        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Gets or sets the account page id.
        /// </summary>
        public int AccountPageId { get; set; }

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
        /// Gets or sets a value indicating whether set the password.
        /// </summary>
        public bool SetPassword { get; set; }

        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        [Required(ErrorMessage = "Old Password is required"), Display(Name = "Current Password"), DataType(DataType.Password)]
        public string OldPassword { get; set; }

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
       
    }
}
