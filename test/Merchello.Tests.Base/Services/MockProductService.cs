using System;
using System.Collections.Generic;
using Merchello.Core.Models;


namespace Merchello.Tests.Base.Services
{
    public class MockProductService
    {
        public IProduct CreateProduct(string name, string sku, decimal price)
        {
            return new Product(new ProductVariant(name, sku, price));
        }

        public void Save(IProduct product, bool raiseEvents = true)
        {
            
        }

        public void Save(IEnumerable<IProduct> productList, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IProduct product, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<IProduct> productList, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public IProduct GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProduct> GetByKeys(IEnumerable<Guid> keys)
        {
            throw new NotImplementedException();
        }
    }
}