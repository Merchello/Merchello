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

        var extendedData = new ExtendedDataCollection();
        extendedData.SetValue(Merchello.Core.Constants.ExtendedDataKeys.Taxable, false.ToString());

        var typeField = EnumTypeFieldConverter.LineItemType.Custom("CcFee");

        //// Act
        var ccFee = new InvoiceLineItem(
            typeField.TypeKey,
            "CC Fee",
            "ccfee",
            1,
            1.0m,
            extendedData);

        e.Entity.Items.Add(ccFee);
        e.Entity.Total += 1.0m;
    }
}
