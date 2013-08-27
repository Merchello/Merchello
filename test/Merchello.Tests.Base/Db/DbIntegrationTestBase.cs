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
            //var config = (MerchelloSection)ConfigurationManager.GetSection("merchello");
            //var connectionString = ConfigurationManager.ConnectionStrings[config.DefaultConnectionStringName].ConnectionString;
            //var provider = ConfigurationManager.ConnectionStrings[config.DefaultConnectionStringName].ProviderName;
            //connectionString = connectionString.Replace("Data Source=|DataDirectory|",
            //    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).Replace("\\bin\\Debug", string.Empty).Replace("\\bin\\Release", string.Empty);

            //connectionString = "Data Source=" + new Uri(connectionString).LocalPath;

            var uowProvider = new PetaPocoUnitOfWorkProvider();


            Database = uowProvider.GetUnitOfWork().Database;

            SqlSyntaxContext.SqlSyntaxProvider =  new SqlServerSyntaxProvider(); //new SqlCeSyntaxProvider(); //

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
