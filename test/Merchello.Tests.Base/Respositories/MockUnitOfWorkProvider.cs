using Merchello.Tests.Base.Respositories.UnitOfWork;
using Umbraco.Core.Persistence.UnitOfWork;

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
