using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models.TypeFields
{

    public class EnumTypeFieldConverter
    {

        /// <summary>
        /// Creates an instance of an <see cref="IAddressTypeField"/> object
        /// </summary>
        internal static IAddressTypeField Address
        {
            get { return new AddressTypeField(); } 
        }

        /// <summary>
        /// Creates an instance of an <see cref="ICustomerItemCacheTypeField"/> object
        /// </summary>
        internal static ICustomerItemCacheTypeField CustomerItemItemCache
        {
            get { return new CustomerItemCacheTypeField(); }            
        }

        /// <summary>
        /// Creates an instance of an <see cref="ILineItemTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static ILineItemTypeField LineItemType
        {
            get { return new LineItemTypeField(); }
        }

        /// <summary>
        /// Creates an instance of an <see cref="IInvoiceItemTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static IInvoiceItemTypeField InvoiceItem
        {
            get { return new InvoiceItemTypeField(); }
        }

        /// <summary>
        /// Creates an instance of an <see cref="IPaymentMethodTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static IPaymentMethodTypeField PaymentMethod
        {
            get { return new PaymentMethodTypeField(); }
        }

        /// <summary>
        /// Creates an instance of an <see cref="IShipmentMethodTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static IShipmentMethodTypeField ShipmentMethod
        {
            get { return new ShipMethodTypeField(); }
        }

        /// <summary>
        /// Creates an instance of an <see cref="IAppliedPaymentTypeField"/>
        /// </summary>
        /// <returns></returns>
        internal static IAppliedPaymentTypeField AppliedPayment
        {
           get { return new AppliedPaymentTypeField(); }
        }

        /// <summary>
        /// Creates an instance of an <see cref="IGatewayProviderTypeField"/>
        /// </summary>
        /// <returns></returns>
        internal static IGatewayProviderTypeField GatewayProvider
        {
            get { return new GatewayProviderTypeField(); }
        }
    }
}
