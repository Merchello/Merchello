namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;

    /// <summary>
    /// The checkout model base.
    /// </summary>
    public abstract class CheckoutModelBase : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutModelBase"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        protected CheckoutModelBase(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the sale summary.
        /// </summary>
        public SalePreparationSummary SaleSummary { get; set; }
    }
}