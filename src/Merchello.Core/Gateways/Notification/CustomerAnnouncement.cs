using Merchello.Core.Events;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Umbraco.Core.Events;

namespace Merchello.Core.Gateways.Notification
{
    internal class CustomerAnnouncement
    {

        public void InvoicedCustomer(ICustomerBase customer, IInvoice invoice)
        {
            Invoiced.RaiseEvent(new CustomerAnnouncementEventArgs<IInvoice>(customer, invoice), this);
        }

        public void InvoiceWasCanceled(ICustomerBase customer, IInvoice invoice)
        {
            InvoiceCanceled.RaiseEvent(new CustomerAnnouncementEventArgs<IInvoice>(customer, invoice), this);
        }

        public void PaymentWasAuthorized(ICustomerBase customer, IPaymentResult result)
        {
            PaymentAuthorized.RaiseEvent(new CustomerAnnouncementEventArgs<IPaymentResult>(customer, result), this);
        }

        public void PaymentWasCaptured(ICustomerBase customer, IPaymentResult result)
        {
            PaymentCaptured.RaiseEvent(new CustomerAnnouncementEventArgs<IPaymentResult>(customer, result), this);
        }

        public void PaymentWasVoided(ICustomerBase customer, IPaymentResult result)
        {
            PaymentVoided.RaiseEvent(new CustomerAnnouncementEventArgs<IPaymentResult>(customer, result), this);
        }

        public void PaymentProblem(ICustomerBase customer, IPaymentResult result)
        {
            PaymentFailed.RaiseEvent(new CustomerAnnouncementEventArgs<IPaymentResult>(customer, result), this);
        }

        public void OrderWasPlaced(ICustomerBase customer, IOrder order)
        {
            Ordered.RaiseEvent(new CustomerAnnouncementEventArgs<IOrder>(customer, order), this);
        }

        public void OrderWasBackOrdered(ICustomerBase customer, IOrder order)
        {
            OrderBackOrdered.RaiseEvent(new CustomerAnnouncementEventArgs<IOrder>(customer, order), this);
        }

        public void OrderWasCancelled(ICustomerBase customer, IOrder order)
        {
            OrderCancelled.RaiseEvent(new CustomerAnnouncementEventArgs<IOrder>(customer, order), this);
        }

        public void ShipmentWasPackaged(ICustomerBase customer, IShipment shipment)
        {
            ShipmentPackaged.RaiseEvent(new CustomerAnnouncementEventArgs<IShipment>(customer, shipment), this);
        }

        public void ShipmentWasShipped(ICustomerBase customer, IShipment shipment)
        {
            ShipmentShipped.RaiseEvent(new CustomerAnnouncementEventArgs<IShipment>(customer, shipment), this);
        }

#region Events
        
        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IInvoice>> Invoiced;

        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IInvoice>> InvoiceCanceled;

        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IPaymentResult>> PaymentAuthorized;

        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IPaymentResult>> PaymentCaptured;

        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IPaymentResult>> PaymentVoided;

        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IPaymentResult>> PaymentFailed;

        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IOrder>> Ordered;

        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IOrder>> OrderBackOrdered;

        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IOrder>> OrderCancelled;

        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IShipment>> ShipmentPackaged;
                
        public static event TypedEventHandler<CustomerAnnouncement, Events.CustomerAnnouncementEventArgs<IShipment>> ShipmentShipped;

        #endregion


    }
}