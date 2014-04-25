using Merchello.Tests.Base.Respositories.UnitOfWork;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;

namespace Merchello.Tests.Base.Services
{
    public abstract class ServiceTestsBase<T>
    {
        protected T Before;
        protected T After;
        protected bool BeforeTriggered;
        protected bool AfterTriggered;
        protected bool CommitCalled;



        [SetUp]
        public virtual void Setup()
        {

            // General trigger setup
            BeforeTriggered = false;
            AfterTriggered = false;
            CommitCalled = false;

            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            // General tests
            MockDatabaseUnitOfWork.Committed += delegate(object sender)
            {
                CommitCalled = true;
            };

        }

       
    }
}
