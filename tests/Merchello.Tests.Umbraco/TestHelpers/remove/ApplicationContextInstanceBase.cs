namespace Merchello.Tests.Umbraco.TestHelpers
{
    using System;

    using global::Umbraco.Core;

    using NUnit.Framework;

    public class ApplicationContextInstanceBase
    {

        protected TestApplicationBase TestApplication;

        [OneTimeSetUp]
        public void Initialize()
        {
            //Initialize the application
            TestApplication = new TestApplicationBase();
            TestApplication.Start(TestApplication, new EventArgs());
            Console.WriteLine("Application Started");

            Console.WriteLine("--------------------");
            //Write status for ApplicationContext
            var context = ApplicationContext.Current;
            Console.WriteLine("ApplicationContext is available: " + (context != null).ToString());
            //Write status for DatabaseContext
            var databaseContext = context.DatabaseContext;
            Console.WriteLine("DatabaseContext is available: " + (databaseContext != null).ToString());
            //Write status for Database object
            var database = databaseContext.Database;
            Console.WriteLine("Database is available: " + (database != null).ToString());
            Console.WriteLine("--------------------");
        } 
    }
}