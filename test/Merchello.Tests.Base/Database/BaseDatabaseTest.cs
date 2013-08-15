using System;
using System.IO;
using NUnit.Framework;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Merchello.Core.Persistence.Migrations.Initial;
using Umbraco.Core.Persistence.SqlSyntax;


namespace Merchello.Tests.Base.Database
{
    /// <summary>
    /// Use this abstract class for tests that requires direct access to the PetaPoco <see cref="Database"/> object.
    /// This base test class will use the database setup with ConnectionString and ProviderName from the test implementation
    /// populated with the umbraco db schema.
    /// </summary>
    /// <remarks>Can be used to test against an Sql Ce, Sql Server and MySql database</remarks>
    [TestFixture]
    public abstract class BaseDatabaseTest
    {
        protected BaseDatabaseTest(UmbracoDatabase database)
        {
            Database = database;
        }
     
     
        [SetUp]
        public virtual void Initialize()
        {
            TestHelper.SetupLog4NetForTests();
            
            string path = TestHelper.CurrentAssemblyDirectory;
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
           
                try
                {
                    //Delete database file before continueing
                    string filePath = string.Concat(path, "\\test.sdf");
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch (Exception)
                {
                    //if this doesn't work we have to make sure everything is reset! otherwise
                    // well run into issues because we've already set some things up
                    TearDown();
                    throw;
                }            



            LogHelper.Info<Umbraco.Core.Persistence.Database>("Initializing database schema creation");

            SqlSyntaxContext.SqlSyntaxProvider = new SqlServerSyntaxProvider();
                
                //new SqlCeSyntaxProvider();
            
           // var creation = new DatabaseSchemaCreation(Database);
            //creation.InitializeDatabaseSchema();

            LogHelper.Info<Umbraco.Core.Persistence.Database>("Finalized database schema creation");
        }


        protected UmbracoDatabase Database { get; private set; }


        [TearDown]
        public virtual void TearDown()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", null);
        }
    }


}