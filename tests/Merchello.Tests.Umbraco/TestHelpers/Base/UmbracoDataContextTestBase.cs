namespace Merchello.Tests.Umbraco.TestHelpers.Base
{
    using System;
    using System.Configuration;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Persistence;
    using global::Umbraco.Core.Persistence.SqlSyntax;

    using Merchello.Tests.Umbraco.TestHelpers.Fake;

    public abstract class UmbracoDataContextTestBase : UmbracoCacheTestBase
    {
        protected ISqlSyntaxProvider SqlSyntaxProvider { get; private set; }


        protected IDatabaseFactory DatabaseFactory { get; private set; }

        protected DatabaseContext DatabaseContext { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);

            // Create a SqlSyntaxProvider for either SqlCe or SqlServer depending on App.config setting
            this.SqlSyntaxProvider = syntax == DbSyntax.SqlCe
                                         ? (ISqlSyntaxProvider)new SqlCeSyntaxProvider()
                                         : (ISqlSyntaxProvider)new SqlServerSyntaxProvider(new Lazy<IDatabaseFactory>(() => null));


            var queryFactory = new FakeQueryFactory(this.SqlSyntaxProvider);

            // Create a Query factory - active database
            this.DatabaseFactory = new TestUmbracoDatabaseFactory(this.Logger, queryFactory);

            // Create an Umbraco Database Context using a fake Umbraco query factory
            this.DatabaseContext = new DatabaseContext(this.DatabaseFactory, this.Logger);

            if (this.DatabaseFactory.CanConnect)
            {
                Console.WriteLine("Can connect to the database.");
            }
        }
    }
}