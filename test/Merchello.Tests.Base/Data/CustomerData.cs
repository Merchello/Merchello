using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Tests.Base.Data
{
    public class CustomerData
    {

        public static IAnonymousCustomer AnonymousCustomerMock()
        {
            var key = new Guid("E2D98FAE-EAF3-47B6-9A3F-5E74202043BC");
            var lmd = DateTime.Parse("8/26/2013");
            var anonymous = new AnonymousCustomer(lmd)
            {
                Key = key,
                CreateDate = lmd,
                UpdateDate = lmd
            };

            return anonymous;
        }

        public static ICustomer CustomerForInserting()
        {
            var customer = new Customer(0, 0, null)
                {
                    FirstName = "Joe",
                    LastName = "Schmoe",
                    Email = "joe@schmoe.com",
                    MemberId = null,
                    TotalInvoiced = 0
                };

            customer.ResetDirtyProperties();

            return customer;
        }

        internal static CustomerDto CustomerDtoForInserting(ICustomer c)
        {            
            var dto = new CustomerDto()
            {
                Key = c.Key,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                MemberId = c.MemberId,
                LastPaymentDate = c.LastPaymentDate,
                TotalInvoiced = c.TotalInvoiced,
                TotalPayments = c.TotalPayments,
                CreateDate = c.CreateDate,
                UpdateDate = c.UpdateDate
            };

            return dto;
        }

        public static ICustomer CustomerForUpdating()
        {
            var customer = CustomerData.CustomerForInserting();
            customer.Key = Guid.NewGuid();
            customer.ResetDirtyProperties();
            return customer;

        }

        public static IEnumerable<ICustomer> CustomerListForInserting()
        {
            return new List<ICustomer>()
                {
                    new Customer(0, 0, null)
                        {
                            FirstName = "Joe",
                            LastName = "Schmoe",
                            Email = "jo@schmoe.com",
                            MemberId = null,
                            TotalInvoiced = 0
                        },
                    new Customer(0, 0, null)
                        {
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "john@doe.com",
                            MemberId = null,
                            TotalInvoiced = 0
                        },
                     new Customer(0, 0, null)
                        {
                            FirstName = "Jane",
                            LastName = "Doe",
                            Email = "jane@doe.com",
                            MemberId = null,
                            TotalInvoiced = 0
                        }

                };


        }

    }
}
