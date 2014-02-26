using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Sales;
using Umbraco.Core;

namespace Merchello.Core.Chains.InvoiceCreation
{
    internal class ApplyTaxesToInvoiceTax : OrderPreparationAttemptChainTaskBase
    {
        public ApplyTaxesToInvoiceTax(SalePreparationBase salePreparation) 
            : base(salePreparation)
        { }

        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            // if taxes are not to be applied, skip this step
            if (SalePreparation.ApplyTaxesToInvoice)
            {
                try
                {
                    // clear any current tax lines
                    var removers = value.Items.Where(x => x.LineItemType == LineItemType.Tax);
                    foreach (var remove in removers)
                    {
                        value.Items.Remove(remove);
                    }

                    var taxes = value.CalculateTaxes(SalePreparation.MerchelloContext, value.GetBillingAddress());

                    value.Items.Add(taxes.AsLineItemOf<InvoiceLineItem>());

                    return Attempt<IInvoice>.Succeed(value);
                }
                catch (Exception ex)
                {
                    return Attempt<IInvoice>.Fail(ex);
                }
            }

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}