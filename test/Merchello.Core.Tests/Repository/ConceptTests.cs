using System;
using System.Collections.Generic;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Services;
using NUnit.Framework;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Tests.Repository
{
    [TestFixture]
    public class ConceptTests
    {
        private string _connectionStringName;
        private Database _database;

        [SetUp]
        public void Setup()
        {
            var config = (MerchelloSection) ConfigurationManager.GetSection("merchello");
            _connectionStringName = config.DefaultConnectionStringName;

            _database = new UmbracoDatabase(_connectionStringName);
        }

        [Test]
        public void Main()
        {
            var provider = new PetaPocoUnitOfWorkProvider();
            var uow = provider.GetUnitOfWork();
            
            
            var dto = new CustomerDto()
                {
                    Pk = Guid.NewGuid(),
                    MemberId = null,
                    FirstName = "John",
                    LastName = "Schmoe",
                    TotalInvoiced = 0,
                    TotalPayments = 0,
                    LastPaymentDate = null,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

            //var poco = _database.Insert(dto);

            //var id = new Guid("87737E8D-8E22-4BE2-B738-B9286A2AFB54");

            var service = new CustomerService();
            var customers = service.GetAll();

            //var c1 = service.CreateCustomer("Rusty", "Swayne");
            //var c2 = service.CreateCustomer("Kara", "Swayne");

            //var customers = new List<ICustomer> {c1, c2 };

            //service.Save(customers);
            service.Delete(customers);
            

            //var kid = new Guid("B5A0E744-4EBD-44E9-BD90-4C76D29594E1");
            //var kara = service.GetByKey(kid);
            //kara.MemberId = 1;
            //service.Save(kara);


            //var firstName = customer.FirstName;

            //int count = customers.Count();

            //_database.Delete(dto);


        }

    }
}
