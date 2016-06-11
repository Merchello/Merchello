namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Core.Services;

    using Newtonsoft.Json;

    using Umbraco.Core.Logging;

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
                Context.Services.CustomerService.Save(Context.Customer as AnonymousCustomer, Context.Settings.RaiseCustomerEvents);
            }
            else
            {
                ((CustomerService)Context.Services.CustomerService).Save(Context.Customer as Customer, Context.Settings.RaiseCustomerEvents);
            }
        }

        /// <summary>
        /// Saves the offer codes.
        /// </summary>
        /// <param name="key">
        /// The key or alias.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        protected virtual void SaveCustomerTempData(string key, IEnumerable<string> data)
        {
            var json =
                JsonConvert.SerializeObject(
                    new CheckoutCustomerTempData() { Data = data, VersionKey = Context.VersionKey });

            Context.Customer.ExtendedData.SetValue(key, json);

            SaveCustomer();
        }

        /// <summary>
        /// Gets a safe list of customer temp data (asserts the version key).
        /// </summary>
        /// <param name="key">
        /// The key or alias.
        /// </param>
        /// <returns>
        /// The <see cref="List{String}"/>.
        /// </returns>
        protected virtual List<string> BuildVersionedCustomerTempData(string key)
        {
            var data = new List<string>();
            var queueDataJson = Context.Customer.ExtendedData.GetValue(key);
            if (string.IsNullOrEmpty(queueDataJson)) return data;

            try
            {
                var savedData = JsonConvert.DeserializeObject<CheckoutCustomerTempData>(queueDataJson);

                // verify that the offer codes are for this version of the checkout
                if (savedData.VersionKey != Context.VersionKey) return data;

                data.AddRange(savedData.Data);
            }
            catch (Exception ex)
            {
                // don't throw an exception here as the customer is in the middle of a checkout.
                MultiLogHelper.Error<CheckoutCustomerDataManagerBase>("Failed to deserialize CheckoutCustomerTempData.  Returned empty offer code list instead.", ex);
            }

            return data;
        } 

    }
}