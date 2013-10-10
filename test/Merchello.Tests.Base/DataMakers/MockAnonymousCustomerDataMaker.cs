using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together anonymous customer data for testing
    /// </summary>
    public class MockAnonymousCustomerDataMaker : MockDataMakerBase
    {
        public static IAnonymousCustomer AnonymousCustomerForInserting()
        {   
            var anonymous = new AnonymousCustomer();
            return anonymous;
        }


    }
}
