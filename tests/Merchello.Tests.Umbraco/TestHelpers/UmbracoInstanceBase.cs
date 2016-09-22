namespace Merchello.Tests.Umbraco.TestHelpers
{
    using System;
    using System.Configuration;
    using System.IO;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Configuration;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Persistence;
    using global::Umbraco.Core.Persistence.SqlSyntax;
    using global::Umbraco.Core.Plugins;
    using global::Umbraco.Core.Services;

    using Merchello.Core.Boot;
    using Merchello.Tests.Umbraco.TestHelpers.Adapter;
    using Merchello.Tests.Umbraco.TestHelpers.Boot;
    using Merchello.Tests.Umbraco.TestHelpers.Fake;
    using Merchello.Umbraco.Adapters;
    using Merchello.Umbraco.Boot;

    using Moq;

    using NUnit.Framework;

    public abstract class UmbracoInstanceBase
    {
        protected ISqlSyntaxProvider SqlSyntaxProvider { get; private set; }

        protected ILogger Logger { get; private set; }

        protected IDatabaseFactory DatabaseFactory { get; private set; }

        protected DatabaseContext DatabaseContext { get; private set; }

        protected CacheHelper CacheHelper { get; private set; }

        protected ApplicationContext ApplicationContext { get; private set; }

        protected virtual bool EnableCache
        {
            get
            {
                return false;
            }
        }

        [OneTimeSetUp]
        public void Initialize()
        {
            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);

            this.SqlSyntaxProvider = syntax == DbSyntax.SqlCe
                                         ? (ISqlSyntaxProvider)new SqlCeSyntaxProvider()
                                         : (ISqlSyntaxProvider)new SqlServerSyntaxProvider(new Lazy<IDatabaseFactory>(() => null));

            this.Logger = new Logger(new FileInfo(TestHelper.MapPathForTest("~/Config/log4net.config")));

            var queryFactory = new FakeQueryFactory(SqlSyntaxProvider);

            this.DatabaseFactory = new TestUmbracoDatabaseFactory(Logger, queryFactory);

            DatabaseContext = new DatabaseContext(DatabaseFactory, Logger);


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

            ApplicationContext = BuildApplicationContext();

            MerchelloBootstrapper.Init(new TestBoot(ApplicationContext));
        }

        private ApplicationContext BuildApplicationContext()
        {
            // Goofy way to get around internals
            var profiler = new Core.Acquired.Logging.LogProfiler(new LoggerAdapter(Logger));
            var profileLogger = new ProfilingLogger(Logger, new LogProfilerAdapter(profiler));

            var serviceContext = new ServiceContext();

            return new ApplicationContext(DatabaseContext, serviceContext, CacheHelper, profileLogger);
        }
    }
}