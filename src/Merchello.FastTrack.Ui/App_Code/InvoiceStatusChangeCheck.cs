using Umbraco.Core;

namespace Merchello.FastTrack.Tests
{
    using Merchello.Core.Services;

    using Umbraco.Core.Logging;

    public class InvoiceStatusChangeCheck : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            InvoiceService.StatusChanging += InvoiceService_StatusChanging;
            InvoiceService.StatusChanged += InvoiceService_StatusChanged;
        }

        private void InvoiceService_StatusChanging(IInvoiceService sender, Core.Events.StatusChangeEventArgs<Core.Models.IInvoice> e)
        {
            LogHelper.Info<InvoiceStatusChangeCheck>("Invoice status changing");
        }

        private void InvoiceService_StatusChanged(IInvoiceService sender, Core.Events.StatusChangeEventArgs<Core.Models.IInvoice> e)
        {
            LogHelper.Info<InvoiceStatusChangeCheck>("Invoice status changed");
        }
    }
}