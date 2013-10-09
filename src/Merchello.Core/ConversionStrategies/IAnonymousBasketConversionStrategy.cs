using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Core.ConversionStrategies
{
    /// <summary>
    /// Defines an anonymous customer basket conversion strategy
    /// </summary>
    public interface IAnonymousBasketConversionStrategy
    {
        /// <summary>
        /// Converts an anonymous customer's basket to a customer basket
        /// </summary>
        /// <returns><see cref="ICustomerItemCache"/></returns>
        ICustomerItemCache ConvertBasket();
    }
}
