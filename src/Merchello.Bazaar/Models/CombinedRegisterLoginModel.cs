namespace Merchello.Bazaar.Models
{
    /// <summary>
    /// The combined register login model.
    /// </summary>
    public partial class CombinedRegisterLoginModel
    {
        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        public CustomerLoginModel Login { get; set; }

        /// <summary>
        /// Gets or sets the registration.
        /// </summary>
        public CustomerRegistrationModel Registration { get; set; }

        /// <summary>
        /// Gets or sets the account page.
        /// </summary>
        public int AccountPageId { get; set; }
    }
}