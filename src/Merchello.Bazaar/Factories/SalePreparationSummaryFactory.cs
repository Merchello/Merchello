namespace Merchello.Bazaar.Factories
{
    using System.Linq;

    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core;

    /// <summary>
    /// A factory responsible for building the <see cref="SalePreparationSummary"/>.
    /// </summary>
    internal class SalePreparationSummaryFactory
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
        /// Initializes a new instance of the <see cref="SalePreparationSummaryFactory"/> class.
        /// </summary>
        /// <param name="currency">
        /// The <see cref="ICurrency"/>.
        /// </param>
        /// <param name="basketLineItemFactory">
        /// The basket Line Item Factory.
        /// </param>
        public SalePreparationSummaryFactory(ICurrency currency, BasketLineItemFactory basketLineItemFactory)
        {
            Mandate.ParameterNotNull(currency, "currency");
            Mandate.ParameterNotNull(basketLineItemFactory, "basketLineItemFactory");

            _currency = currency;
            _basketLineItemFactory = basketLineItemFactory;
        }

        /// <summary>
        /// Builds a <see cref="SalePreparationSummary"/>
        /// </summary>
        /// <param name="preparation">
        /// The preparation.
        /// </param>
        /// <returns>
        /// The <see cref="SalePreparationSummary"/>.
        /// </returns>
        public SalePreparationSummary Build(SalePreparationBase preparation)
        {
            if (preparation.IsReadyToInvoice())
            {
                var invoice = preparation.PrepareInvoice();
                return new SalePreparationSummary()
                           {
                               Currency = _currency,
                               Items = invoice.Items.Select(x => _basketLineItemFactory.Build(x)),
                               ItemTotal = invoice.TotalItemPrice(),
                               ShippingTotal = invoice.TotalShipping(),
                               TaxTotal = invoice.TotalTax(),
                               DiscountsTotal = invoice.TotalDiscounts(),
                               InvoiceTotal = invoice.Total
                           };
            }
            
            return new SalePreparationSummary()
                {
                    Currency = _currency,
                    Items = preparation.ItemCache.Items.Select(x => _basketLineItemFactory.Build(x)),
                    ItemTotal = preparation.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Product).Sum(x => x.TotalPrice),
                    ShippingTotal = preparation.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Shipping).Sum(x => x.TotalPrice),
                    TaxTotal = preparation.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Tax).Sum(x => x.TotalPrice),
                    DiscountsTotal = preparation.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => x.TotalPrice),
                    InvoiceTotal = preparation.ItemCache.Items.Where(x => x.LineItemType != LineItemType.Discount).Sum(x => x.TotalPrice) - preparation.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => x.TotalPrice)
                };
        }
    }
}