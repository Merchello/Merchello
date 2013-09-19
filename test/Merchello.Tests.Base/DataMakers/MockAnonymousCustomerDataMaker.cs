using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together anonymous customer data for testing
    /// </summary>
    public class MockAnonymousCustomerDataMaker : MockDataMakerBase
    {
        public static IAnonymousCustomer AnonymousCustomerForInserting()
        {   
            var anonymous = new AnonymousCustomer(DateTime.Now);
            return anonymous;
        }

        public static IAnonymousCustomer AnonymousCustomerForUpdating()
        {
            var anonymous = AnonymousCustomerForInserting();
            anonymous.Key = Guid.NewGuid();
            anonymous.CreateDate = DateTime.Now;
            anonymous.UpdateDate = DateTime.Now;
            anonymous.ResetDirtyProperties();
            return anonymous;
        }
    }
}
