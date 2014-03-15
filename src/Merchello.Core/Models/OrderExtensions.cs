using System.Globalization;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Extension methods for <see cref="IOrder"/>
    /// </summary>
    public static class OrderExtensions
    {
        /// <summary>
        /// Returns a constructed order number (including it's invoice number prefix - if any)
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/></param>
        /// <returns>The prefixed order number</returns>
        public static string PrefixedOrderNumber(this IOrder order)
        {
            return string.IsNullOrEmpty(order.OrderNumberPrefix)
                ? order.OrderNumber.ToString(CultureInfo.InvariantCulture)
                : string.Format("{0}-{1}", order.OrderNumberPrefix, order.OrderNumber);
        }
    }
}