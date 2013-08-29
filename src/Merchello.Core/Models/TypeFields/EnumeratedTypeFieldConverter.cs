using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models.TypeFields
{
    public class EnumeratedTypeFieldConverter
    {
        /// <summary>
        /// Creates an instance of an <see cref="IAddressTypeField"/> object
        /// </summary>
        internal static IAddressTypeField Address()
        {
            return new AddressTypeField();
        }

        /// <summary>
        /// Creates an instance of an <see cref="IBasketTypeField"/> object
        /// </summary>
        internal static IBasketTypeField Basket()
        {
            return new BasketTypeField();            
        }

        /// <summary>
        /// Creates an instance of an <see cref="IInvoiceItemTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static IInvoiceItemTypeField InvoiceItem()
        {
            return new InvoiceItemTypeField();
        }

        /// <summary>
        /// Creates an instance of an <see cref="IPaymentMethodTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static IPaymentMethodTypeField PaymentMethod()
        {
            return new PaymentMethodTypeField();
        }

        /// <summary>
        /// Creates an instance of an <see cref="IShipmentMethodTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static IShipmentMethodTypeField ShipmentMethod()
        {
            return new ShipMethodTypeField();
        }

    }
}
