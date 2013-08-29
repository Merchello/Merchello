﻿using System;
using Lucene.Net.Documents;
using Merchello.Core;
using Merchello.Core.ObjectResolution;
using Merchello.Web;
using NUnit.Framework;


namespace Merchello.Tests.UnitTests.Contexts
{
    [TestFixture]
    public class MerchelloBootstrapperTests
    {
        private bool _initEventCalled;
        private bool _startingEventCalled;
        private bool _completedEventCalled;

        [SetUp]
        public void Setup()
        {
            _initEventCalled = false;
            _startingEventCalled = false;
            _completedEventCalled = false;

            MerchelloContext.Current = null;
            Resolution.Reset();

            BootManagerBase.MerchelloInit += delegate {
                _initEventCalled = true;
            };

            BootManagerBase.MerchelloStarting += delegate {
                _startingEventCalled = true;
            };

            BootManagerBase.MerchelloStarted += delegate {
                _completedEventCalled = true;
            };
        }

        /// <summary>
        /// Tests to verify if the <see cref="CoreBootManager"/> can instantiate the MerchelloContext
        /// </summary>
        [Test]
        public void Core_BootManager_Can_Create_MerchelloContext()
        {
            MerchelloBootstrapper.Init(new CoreBootManager());

            var context = MerchelloContext.Current;
            Assert.NotNull(context);
            
            var service = context.Services.CustomerService;
            Assert.NotNull(service);

            Assert.IsTrue(_initEventCalled);
            Assert.IsTrue(_startingEventCalled);
            Assert.IsTrue(_completedEventCalled);
        }

        /// <summary>
        /// Tests to verify if the <see cref="WebBootManager"/> can instantiate the MerchelloContext
        /// </summary>
        [Test]
        public void Web_BootManager_Can_Create_MerchelloContext()
        {
            MerchelloBootstrapper.Init(new WebBootManager(true));

            var context = MerchelloContext.Current;
            Assert.NotNull(context);

            var service = context.Services.CustomerService;
            Assert.NotNull(service);

            Assert.IsTrue(_initEventCalled);
            Assert.IsTrue(_startingEventCalled);
            Assert.IsTrue(_completedEventCalled);
        }

    }
}
