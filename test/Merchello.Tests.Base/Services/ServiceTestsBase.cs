using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using NUnit.Framework;
using Umbraco.Core.Services;

namespace Merchello.Tests.Base.Services
{
    public abstract class ServiceTestsBase<T>
    {
        protected T Before;
        protected T After;
        protected bool BeforeTriggered;
        protected bool AfterTriggered;
        protected bool CommitCalled;



        protected abstract void Initialize();

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

            Initialize();
        }

       
    }
}
