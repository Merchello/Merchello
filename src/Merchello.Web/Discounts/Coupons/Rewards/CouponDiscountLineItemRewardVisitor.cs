namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Models;
    using Merchello.Web.DataModifiers.Product;

    /// <summary>
    /// The coupon discount line item reward visitor.
    /// </summary>
    internal class CouponDiscountLineItemRewardVisitor : ILineItemVisitor
    {
        #region Private Fields

        /// <summary>
        /// The audits.
        /// </summary>
        private readonly List<CouponRewardAdjustmentAudit> _audits = new List<CouponRewardAdjustmentAudit>();

        /// <summary>
        /// The amount.
        /// </summary>
        private readonly decimal _amount;

        /// <summary>
        /// The adjustment type.
        /// </summary>
        private readonly CouponDiscountLineItemReward.Adjustment _adjustmentType;

        /// <summary>
        /// A value indicating whether or not the taxation context is ready to handle product based taxation.
        /// </summary>
        private bool _productTaxationEnabled = false;

        /// <summary>
        /// The <see cref="ITaxationContext"/>.
        /// </summary>
        private ITaxationContext _taxationContext;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private MerchelloHelper _merchello;

        /// <summary>
        /// The qualifying total.
        /// </summary>
        private decimal _qualifyingTotal = 0M;

        /// <summary>
        /// The adjusted product total.
        /// </summary>
        private decimal _adjustedProductPreTaxTotal = 0M;

        /// <summary>
        /// The adjusted tax total.
        /// </summary>
        private decimal _adjustedTaxTotal = 0M;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponDiscountLineItemRewardVisitor"/> class.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="adjustmentType">
        /// The adjustment type.
        /// </param>
        public CouponDiscountLineItemRewardVisitor(decimal amount, CouponDiscountLineItemReward.Adjustment adjustmentType)
        {
            if (MerchelloContext.Current == null) throw new NullReferenceException("MerchelloContext was null");
            this._amount = amount;
            this._adjustmentType = adjustmentType;
            this.Initialize();
        }

        #region Properties

        /// <summary>
        /// Gets the audits.
        /// </summary>
        public IEnumerable<CouponRewardAdjustmentAudit> Audits
        {
            get
            {
                return _audits;
            }
        }

        /// <summary>
        /// Gets the qualifying total.
        /// </summary>
        public decimal QualifyingTotal
        {
            get
            {
                return _qualifyingTotal;
            }
        }

        /// <summary>
        /// Gets the adjusted product pre tax total.
        /// </summary>
        public decimal AdjustedProductPreTaxTotal
        {
            get
            {
                return _adjustedProductPreTaxTotal;
            }
        }

        /// <summary>
        /// Gets the adjusted tax total.
        /// </summary>
        public decimal AdjustedTaxTotal
        {
            get
            {
                return _adjustedTaxTotal;
            }
        }

        /// <summary>
        /// Gets the percent discount.
        /// </summary>
        private decimal DiscountPercent
        {
            get
            {
                return _amount / 100;
            }
        }

        #endregion

        /// <summary>
        /// Visits each qualifying line item and applies the discount.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(ILineItem lineItem)
        {
            // handle the special case for percent deduction based when tax is included in the product price
            var audit = new CouponRewardAdjustmentAudit() { RelatesToSku = lineItem.Sku };

            if (lineItem.ExtendedData.TaxIncludedInProductPrice() && _productTaxationEnabled
                && lineItem.ExtendedData.DefinesProductVariant()
                && _adjustmentType == CouponDiscountLineItemReward.Adjustment.Percent)
            {
                var product = _merchello.Query.Product.GetByKey(lineItem.ExtendedData.GetProductKey());
                var preTaxPrice = product.Price - (product.Price * DiscountPercent);
                var preTaxAmount = lineItem.ExtendedData.ProductPriceTaxAmount() - (lineItem.ExtendedData.ProductPriceTaxAmount() * DiscountPercent);
                //// this is sort of weird here, but the line item price may have been set outside the typical workflow
                //// and we cannot rely on the OnSale flag being set so we set both prices to the value set in the line item 
                //// and then apply the taxes to the new price.  Example of this would be using different currencies                                
                product.Price = lineItem.Price;
                
                // this will be the amount of the discount with the taxation break out from the taxation results
                product.SalePrice = preTaxPrice;
                var result = _taxationContext.CalculateTaxesForProduct(product);
                product.AlterProduct(result);



                audit.Log = product.Price == lineItem.Price ? 
                    product.ModifiedDataLogs :
                    product.ModifiedDataLogs.Where(x => x.PropertyName == "SalePrice").ToArray();
                _audits.Add(audit);


                // if taxes are to be excluded the constraint ExcludeTaxesIncludedInProductPrices should be added to remove them
               _qualifyingTotal += lineItem.Price * lineItem.Quantity;
                _adjustedProductPreTaxTotal += preTaxPrice * lineItem.Quantity;
                _adjustedTaxTotal += preTaxAmount * lineItem.Quantity;
            }
            else if (_adjustmentType == CouponDiscountLineItemReward.Adjustment.Percent)
            {
                var modifiedPrice = lineItem.Price - (lineItem.Price * DiscountPercent);
                audit.Log = new[]
                {
                    new DataModifierLog()
                        {
                            PropertyName = "Price",
                            OriginalValue = lineItem.Price,
                            ModifiedValue = modifiedPrice
                        }
                };
                _audits.Add(audit);
                _qualifyingTotal += lineItem.Price * lineItem.Quantity;
            }
            else
            {
                _qualifyingTotal += lineItem.Price * lineItem.Quantity;
            }
        }

        /// <summary>
        /// Initializes the visitor.
        /// </summary>
        private void Initialize()
        {
            _taxationContext = MerchelloContext.Current.Gateways.Taxation;
            _productTaxationEnabled = _taxationContext.ProductPricingEnabled;

            // we do not want to modify data here
            _merchello = new MerchelloHelper(false);
        }
    }
}