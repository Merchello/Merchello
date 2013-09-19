using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Umbraco.Core;

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
            var first = FirstNames();
            var last = LastNames();
            var customer = new Customer(0, 0, null)
                {
                    FirstName = first,
                    LastName = last,
                    Email = Email(first, last),
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
                    CustomerForInserting(),
                    CustomerForInserting(),
                    CustomerForInserting()                    
                };


        }


        private static string FirstNames()
        {
            var names = new[]
                {
                    "Jaqueline", "Steve", "Fred", "Stella",
                    "Conner", "Tommy", "Tony", "Shubert", "Abigail", "Lucy",
                    "Olivia", "Lucas", "Gavin", "Stella",
                    "Scooby", "Felix", "Jove", "Flower", "Bailey", "Mohammad", "Issac",
                    "Goofy", "Mickey", "Florence", "Lilly", "Megan", "Oscar",
                    "Boris", "Ivan", "Adolf", "Soren", "Guy", "Pepe", "William", "Pierre", "Francois", "Atilla"
                };
            return SelectRandomString(names);
        }


        private static string LastNames()
        {
            var names = new[]
                {
                    "Wilson", "MacDonald", "Hun", "Scott", "Baker", "Gonzalez", "Nelson",
                    "Carter", "Mitchell", "Perez", "Green", "Allen", "Alan", "Adams", "Martinez",
                    "Hill", "Lee", "Thompson", "Martin", "White", "Fox", "Austin", "Sims", "Lane", "Ching",
                    "Chang", "Ling", "Ping", "Samson", "Marshall", "Oxley", "Pippin", "Ford", "Regan", "Nixon",
                    "Obama", "Bush", "Peterson"
                };

            return SelectRandomString(names);
        }

        public static string Email(string first, string last)
        {
            var name = string.Empty;
            const string email = "{0}@{1}";

         
            var select = Random.Next(3);


            var domains = new[]
                {
                    "dogs.com", "cats.com", "moonshine.net", "jupiterlander.com", "greys.com", "business.io",
                    "scoopy.doo.tv", "whackyfacts.biz",
                    "loads.com", "umbraco.com", "umbraco.io", "proworks.com", "mindfly.com", "merchello.com",
                    "meritage.com", "aramcoexpats.com",
                    "bizspark.org", "orcas.cc", "morcfromorc.com", "me.com", "hotmail.com", "gmail.com"
                };

            switch (select)
            {
                case 1:
                    name = first;
                    break;
                case 2:
                    name = first + "." + last;
                    break;
                default:
                    name = first.Substring(0, 1) + last;
                    break;
            }

            return string.Format(email, name, SelectRandomString(domains));

        }

        private static string SelectRandomString(string[] values)
        {
            
            var index = Random.Next(values.Count());
            return values.ToArray()[index];
        }

        private static readonly Random Random = new Random();
    }
}
