using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Tests.Base.Respositories.UnitOfWork;


namespace Merchello.Tests.Base.Respositories
{
    public class MockUnitOfWorkProvider : IDatabaseUnitOfWorkProvider
    {

        public IDatabaseUnitOfWork GetUnitOfWork()
        {
            return new MockDatabaseUnitOfWork();
        }
    }
}
