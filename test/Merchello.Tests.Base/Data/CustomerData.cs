﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.Data
{
    public class CustomerData
    {

        public static ICustomer CustomerForInserting()
        {
            var customer = new Customer(0, 0, null)
                {
                    FirstName = "Joe",
                    LastName = "Schmoe",
                    MemberId = null,
                    TotalInvoiced = 0
                };

            customer.ResetDirtyProperties();

            return customer;
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
                            MemberId = null,
                            TotalInvoiced = 0
                        },
                    new Customer(0, 0, null)
                        {
                            FirstName = "John",
                            LastName = "Doe",
                            MemberId = null,
                            TotalInvoiced = 0
                        },
                     new Customer(0, 0, null)
                        {
                            FirstName = "Jane",
                            LastName = "Doe",
                            MemberId = null,
                            TotalInvoiced = 0
                        }

                };


        }

    }
}
