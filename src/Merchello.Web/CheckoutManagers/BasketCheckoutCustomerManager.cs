namespace Merchello.Web.CheckoutManagers
{
    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Models;

    /// <summary>
    /// The basket checkout customer manager.
    /// </summary>
    internal class BasketCheckoutCustomerManager : CheckoutCustomerManagerBase, ICheckoutCustomerManager 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketCheckoutCustomerManager"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public BasketCheckoutCustomerManager(ICheckoutContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Save the billing address.
        /// </summary>
        /// <param name="billToAddress">
        /// The bill to address.
        /// </param>
        public override void SaveBillToAddress(IAddress billToAddress)
        {
            this.Context.Customer.ExtendedData.AddAddress(billToAddress, AddressType.Billing);
            this.SaveCustomer();
        }

        /// <summary>
        /// Saves the shipping address.
        /// </summary>
        /// <param name="shipToAddress">
        /// The ship to address.
        /// </param>
        public override void SaveShipToAddress(IAddress shipToAddress)
        {
            this.Context.Customer.ExtendedData.AddAddress(shipToAddress, AddressType.Shipping);
            this.SaveCustomer();
        }

        /// <summary>
        /// Gets the billing address.
        /// </summary>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public override IAddress GetBillToAddress()
        {
            return this.Context.Customer.ExtendedData.GetAddress(AddressType.Billing);
        }

        /// <summary>
        /// Gets the shipping address.
        /// </summary>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public override IAddress GetShipToAddress()
        {
            return this.Context.Customer.ExtendedData.GetAddress(AddressType.Shipping);
        }
    }
}