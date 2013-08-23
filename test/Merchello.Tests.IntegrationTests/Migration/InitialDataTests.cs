using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Tests.Base.Db;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Migration
{
    [TestFixture]
    public class InitialDataTests : DbIntegrationTestBase
    {
        
        [Test]
        public void Can_Populate_typeFieldData_Into_merchDBTypeField()
        {
         
            var creation = new BaseDataCreation(Database);

            creation.InitializeBaseData("merchDBTypeField");

            var dtos = Database.Query<TypeFieldDto>("SELECT * FROM merchDBTypeField");

            var count = dtos.Count();

        }
    }
}
