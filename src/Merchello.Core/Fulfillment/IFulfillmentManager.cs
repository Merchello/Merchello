using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;

namespace Merchello.Core.Fulfillment
{
    internal interface IFulfillmentManager // should simply use the WarehouseService for all of this
    {
        // business logic
        IOrder PrepareOrderForShipping(IOrder order);
        IEnumerable<IShipment> ShipOrder(IOrder order);

        IPaymentResult ProcessPayment(IInvoice invoice, IPaymentGatewayMethod paymentGatewayMethod, decimal amount, ProcessorArgumentCollection args);
        void CancelOrder(IOrder order);
        void VoidInvoice(IInvoice invoice);
        void ApplyPayment(IInvoice invoice);

        IEnumerable<IOrder> GetUnshippedOrders();
        IEnumerable<IOrder> GetUnshippedOrders(Guid warehouseKey);
        IEnumerable<IOrder> GetBackOrders();
        IEnumerable<IOrder> GetBackOrders(Guid warehouseKey);

        void Save(IOrder order);
        void Save(IEnumerable<IOrder> orders);
        void Delete(IOrder order);
        void Delete(IEnumerable<IOrder> orders);
        IEnumerable<IShipment> GetUnshippedShipments();
        IEnumerable<IShipment> GetUnshippedShipments(Guid warehouseKey);
        IEnumerable<IShipment> GetShipmentsByOrderKey(Guid orderKey);
        IEnumerable<IShipment> GetShipmentsByTrackingCode(string trackingCode);
        void Save(IShipment shipment);
        void Save(IEnumerable<IShipment> shipments);
        void Delete(IShipment shipment);
        void Delete(IEnumerable<IShipment> shipments);
    }
}