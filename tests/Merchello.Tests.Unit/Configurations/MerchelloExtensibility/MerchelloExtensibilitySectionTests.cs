namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using Merchello.Core.Configuration;

    using NUnit.Framework;

    [TestFixture]
    public class MerchelloExtensibilitySectionTests : MerchelloExtensibilityTests
    {
        [Test]
        public void Views()
        {
            Assert.NotNull(MerchelloConfig.For.Extensibility.Mvc);
        }

        [Test]
        public void BackOffice()
        {
            Assert.NotNull(MerchelloConfig.For.Extensibility.BackOffice);
        }

        /// TODO-remove when add LightInject
        [Test]
        public void Pluggable()
        {
            Assert.NotNull(MerchelloConfig.For.Extensibility.Pluggable);
        }

        /// TODO-remove when add LightInject
        [Test]
        public void Strategies()
        {
            Assert.NotNull(MerchelloConfig.For.Extensibility.Strategies);
        }


        [Test]
        public void TaskChains()
        {
            Assert.NotNull(MerchelloConfig.For.Extensibility.TaskChains);
        }

        [Test]
        public void TypeFields()
        {
            Assert.NotNull(MerchelloConfig.For.Extensibility.TypeFields);
        }
    }
}