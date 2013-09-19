using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.Base.Db
{
    public class DbIntegrationTestBase
    {

        protected Database Database;

        [SetUp]
        public void Initialize()
        {

            var uowProvider = new PetaPocoUnitOfWorkProvider();

            Database = uowProvider.GetUnitOfWork().Database;

            SqlSyntaxContext.SqlSyntaxProvider = SqlSyntaxProviderTestHelper.SqlSyntaxProvider();

            Setup();
        }

        public virtual void Setup()
        {
            
        }

        [TearDown]
        public virtual void TearDown()
        {
            SqlSyntaxContext.SqlSyntaxProvider = null;
            Resolution.Reset();
        }
    }
}
