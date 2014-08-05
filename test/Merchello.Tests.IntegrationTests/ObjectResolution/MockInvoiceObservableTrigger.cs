using System;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Observation;
using Merchello.Core.Services;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    //[TriggerFor("2DA5CE92-E388-4788-A647-CDEA82EE6C9F", "Mock Trigger", "Testing", typeof(InvoiceService), "Creating")]
    //internal class MockInvoiceObservableTrigger : ObservableTriggerBase
    //{
    //    public static bool EventInvoked = false;

    //    public override void Invoke(object sender, EventArgs e)
    //    {
    //        Console.WriteLine(sender.GetType());

    //        var invoice = ((NewEventArgs<IInvoice>) e).Entity;

    //        Console.WriteLine(invoice.GetType());

    //        EventInvoked = true;
    //    }
    //}
}