namespace Merchello.Bazaar.Models.ViewModels
{
    using Merchello.Bazaar.Models.Account;

    using Umbraco.Core.Models;

    /// <summary>
    /// Represents a customer account.
    /// </summary>
    public partial class AccountModel : MasterModel
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

        /// <summary>
        /// Gets or sets the account profile model.
        /// </summary>
        public AccountProfileModel AccountProfileModel { get; set; }

        /// <summary>
        /// Gets or sets the customer address model.
        /// </summary>
        public CustomerAddressModel CustomerAddressModel { get; set; }
    }
}