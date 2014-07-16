using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together customer data for testing
    /// </summary>
    public class MockCustomerDataMaker : MockDataMakerBase
    {       

        public static ICustomer CustomerForInserting()
        {
            var first = FirstNames();
            var last = LastNames();
            var customer = new Customer(FirstNames())
                {
                    FirstName = first,
                    LastName = last,
                    Email = Email(first, last)
                };

            return customer;
        }



        internal static CustomerDto CustomerDtoForInserting(ICustomer c)
        {
            var dto = new CustomerDto()
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    LoginName = c.LoginName,
                    CreateDate = c.CreateDate,
                    UpdateDate = c.UpdateDate
                };

            return dto;
        }



        public static IEnumerable<ICustomer> CustomerListForInserting(int count)
        {
            for (var i = 0; i < count; i++) yield return CustomerForInserting();
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

        private static string Email(string first, string last)
        {
            var name = string.Empty;
            const string email = "{0}@{1}";

         
            var select = NoWhammyStop.Next(3);


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

    }
}
