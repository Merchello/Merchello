using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using NUnit.Framework;

namespace Merchello.Tests.Base.Services
{
    public abstract class ServiceEventTestBase<TService, TEntity>
    {
        private TService _service;
        protected object Before;
        protected object After;

        protected bool BeforeTriggered;
        protected bool AfterTriggered;
        protected bool CommitCalled;

        protected ServiceEventTestBase(TService service)
        {
            _service = service;
        }

        [SetUp]
        public void Setup()
        {
            // General trigger setup
            BeforeTriggered = false;
            AfterTriggered = false;
            CommitCalled = false;

            // General tests
            MockDatabaseUnitOfWork.Committed += delegate(object sender)
            {
                CommitCalled = true;
            };
        }

    }
}
