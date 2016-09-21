namespace Merchello.Tests.Umbraco.TestHelpers
{
    using System;
    using System.Configuration;

    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Persistence;
    using global::Umbraco.Core.Persistence.SqlSyntax;
    using global::Umbraco.Core.Plugins;


    using NUnit.Framework;

    public abstract class UmbracoInstanceBase
    {
        protected ISqlSyntaxProvider SqlSyntaxProvider { get; private set; }

        protected ILogger Logger { get; private set; }

        protected IDatabaseFactory DatabaseFactory { get; private set; }

        protected CacheHelper CacheHelper { get; private set; }

        protected abstract bool EnableCache { get; }

        [OneTimeSetUp]
        public void Initialize()
        {
            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);

            this.SqlSyntaxProvider = syntax == DbSyntax.SqlCe
                                         ? (ISqlSyntaxProvider)new SqlCeSyntaxProvider()
                                         : (ISqlSyntaxProvider)new SqlServerSyntaxProvider(new Lazy<IDatabaseFactory>(() => null));

            this.Logger = global::Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration();

            this.DatabaseFactory = new TestDatabaseFactory();
            if (this.DatabaseFactory.CanConnect)
            {
                Console.WriteLine("Can connect to the database.");
            }

            this.CacheHelper = EnableCache
                                   ? new CacheHelper(
                                         new ObjectCacheRuntimeCacheProvider(),
                                         new StaticCacheProvider(),
                                         new NullCacheProvider(),
                                         new IsolatedRuntimeCache(type => new ObjectCacheRuntimeCacheProvider()))
                                   : CacheHelper.CreateDisabledCacheHelper();

        }
    }
}