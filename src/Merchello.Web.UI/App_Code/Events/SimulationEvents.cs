using System.Linq;

using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Sales;

using Umbraco.Core;

/// <summary>
/// The simulation events.
/// </summary>
public class SimulationEvents : ApplicationEventHandler
{
    /// <summary>
    /// Overrides for Umbraco application starting.
    /// </summary>
    /// <param name="umbracoApplication">
    /// The Umbraco application.
    /// </param>
    /// <param name="applicationContext">
    /// The application context.
    /// </param>
    protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
    {
        SalePreparationBase.InvoicePrepared += SalePreparationBaseOnInvoicePrepared;
    }

    private void SalePreparationBaseOnInvoicePrepared(SalePreparationBase sender, SalesPreparationEventArgs<IInvoice> e)
    {

        // custom line items
        var ed1 = new ExtendedDataCollection();
        ed1.SetValue(Merchello.Core.Constants.ExtendedDataKeys.Taxable, false.ToString());
        ed1.SetValue(Merchello.Core.Constants.ExtendedDataKeys.Shippable, false.ToString());

        var typeField = EnumTypeFieldConverter.LineItemType.Custom("CcFee");

        //// Act
        var ccFee = new InvoiceLineItem(
            typeField.TypeKey,
            "CC Fee",
            "ccfee",
            1,
            1.0m,
            ed1);

        e.Entity.Items.Add(ccFee);
        e.Entity.Total += 1.0m;

        var ed2 = new ExtendedDataCollection();
        var shippingLineItem = e.Entity.ShippingLineItems().FirstOrDefault();
        if (shippingLineItem != null)
        {
            //// 
            if (shippingLineItem.Price >= 30)
            {
                var shipping10Off = new InvoiceLineItem(
                    EnumTypeFieldConverter.LineItemType.Discount.TypeKey,
                    "10 Off Shipping", 
                    "BreakOnShipping", 
                    1, 
                    10M, 
                    ed2);
                e.Entity.Items.Add(shipping10Off);
                e.Entity.Total -= 10M;
            }
        }         
    }
}
