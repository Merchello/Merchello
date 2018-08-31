namespace Merchello.Core.Chains.InvoiceCreation.CheckoutManager
{
    using System.IO;
    using System.Linq;

    using Merchello.Core.Checkout;
    using Merchello.Core.Models;

    using Umbraco.Core;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// The validate common currency.
    /// </summary>
    internal class ValidateCommonCurrency : CheckoutManagerInvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateCommonCurrency"/> class.
        /// </summary>
        /// <param name="checkoutManager">
        /// The <see cref="ICheckoutManagerBase"/>.
        /// </param>
        public ValidateCommonCurrency(ICheckoutManagerBase checkoutManager)
            : base(checkoutManager)
        {
        }

        /// <summary>
        /// Performs the task of asserting everything is billed in a common currency.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            var unTagged = value.Items.Where(x => !x.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.CurrencyCode)).ToArray();

            if (unTagged.Any())
            {
                string defaultCurrencyCode =
                    this.CheckoutManager.Context.Services.StoreSettingService.GetByKey(
                        Constants.StoreSetting.CurrencyCodeKey).Value;
				
				ICustomer customer = value.Customer();
				if (customer != null && !customer.PriceGroup.IsEmpty)
					defaultCurrencyCode = customer.PriceGroup.Currency;

				foreach (var item in unTagged)
                {
                    item.ExtendedData.SetValue(Constants.ExtendedDataKeys.CurrencyCode, defaultCurrencyCode);
                }
            }

            var allCurrencyCodes =
                value.Items.Select(x => x.ExtendedData.GetValue(Constants.ExtendedDataKeys.CurrencyCode)).Distinct().ToArray();

            //// Assign the currency code on the invoice
            if (allCurrencyCodes.Length == 1) value.CurrencyCode = allCurrencyCodes.First();

            return 1 == allCurrencyCodes.Length
                       ? Attempt<IInvoice>.Succeed(value)
                       : Attempt<IInvoice>.Fail(new InvalidDataException("Invoice is being created with line items costed in different currencies."));

        }
    }
}