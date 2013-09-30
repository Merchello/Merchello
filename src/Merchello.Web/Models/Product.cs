using System;
using Merchello.Core.Models;

namespace Merchello.Web.Models
{
    public class Product : IProduct
    {

        public Product(Guid key)
        { }

        public Product(Guid productCompositionKey, int[] optionIds)
        {
            
        }

    }
}