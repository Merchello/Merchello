namespace Merchello.Web.Factories
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Workflow;

    /// <summary>
    /// A base factory responsible for building <see cref="ICheckoutSummaryModel{TBillingAddress, TShippingAddress, TLineItem}"/>.
    /// </summary>
    /// <typeparam name="TSummary">
    /// The type of <see cref="ICheckoutSummaryModel{TBillingAddress, TShippingAddress, TLineItem}"/>
    /// </typeparam>
    /// <typeparam name="TBillingAddress">
    /// The type of the <see cref="ICheckoutAddressModel"/> used for billing
    /// </typeparam>
    /// <typeparam name="TShippingAddress">
    /// The type of the <see cref="ICheckoutAddressModel"/> used for shipping
    /// </typeparam>
    /// <typeparam name="TLineItem">
    /// The type of the <see cref="ILineItemModel"/> used in the summary
    /// </typeparam>
    public class CheckoutSummaryModelFactory<TSummary, TBillingAddress, TShippingAddress, TLineItem>
        where TBillingAddress : class, ICheckoutAddressModel, new()
        where TShippingAddress : class, ICheckoutAddressModel, new()
        where TLineItem : class, ILineItemModel, new()
        where TSummary : class, ICheckoutSummaryModel<TBillingAddress, TShippingAddress, TLineItem>, new()
    {
        /// <summary>
        /// Creates a <see cref="ICheckoutSummaryModel{TBillingAddress, TShippingAddress, TLineItem}"/>.
        /// </summary>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <param name="checkoutManager">
        /// The <see cref="ICheckoutManagerBase"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutSummaryModel{TBillingAddress, TShippingAddress, TLineItem}"/>.
        /// </returns>
        public TSummary Create(IBasket basket, ICheckoutManagerBase checkoutManager)
        {
            var billing = checkoutManager.Customer.GetBillToAddress();
            var shipping = checkoutManager.Customer.GetShipToAddress();

            return new TSummary
                {
                    BillingAddress = Create<TBillingAddress>(billing ?? new Address { AddressType = AddressType.Billing }),
                    ShippingAddress = Create<TShippingAddress>(shipping ?? new Address { AddressType = AddressType.Shipping }),
                    Items = basket.Items.Select(Create)
                };
        }

        /// <summary>
        /// Creates a <see cref="ICheckoutAddressModel"/> from <see cref="IAddress"/>.
        /// </summary>
        /// <param name="adr">
        /// The <see cref="IAddress"/>.
        /// </param>
        /// <typeparam name="TAddress">
        /// The type of <see cref="ICheckoutAddressModel"/>
        /// </typeparam>
        /// <returns>
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </returns>
        protected TAddress Create<TAddress>(IAddress adr) 
            where TAddress : class, ICheckoutAddressModel, new()
        {
            var first = adr.TrySplitFirstName();
            var address = new TAddress
            {
                Name = adr.Name, 
                Organization = adr.Organization,
                Address1 = adr.Address1,
                Address2 = adr.Address2,
                Locality = adr.Locality,
                Region = adr.Region,
                PostalCode = adr.PostalCode,
                CountryCode = adr.CountryCode,
                IsCommercial = adr.IsCommercial,
                Phone = adr.Phone,
                Email = adr.Email,
                AddressType = adr.AddressType 
            };

            return OnCreate(address, adr);
        }

        /// <summary>
        /// Creates a <see cref="ILineItemModel"/> from <see cref="ILineItem"/>.
        /// </summary>
        /// <param name="item">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemModel"/>.
        /// </returns>
        protected TLineItem Create(ILineItem item)
        {
            var lineItem = new TLineItem
                    {
                        Key = item.Key, 
                        Name = item.Name,
                        Sku = item.Sku,
                        Quantity = item.Quantity,
                        Amount = item.Price,
                        LineItemType = item.LineItemType
                    };

            return OnCreate(lineItem, item);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ILineItemModel"/> from <see cref="IAddress"/>.
        /// </summary>
        /// <param name="lineItem">
        /// The <see cref="ILineItemModel"/>.
        /// </param>
        /// <param name="item">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ILineItemModel"/>.
        /// </returns>
        protected virtual TLineItem OnCreate(TLineItem lineItem, ILineItem item)
        {
            return lineItem;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ICheckoutAddressModel"/> from <see cref="IAddress"/>.
        /// </summary>
        /// <param name="address">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <param name="adr">
        /// The <see cref="IAddress"/>.
        /// </param>
        /// <typeparam name="TAddress">
        /// The type of the <see cref="ICheckoutAddressModel"/>
        /// </typeparam>
        /// <returns>
        /// The modified <see cref="ICheckoutAddressModel"/>.
        /// </returns>
        protected virtual TAddress OnCreate<TAddress>(TAddress address, IAddress adr)
        {
            return address;
        }
    }
}