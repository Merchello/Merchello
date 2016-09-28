namespace Merchello.Tests.Umbraco
{
    using System;

    using global::Umbraco.Core.Logging;

    using Merchello.Core.DependencyInjection;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Mappers;
    using Merchello.Core.Persistence.Repositories;
    using Merchello.Core.Persistence.UnitOfWork;
    using Merchello.Tests.Umbraco.TestHelpers.Base;

    using NUnit.Framework;

    [TestFixture]
    public class ScratchTests : UmbracoApplicationContextBase
    {
        [Test]
        public void LogTest()
        { 
            Logger.Info<ScratchTests>("Logging test");

            Assert.NotNull(IoC.Current);


            Assert.NotNull(ApplicationContext.DatabaseContext, "DatabaseContext was null");
            Assert.NotNull(ApplicationContext.DatabaseContext.SqlSyntax, "SqlSyntax was null");

            var mappingResolver = IoC.Container.GetInstance<IMappingResolver>();

            Assert.NotNull(mappingResolver);

            var dbFactory = IoC.Container.GetInstance<IDatabaseFactory>();
            Assert.NotNull(dbFactory);

            var uowProvider = IoC.Container.GetInstance<IDatabaseUnitOfWorkProvider>();
            using (var uow = uowProvider.CreateUnitOfWork())
            {
                var repo = uow.CreateRepository<IMigrationStatusRepository>();
                Assert.NotNull(repo);
            }
            

            //var unitOfWork = IoC.Container.GetInstance<IUnitOfWork>();

            var manager = IoC.Container.GetInstance<IDatabaseSchemaManager>();
            manager.UninstallDatabaseSchema();
            manager.InstallDatabaseSchema();
            Assert.NotNull(manager);
        }
    }
}
