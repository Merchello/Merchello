using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models.TypeFields
{
    //public class EnumTypeFieldConverter<T, TResult>
    //{
    //    public static TResult GetTypeField(T enumeratedType)
    //    {
    //        object o = null;
    //        if (typeof (T) == typeof (AddressType)) o = new AddressTypeField();
    //        if (typeof (T) == typeof (BasketType)) o = new BasketTypeField();
    //        if (typeof (T) == typeof (InvoiceItemType)) o = new InvoiceItemTypeField();
    //        if (typeof (T) == typeof (PaymentMethodType)) o = new PaymentMethodTypeField();
    //        if (typeof (T) == typeof (ShipMethodType)) o = new ShipMethodTypeField();
    //        if (typeof (T) == typeof (TransactionType)) o = new TransactionTypeField();
    //        if(o == null) throw new NotSupportedException();

    //        return (TResult) o;
    //    }
    //}


    public class EnumTypeFieldConverter
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

        /// <summary>
        /// Creates an instance of an <see cref="ITransactionTypeField"/>
        /// </summary>
        /// <returns></returns>
        internal static ITransactionTypeField Transaction()
        {
            return new TransactionTypeField();
        }

    }
}
