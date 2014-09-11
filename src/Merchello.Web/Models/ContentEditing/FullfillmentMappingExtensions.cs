using System;
using System.Linq;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Gateways.Shipping;
using Merchello.Web.Models.Payments;

namespace Merchello.Web.Models.ContentEditing
{
	internal static class FullfillmentMappingExtensions
	{

		#region Address

		internal static AddressDisplay ToAddressDisplay(this IAddress address)
		{
			return AutoMapper.Mapper.Map<AddressDisplay>(address);
		}

		internal static IAddress ToAddress(this AddressDisplay addressDisplay)
		{
			return AutoMapper.Mapper.Map<Address>(addressDisplay);
		}

		#endregion

		#region AppliedPayment

		internal static AppliedPaymentDisplay ToAppliedPaymentDisplay(this IAppliedPayment appliedPayment)
		{
			return AutoMapper.Mapper.Map<AppliedPaymentDisplay>(appliedPayment);
		}

		internal static IAppliedPayment ToAppliedPayment(this AppliedPaymentDisplay appliedPaymentDisplay, IAppliedPayment destination)
		{
			if (appliedPaymentDisplay.Key != Guid.Empty) destination.Key = appliedPaymentDisplay.Key;

			// the only things we can change here are the amount and the description
			destination.Description = appliedPaymentDisplay.Description;
			destination.Amount = appliedPaymentDisplay.Amount;

			return destination;
		}

		#endregion

		#region CatalogInventoryDisplay

		internal static CatalogInventoryDisplay ToCatalogInventoryDisplay(this ICatalogInventory catalogInventory)
		{            
			return AutoMapper.Mapper.Map<CatalogInventoryDisplay>(catalogInventory);
		}

		#endregion



		#region ShipCountryDisplay

		internal static ShipCountryDisplay ToShipCountryDisplay(this IShipCountry shipCountry)
		{            
			return AutoMapper.Mapper.Map<ShipCountryDisplay>(shipCountry);
		}

		#endregion

		#region IShipCountry

		internal static IShipCountry ToShipCountry(this ShipCountryDisplay shipCountryDisplay, IShipCountry destination)
		{
			// May not be any mapping

			return destination;
		}

		#endregion

		#region GatewayProviderDisplay

		internal static GatewayProviderDisplay ToGatewayProviderDisplay(this IGatewayProviderSettings gatewayProviderSettings)
		{
			return AutoMapper.Mapper.Map<GatewayProviderDisplay>(gatewayProviderSettings);
		}

		internal static IGatewayProviderSettings ToGatewayProvider(this GatewayProviderDisplay gatewayProvider, IGatewayProviderSettings destination)
		{
			if (gatewayProvider.Key != Guid.Empty) destination.Key = gatewayProvider.Key;
			// type key and typeFullName should be handled by the resolver 
			destination.Name = gatewayProvider.Name;
			destination.Description = gatewayProvider.Description;
			destination.EncryptExtendedData = gatewayProvider.EncryptExtendedData;
			
			((GatewayProviderSettings)destination).ExtendedData = gatewayProvider.ExtendedData.AsExtendedDataCollection();

			return destination;
		}


		#endregion

		#region GatewayResourceDisplay

		internal static GatewayResourceDisplay ToGatewayResourceDisplay(this IGatewayResource gatewayResource)
		{
			return AutoMapper.Mapper.Map<GatewayResourceDisplay>(gatewayResource);
		}

		#endregion

		#region Payment

		internal static PaymentDisplay ToPaymentDisplay(this IPayment payment)
		{
			return AutoMapper.Mapper.Map<PaymentDisplay>(payment);
		}

		internal static IPayment ToPayment(this PaymentDisplay paymentDisplay, IPayment destination)
		{
			if (paymentDisplay.Key != Guid.Empty) destination.Key = paymentDisplay.Key;
			destination.PaymentMethodName = paymentDisplay.PaymentMethodName;
			destination.ReferenceNumber = paymentDisplay.ReferenceNumber;
			destination.Amount = paymentDisplay.Amount;
			//destination.Authorized = paymentDisplay.Authorized;
			//destination.Collected = paymentDisplay.Collected;

			return destination;
		}

		#endregion

		#region PaymentMethod

		internal static PaymentMethodDisplay ToPaymentMethodDisplay(this IPaymentMethod paymentMethod)
		{
			return AutoMapper.Mapper.Map<PaymentMethodDisplay>(paymentMethod);
		}

		internal static PaymentMethodDisplay ToPaymentMethodDisplay(this IPaymentGatewayMethod paymentGatewayMethod)
		{
			return AutoMapper.Mapper.Map<PaymentMethodDisplay>(paymentGatewayMethod);
		}

		internal static IPaymentMethod ToPaymentMethod(this PaymentMethodDisplay paymentMethodDisplay, IPaymentMethod destination)
		{
			if (paymentMethodDisplay.Key != Guid.Empty) destination.Key = paymentMethodDisplay.Key;

			destination.Name = paymentMethodDisplay.Name;
			destination.PaymentCode = paymentMethodDisplay.PaymentCode;
			destination.Description = paymentMethodDisplay.Description;

			return destination;
		}

		#endregion

		#region ShipGatewayProviderDisplay

		internal static ShippingGatewayProviderDisplay ToShipGatewayProviderDisplay(this IShippingGatewayProvider shipGatewayProvider)
		{
			return AutoMapper.Mapper.Map<ShippingGatewayProviderDisplay>(shipGatewayProvider); 
		}

		#endregion

		#region ShipMethodDisplay

		internal static ShipMethodDisplay ToShipMethodDisplay(this IShipMethod shipMethod)
		{            
			return AutoMapper.Mapper.Map<ShipMethodDisplay>(shipMethod);      
		}

		internal static ShipMethodDisplay ToShipMethodDisplay(this IShippingGatewayMethod shippingGatewayMethod)
		{
			return AutoMapper.Mapper.Map<ShipMethodDisplay>(shippingGatewayMethod);
		}

		#endregion

		#region IShipMethod

		internal static IShipMethod ToShipMethod(this ShipMethodDisplay shipMethodDisplay, IShipMethod destination)
		{
			if (shipMethodDisplay.Key != Guid.Empty)
			{
				destination.Key = shipMethodDisplay.Key;
			}
			destination.Name = shipMethodDisplay.Name;
			destination.ServiceCode = shipMethodDisplay.ServiceCode;
			destination.Surcharge = shipMethodDisplay.Surcharge;
			destination.Taxable = shipMethodDisplay.Taxable;           

			foreach (var shipProvinceDisplay in shipMethodDisplay.Provinces)
			{
				IShipProvince destinationShipProvince;

				var matchingItems = destination.Provinces.Where(x => x.Code == shipProvinceDisplay.Code);
				var shipProvinces = matchingItems as IShipProvince[] ?? matchingItems.ToArray();
				if (shipProvinces.Any())
				{
					var existingShipProvince = shipProvinces.First();
					if (existingShipProvince != null)
					{
						destinationShipProvince = existingShipProvince;

						destinationShipProvince = shipProvinceDisplay.ToShipProvince(destinationShipProvince);
					}
				}
			}

			return destination;
		}

		#endregion

		#region ShipProvinceDisplay

		internal static ShipProvinceDisplay ToShipProvinceDisplay(this IShipProvince shipProvince)
		{            
			return AutoMapper.Mapper.Map<ShipProvinceDisplay>(shipProvince);
		}

		#endregion

		#region IShipProvince

		internal static IShipProvince ToShipProvince(this ShipProvinceDisplay shipProvinceDisplay, IShipProvince destination)
		{
			destination.AllowShipping = shipProvinceDisplay.AllowShipping;
			destination.RateAdjustment = shipProvinceDisplay.RateAdjustment;
			destination.RateAdjustmentType = shipProvinceDisplay.RateAdjustmentType;

			return destination;
		}

		#endregion

		#region Invoice

		internal static InvoiceDisplay ToInvoiceDisplay(this IInvoice invoice)
		{
			return AutoMapper.Mapper.Map<InvoiceDisplay>(invoice);
		}

		internal static IInvoice ToInvoice(this InvoiceDisplay invoiceDisplay, IInvoice destination)
		{
			if (invoiceDisplay.Key != Guid.Empty) destination.Key = invoiceDisplay.Key;
			destination.InvoiceNumberPrefix = invoiceDisplay.InvoiceNumberPrefix;
			destination.InvoiceDate = invoiceDisplay.InvoiceDate;
			destination.InvoiceStatus = invoiceDisplay.InvoiceStatus.ToInvoiceStatus();
			destination.VersionKey = invoiceDisplay.VersionKey;
			destination.BillToName = invoiceDisplay.BillToName;
			destination.BillToAddress1 = invoiceDisplay.BillToAddress1;
			destination.BillToAddress2 = invoiceDisplay.BillToAddress2;
			destination.BillToLocality = invoiceDisplay.BillToLocality;
			destination.BillToRegion = invoiceDisplay.BillToRegion;
			destination.BillToPostalCode = invoiceDisplay.BillToPostalCode;
			destination.BillToCountryCode = invoiceDisplay.BillToCountryCode;
			destination.BillToEmail = invoiceDisplay.BillToEmail;
			destination.BillToPhone = invoiceDisplay.BillToPhone;
			destination.BillToCompany = invoiceDisplay.BillToCompany;
			destination.Exported = invoiceDisplay.Exported;
			destination.Archived = invoiceDisplay.Archived;
			return destination;
		}

		internal static IInvoiceStatus ToInvoiceStatus(this InvoiceStatusDisplay invoiceStatusDisplay)
		{
			return new InvoiceStatus()
			{
				Key = invoiceStatusDisplay.Key,
				Alias = invoiceStatusDisplay.Alias,
				Name = invoiceStatusDisplay.Name,
				Active = invoiceStatusDisplay.Active,
				SortOrder = invoiceStatusDisplay.SortOrder,
				Reportable = invoiceStatusDisplay.Reportable
			};
		}

		#endregion

		#region NotificationMessage

		internal static NotificationMessageDisplay ToNotificationMessageDisplay(this INotificationMessage message)
		{
			return AutoMapper.Mapper.Map<NotificationMessageDisplay>(message);
		}

		internal static INotificationMessage ToNotificationMessage(this NotificationMessageDisplay notificationMessageDisplay, INotificationMessage destination)
		{
			if (notificationMessageDisplay.Key != Guid.Empty) destination.Key = notificationMessageDisplay.Key;
			destination.Description = notificationMessageDisplay.Description;
			destination.BodyText = notificationMessageDisplay.BodyText;
			destination.MaxLength = notificationMessageDisplay.MaxLength;
			destination.BodyTextIsFilePath = notificationMessageDisplay.BodyTextIsFilePath;
            destination.MonitorKey = notificationMessageDisplay.MonitorKey;
			destination.Recipients = notificationMessageDisplay.Recipients;
			destination.SendToCustomer = notificationMessageDisplay.SendToCustomer;
			destination.Disabled = notificationMessageDisplay.Disabled;

			return destination;
		}

		internal static INotificationMethod ToNotificationMethod(this NotificationMethodDisplay notificationMethodDisplay, INotificationMethod destination)
		{
			if (notificationMethodDisplay.Key != Guid.Empty) destination.Key = notificationMethodDisplay.Key;
			destination.Name = notificationMethodDisplay.Name;
			destination.Description = notificationMethodDisplay.Description;
			destination.ServiceCode = notificationMethodDisplay.ServiceCode;

			return destination;
		}

		internal static NotificationMethodDisplay ToNotificationMethodDisplay(this INotificationGatewayMethod method)
		{
			return AutoMapper.Mapper.Map<NotificationMethodDisplay>(method);
		}

		#endregion

		#region Order

		internal static OrderDisplay ToOrderDisplay(this IOrder order)
		{
			return AutoMapper.Mapper.Map<OrderDisplay>(order);
		}


		internal static IOrder ToOrder(this OrderDisplay orderDisplay, IOrder destination)
		{
			if (orderDisplay.Key != Guid.Empty) destination.Key = orderDisplay.Key;
			destination.OrderNumberPrefix = orderDisplay.OrderNumberPrefix;
			destination.OrderDate = orderDisplay.OrderDate;
			destination.OrderStatus = orderDisplay.OrderStatus.ToOrderStatus();
			destination.VersionKey = orderDisplay.VersionKey;
			destination.Exported = orderDisplay.Exported;

			// TODO remove existing line items from destination not present in orderDisplay
		    var items = destination.Items.Where(x => orderDisplay.Items.Any(display => display.Key == x.Key));
		    var collection = new LineItemCollection();
		    foreach (var item in items)
		    {
		        collection.Add(item);
		    }

		    ((Order)destination).Items = collection;

			return destination;
		}

		internal static IOrderStatus ToOrderStatus(this OrderStatusDisplay orderStatusDisplay)
		{
			return new OrderStatus()
				{
					Key = orderStatusDisplay.Key,
					Alias = orderStatusDisplay.Alias,
					Name = orderStatusDisplay.Name,
					Active = orderStatusDisplay.Active,
					SortOrder = orderStatusDisplay.SortOrder,
					Reportable = orderStatusDisplay.Reportable
				};
		}

		internal static OrderLineItemDisplay ToOrderLineItemDisplay(this IOrderLineItem orderLineItem)
		{
			return AutoMapper.Mapper.Map<OrderLineItemDisplay>(orderLineItem);
		}

		#endregion

		#region Shipment

		internal static ShipmentDisplay ToShipmentDisplay(this IShipment shipment)
		{
			return AutoMapper.Mapper.Map<ShipmentDisplay>(shipment);
		}

		internal static IShipment ToShipment(this ShipmentDisplay shipmentDisplay, IShipment destination)
		{
			if (shipmentDisplay.Key != Guid.Empty) destination.Key = shipmentDisplay.Key;
			destination.ShippedDate = shipmentDisplay.ShippedDate;
			destination.FromOrganization = destination.FromOrganization;
			destination.FromName = shipmentDisplay.FromName;
			destination.FromAddress1 = shipmentDisplay.FromAddress1;
			destination.FromAddress2 = shipmentDisplay.FromAddress2;
			destination.FromLocality = shipmentDisplay.FromLocality;
			destination.FromRegion = shipmentDisplay.FromRegion;
			destination.FromPostalCode = shipmentDisplay.FromPostalCode;
			destination.FromCountryCode = shipmentDisplay.FromCountryCode;
			destination.FromIsCommercial = shipmentDisplay.FromIsCommercial;
			destination.ToOrganization = shipmentDisplay.ToOrganization;
			destination.ToName = shipmentDisplay.ToName;
			destination.ToAddress1 = shipmentDisplay.ToAddress1;
			destination.ToAddress2 = shipmentDisplay.ToAddress2;
			destination.ToLocality = shipmentDisplay.ToLocality;
			destination.ToRegion = shipmentDisplay.ToRegion;
			destination.ToPostalCode = shipmentDisplay.ToPostalCode;
			destination.ToCountryCode = shipmentDisplay.ToCountryCode;
			destination.ToIsCommercial = shipmentDisplay.ToIsCommercial;
			destination.Phone = shipmentDisplay.Phone;
			destination.Email = shipmentDisplay.Email;
			destination.ShipMethodKey = shipmentDisplay.ShipMethodKey;
			destination.VersionKey = shipmentDisplay.VersionKey;
			destination.Carrier = shipmentDisplay.Carrier;
			destination.TrackingCode = shipmentDisplay.TrackingCode;

			var existing = shipmentDisplay.Items.Where(x => x.Key != Guid.Empty);
			var removed = destination.Items.Where(x => x.Key != Guid.Empty && existing.All(y => y.Key != x.Key)).ToArray();
			if (removed.Any())
			{
				foreach (var remover in removed)
				{
					destination.Items.Remove(remover);
				}
			}

			return destination;
		}

		#endregion



	

		#region ShipFixedRateTableDisplay

		internal static ShipFixedRateTableDisplay ToShipFixedRateTableDisplay(this IShippingFixedRateTable shippingFixedRateTable)
		{            
			return AutoMapper.Mapper.Map<ShipFixedRateTableDisplay>(shippingFixedRateTable);
		}

		#endregion

		#region IShipRateTable

		/// <summary>
		/// Maps changes made in the <see cref="ShipFixedRateTableDisplay"/> to the <see cref="IShippingFixedRateTable"/>
		/// </summary>
		/// <param name="shipFixedRateTableDisplay">The <see cref="ShipFixedRateTableDisplay"/> to map</param>
		/// <param name="destination">The <see cref="IShippingFixedRateTable"/> to have changes mapped to</param>
		/// <returns>The updated <see cref="IShippingFixedRateTable"/></returns>
		/// <remarks>
		/// 
		/// Note: after calling this mapping, the changes are still not persisted to the database as the .Save() method is not called.
		/// 
		/// * For testing you will have to use the static .Save(IGatewayProviderService ..., as MerchelloContext.Current will likely be null
		/// 
		/// </remarks>
		internal static IShippingFixedRateTable ToShipRateTable(this ShipFixedRateTableDisplay shipFixedRateTableDisplay, IShippingFixedRateTable destination)
		{

			// determine if any rows were deleted
			var missingRows =
				destination.Rows.Where(
					persisted => !shipFixedRateTableDisplay.Rows.Select(display => display.Key).Where(x => x != Guid.Empty).Contains(persisted.Key));

			foreach (var missing in missingRows)
			{
				destination.DeleteRow(missing);
			}

			foreach (var shipRateTierDisplay in shipFixedRateTableDisplay.Rows)
			{

				// try to find the matching row
				var destinationTier = destination.Rows.FirstOrDefault(x => x.Key == shipRateTierDisplay.Key);
								
				if (destinationTier != null)
				{
					// update the tier information : note we can only update the Rate here!
					// We need to remove the text boxes for the RangeLow and RangeHigh on any existing FixedRateTable
					destinationTier.Rate = shipRateTierDisplay.Rate;                    
				}
				else
				{
					// this should be implemented in V1
					destination.AddRow(shipRateTierDisplay.RangeLow, shipRateTierDisplay.RangeHigh, shipRateTierDisplay.Rate);
				}

				//IShipRateTier destinationShipRateTier;

				//var matchingItems = destination.Rows.Where(x => x.Key == shipRateTierDisplay.Key).ToArray();
				//if (matchingItems.Any())
				//{
				//    var existingshipRateTier = matchingItems.First();
				//    if (existingshipRateTier != null)
				//    {
				//        destinationShipRateTier = existingshipRateTier;

				//        destinationShipRateTier = shipRateTierDisplay.ToShipRateTier(destinationShipRateTier);
				//    }
				//}
				//else
				//{
				//    // Case if one was created in the back-office.  Not planned for v1
				//    destination.AddRow(shipRateTierDisplay.RangeLow, shipRateTierDisplay.RangeHigh, shipRateTierDisplay.Rate);
				//}
			}

			return destination;
		}

		#endregion

		#region ShipRateTierDisplay

		internal static ShipRateTierDisplay ToShipRateTierDisplay(this IShipRateTier shipRateTier)
		{            
			return AutoMapper.Mapper.Map<ShipRateTierDisplay>(shipRateTier);
		}

		#endregion

		#region IShipRateTier

		internal static IShipRateTier ToShipRateTier(this ShipRateTierDisplay shipRateTierDisplay, IShipRateTier destination)
		{
			if (shipRateTierDisplay.Key != Guid.Empty)
			{
				destination.Key = shipRateTierDisplay.Key;
			}
			destination.RangeHigh = shipRateTierDisplay.RangeHigh;
			destination.RangeLow = shipRateTierDisplay.RangeLow;
			destination.Rate = shipRateTierDisplay.Rate;

			return destination;
		}

		#endregion

		#region TaxMethodDisplay

		internal static ITaxMethod ToTaxMethod(this TaxMethodDisplay taxMethodDisplay, ITaxMethod destination)
		{
			if (taxMethodDisplay.Key != Guid.Empty) destination.Key = taxMethodDisplay.Key;

		   
			destination.Name = taxMethodDisplay.Name;
			destination.PercentageTaxRate = taxMethodDisplay.PercentageTaxRate;

			// this may occur when creating a new tax method since the UI does not 
			// query for provinces 
			// TODO fix
			if (destination.HasProvinces && !taxMethodDisplay.Provinces.Any())
			{
				taxMethodDisplay.Provinces = destination.Provinces.Select(x => x.ToTaxProvinceDisplay()).ToArray();
			}

			foreach (var province in taxMethodDisplay.Provinces)
			{
				var p = destination.Provinces.FirstOrDefault(x => x.Code == province.Code);
				if (p != null) p.PercentAdjustment = province.PercentAdjustment;
			}
		
			return destination;
		}

		internal static TaxMethodDisplay ToTaxMethodDisplay(this ITaxMethod taxMethod)
		{
		   return AutoMapper.Mapper.Map<TaxMethodDisplay>(taxMethod);     
		}

		internal static TaxMethodDisplay ToTaxMethodDisplay(this ITaxationGatewayMethod taxGatewayMethod)
		{
			return AutoMapper.Mapper.Map<TaxMethodDisplay>(taxGatewayMethod);
		}

		#endregion

		#region TaxProvinceDisplay

		internal static TaxProvinceDisplay ToTaxProvinceDisplay(this ITaxProvince taxProvince)
		{            
			return AutoMapper.Mapper.Map<TaxProvinceDisplay>(taxProvince);
		}

		#endregion

		#region WarehouseDisplay

		internal static WarehouseDisplay ToWarehouseDisplay(this IWarehouse warehouse)
		{
			return AutoMapper.Mapper.Map<WarehouseDisplay>(warehouse);
		}

		#endregion

		#region IWarehouse

		internal static IWarehouse ToWarehouse(this WarehouseDisplay warehouseDisplay, IWarehouse destination)
		{
			if (warehouseDisplay.Key != Guid.Empty)
			{
				destination.Key = warehouseDisplay.Key;
			}
			destination.Name = warehouseDisplay.Name;
			destination.Address1 = warehouseDisplay.Address1;
			destination.Address2 = warehouseDisplay.Address2;
			destination.Locality = warehouseDisplay.Locality;
			destination.Region = warehouseDisplay.Region;
			destination.PostalCode = warehouseDisplay.PostalCode;
			destination.CountryCode = warehouseDisplay.CountryCode;
			destination.Phone = warehouseDisplay.Phone;
			destination.Email = warehouseDisplay.Email;
			destination.IsDefault = warehouseDisplay.IsDefault;

			foreach (var warehouseCatalogDisplay in warehouseDisplay.WarehouseCatalogs)
			{
				IWarehouseCatalog destinationWarehouseCatalog;

				var matchingItems = destination.WarehouseCatalogs.Where(x => x.Key == warehouseCatalogDisplay.Key);
				if (matchingItems.Any())
				{
					var existingWarehouseCatalog = matchingItems.First();
					if (existingWarehouseCatalog != null)
					{
						destinationWarehouseCatalog = existingWarehouseCatalog;

						destinationWarehouseCatalog = warehouseCatalogDisplay.ToWarehouseCatalog(destinationWarehouseCatalog);
					}
				}
			}

			return destination;
		}

		#endregion

	}
}
