using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.UnitTests.Repository
{
    public class MockUnitOfWorkProvider : IDatabaseUnitOfWorkProvider
    {

        public IDatabaseUnitOfWork GetUnitOfWork()
        {
            return new MockDatabaseUnitOfWork();
        }
    }
}
