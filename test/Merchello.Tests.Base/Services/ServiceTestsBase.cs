using Merchello.Tests.Base.Respositories.UnitOfWork;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;

namespace Merchello.Tests.Base.Services
{
    using global::Umbraco.Core.Persistence.SqlSyntax;

    public abstract class ServiceTestsBase<T>
    {
        protected T Before;
        protected T After;
        protected bool BeforeTriggered;
        protected bool AfterTriggered;
        protected bool CommitCalled;

        protected ISqlSyntaxProvider SqlSyntaxProvider;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            SqlSyntaxProvider = SqlSyntaxProviderTestHelper.SqlSyntaxProvider();

            // General tests
            MockDatabaseUnitOfWork.Committed += Comitted;
           
        }

        [TestFixtureTearDown]
        public virtual void FixtureTearDown()
        {
            MockDatabaseUnitOfWork.Committed -= Comitted;
        }

        private void Comitted(object sender)
        {
            CommitCalled = true;
        }

        [SetUp]
        public virtual void Setup()
        {

            // General trigger setup
            BeforeTriggered = false;
            AfterTriggered = false;
            CommitCalled = false;

        }

       
    }
}
