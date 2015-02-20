namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;

    /// <summary>
    /// The account history model.
    /// </summary>
    public class AccountHistoryModel : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountHistoryModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public AccountHistoryModel(IPublishedContent content)
            : base(content)
        {
        }
    }
}