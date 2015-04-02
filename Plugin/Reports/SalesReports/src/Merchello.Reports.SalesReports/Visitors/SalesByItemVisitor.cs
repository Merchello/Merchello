namespace Merchello.Reports.SalesReports.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Reports.SalesReports.Models;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Visitors;

    using Umbraco.Core.Logging;

    /// <summary>
    /// The sales by item visitor.
    /// </summary>
    public class SalesByItemVisitor : ILineItemDisplayVisitor
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// The results.
        /// </summary>
        private readonly IDictionary<Guid, SalesByItemResult> _results = new Dictionary<Guid, SalesByItemResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesByItemVisitor"/> class.
        /// </summary>
        public SalesByItemVisitor()
            : this(new MerchelloHelper(MerchelloContext.Current.Services))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesByItemVisitor"/> class.
        /// </summary>
        /// <param name="merchello">
        /// The merchello.
        /// </param>
        /// <exception cref="NullReferenceException">
        /// Throws an exception if the MerchelloHelper is null
        /// </exception>
        public SalesByItemVisitor(MerchelloHelper merchello)
        {
            if (merchello == null) throw  new NullReferenceException("The Merchello helper cannot be null");

            _merchello = merchello;
        }

        /// <summary>
        /// Gets the results.
        /// </summary>
        public IEnumerable<SalesByItemResult> Results
        {
            get
            {
                return _results.Select(x => x.Value).OrderByDescending(x => x.Total);
            }
        }

        /// <summary>
        /// The visit.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(LineItemDisplayBase lineItem)
        {
            if (lineItem.LineItemTfKey != Constants.TypeFieldKeys.LineItem.ProductKey) return;

            try
            {
                var key = lineItem.ExtendedData.FirstOrDefault(x => x.Key == Constants.ExtendedDataKeys.ProductVariantKey);

                Guid productVariantKey;
                if (Guid.TryParse(key.Value, out productVariantKey))
                {
                    if (_results.ContainsKey(productVariantKey))
                    {
                        _results[productVariantKey].Quantity += lineItem.Quantity;
                        _results[productVariantKey].Total += lineItem.Quantity * lineItem.Price;
                        return;
                    }

                    var variant = _merchello.Query.Product.GetProductVariantByKey(productVariantKey);
                    if (variant == null) return;

                    _results.Add(productVariantKey, new SalesByItemResult()
                    {
                        ProductVariant = variant,
                        Quantity = lineItem.Quantity,
                        Total = lineItem.Quantity * lineItem.Price
                    });
                }
            }
            catch (Exception)
            {
                LogHelper.Debug<SalesByItemVisitor>("Could not retrieve product variant key from the extended data collection.  This may be a result of an issue fixed in 1.5.1 or indicate a custom product line item which is not valid for this report.");
            }
        }
    }
}