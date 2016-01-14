namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;

    /// <summary>
    /// Represents the Basket page model.
    /// </summary>
    public partial class BasketModel : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public BasketModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the basket table.
        /// </summary>
        public BasketTableModel BasketTable { get; set; }
    }
}