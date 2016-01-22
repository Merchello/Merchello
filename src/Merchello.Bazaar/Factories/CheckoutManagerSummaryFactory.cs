namespace Merchello.Bazaar.Factories
{
    using System.Linq;

    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Core.Checkout;

    using Umbraco.Core;

    /// <summary>
    /// A factory responsible for building the <see cref="SalePreparationSummary"/>.
    /// </summary>
    internal class CheckoutManagerSummaryFactory
    {
        /// <summary>
        /// The <see cref="ICurrency"/>.
        /// </summary>
        private readonly ICurrency _currency;

        /// <summary>
        /// The _basket line item factory.
        /// </summary>
        private readonly BasketLineItemFactory _basketLineItemFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutManagerSummaryFactory"/> class.
        /// </summary>
        /// <param name="currency">
        /// The <see cref="ICurrency"/>.
        /// </param>
        /// <param name="basketLineItemFactory">
        /// The basket Line Item Factory.
        /// </param>
        public CheckoutManagerSummaryFactory(ICurrency currency, BasketLineItemFactory basketLineItemFactory)
        {
            Mandate.ParameterNotNull(currency, "currency");
            Mandate.ParameterNotNull(basketLineItemFactory, "basketLineItemFactory");

            _currency = currency;
            _basketLineItemFactory = basketLineItemFactory;
        }

        /// <summary>
        /// Builds a <see cref="SalePreparationSummary"/>
        /// </summary>
        /// <param name="manager">
        /// The preparation.
        /// </param>
        /// <returns>
        /// The <see cref="SalePreparationSummary"/>.
        /// </returns>
        public SalePreparationSummary Build(ICheckoutManagerBase manager)
        {

            if (manager.Payment.IsReadyToInvoice())
            {
                var invoice = manager.Payment.PrepareInvoice();
                return new SalePreparationSummary()
                           {
                               TotalLabel = "Total",
                               Currency = _currency,
                               Items = invoice.Items.Where(x => x.LineItemType == LineItemType.Product).Select(x => _basketLineItemFactory.Build(x)),
                               ItemTotal = ModelExtensions.FormatPrice(invoice.TotalItemPrice(), _currency),
                               ShippingTotal = ModelExtensions.FormatPrice(invoice.TotalShipping(), _currency),
                               TaxTotal = ModelExtensions.FormatPrice(invoice.TotalTax(), _currency),
                               DiscountsTotal = ModelExtensions.FormatPrice(invoice.TotalDiscounts(), _currency),
                               InvoiceTotal = ModelExtensions.FormatPrice(invoice.Total, _currency),                               
                           };
            }
            
            return new SalePreparationSummary()
                {
                    TotalLabel = "Sub Total",
                    Currency = _currency,
                    Items = manager.Context.ItemCache.Items.Select(x => _basketLineItemFactory.Build(x)),
                    ItemTotal = ModelExtensions.FormatPrice(manager.Context.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Product).Sum(x => x.TotalPrice), _currency),
                    ShippingTotal = ModelExtensions.FormatPrice(manager.Context.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Shipping).Sum(x => x.TotalPrice), _currency),
                    TaxTotal = ModelExtensions.FormatPrice(manager.Context.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Tax).Sum(x => x.TotalPrice), _currency),
                    DiscountsTotal = ModelExtensions.FormatPrice(manager.Context.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => x.TotalPrice), _currency),
                    InvoiceTotal = ModelExtensions.FormatPrice(manager.Context.ItemCache.Items.Where(x => x.LineItemType != LineItemType.Discount).Sum(x => x.TotalPrice) - manager.Context.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => x.TotalPrice), _currency),
                };
        }
    }
}