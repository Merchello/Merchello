namespace Merchello.Core.Chains.InvoiceCreation
{
    using System.IO;
    using System.Linq;
    using Models;
    using Sales;
    using Umbraco.Core;
    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Validates that all line items are costed in the same currency.  If a currency has not been set
    /// the line item is tagged with the default currency from Store Settings
    /// </summary>
    internal class ValidateCommonCurrency : InvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateCommonCurrency"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The sale preparation.
        /// </param>
        public ValidateCommonCurrency(SalePreparationBase salePreparation)
            : base(salePreparation)
        {            
        }

        /// <summary>
        /// The perform task.
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
                var defaultCurrency =
                    SalePreparation.MerchelloContext.Services.StoreSettingService.GetByKey(
                        Constants.StoreSettingKeys.CurrencyCodeKey);

                foreach (var item in unTagged)
                {
                    item.ExtendedData.SetValue(Constants.ExtendedDataKeys.CurrencyCode, defaultCurrency.Value);
                }
            }

            var allCurrencyCodes =
                value.Items.Select(x => x.ExtendedData.GetValue(Constants.ExtendedDataKeys.CurrencyCode)).Distinct();

            return 1 == allCurrencyCodes.Count()
                       ? Attempt<IInvoice>.Succeed(value)
                       : Attempt<IInvoice>.Fail(new InvalidDataException("Invoice is being created with line items costed in different currencies."));

        }
    }
}