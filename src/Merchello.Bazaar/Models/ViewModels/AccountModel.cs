namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;

    /// <summary>
    /// Represents a customer account.
    /// </summary>
    public class AccountModel : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public AccountModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        public CustomerProfileModel Profile { get; set; }
    }
}