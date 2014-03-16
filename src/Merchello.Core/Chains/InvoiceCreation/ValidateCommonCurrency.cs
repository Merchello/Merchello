using System.IO;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Sales;
using Umbraco.Core;

namespace Merchello.Core.Chains.InvoiceCreation
{
    /// <summary>
    /// Validates that all line items are costed in the same currency.  If a currency has not been set
    /// the line item is tagged with the default currency from Store Settings
    /// </summary>
    internal class ValidateCommonCurrency : InvoiceCreationAttemptChainTaskBase
    {
        public ValidateCommonCurrency(SalePreparationBase salePreparation) 
            : base(salePreparation)
        { }

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