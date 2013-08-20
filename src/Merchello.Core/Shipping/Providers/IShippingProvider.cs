using System;
using Merchello.Core.Models;

namespace Merchello.Core.Shipping.Providers
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
 
    /// <summary>
    /// Interface that will be implemented by shipping providers
    /// </summary>
    public interface IShippingProvider
    {
        Guid ShipGatewayId { get; }
        string Name { get; }
        string Version { get; }
        string Description { get; }
        bool UseDebugMode { get; set; }
        
        void Initialize(Guid shipGatewayId, IDictionary<string, string> configData);

        IDictionary<string, string> GetConfigData();

        ListItem[] GetServiceListItems();
        
        ShipRateQuote GetShipRateQuote(Warehouse origin, Address destination, IList<InvoiceItem> contents,
            string serviceCode);

        ShipRateQuote GetShipRateQuote(IShipment shipment, string serviceCode);

        TrackingSummary GetTrackingSummary(TrackingNumber trackingNumber);
    }
}