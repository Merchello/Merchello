using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models.Rdbms;
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
                    FirstName = "Joe",
                    LastName = "Schmoe",
                    TotalInvoiced = 0,
                    TotalPayments = 0,
                    LastPaymentDate = null,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

           //var poco = _database.Insert(dto);

            var id = new Guid("87737E8D-8E22-4BE2-B738-B9286A2AFB54");



          //_database.Delete(dto);


        }

    }
}
