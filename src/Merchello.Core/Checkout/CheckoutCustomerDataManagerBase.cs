namespace Merchello.Core.Checkout
{
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    /// <summary>
    /// A checkout manager base class for saving customer data.
    /// </summary>
    public abstract class CheckoutCustomerDataManagerBase : CheckoutContextManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutCustomerDataManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected CheckoutCustomerDataManagerBase(ICheckoutContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Saves the customer.
        /// </summary>
        protected virtual void SaveCustomer()
        {
            if (Context.Customer.IsAnonymous)
            {
                Context.Services.CustomerService.Save(Context.Customer as AnonymousCustomer, Context.RaiseCustomerEvents);
            }
            else
            {
                ((CustomerService)Context.Services.CustomerService).Save(Context.Customer as Customer, Context.RaiseCustomerEvents);
            }
        }
    }
}