namespace Merchello.Bazaar.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The customer login model.
    /// </summary>
    public partial class CustomerLoginModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required, Display(Name = "Email")]
        [EmailAddress]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required, Display(Name = "Password"), DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember me.
        /// </summary>
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
        
    }
}