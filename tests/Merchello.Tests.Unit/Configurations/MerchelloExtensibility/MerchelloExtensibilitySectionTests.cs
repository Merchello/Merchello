namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using Merchello.Core.Configuration;

    using NUnit.Framework;

    [TestFixture]
    public class MerchelloExtensibilitySectionTests : MerchelloExtensibilityTests
    {
        [Test]
        public void Mvc()
        {
            Assert.NotNull(ExtensibilitySection.Mvc);
        }

        [Test]
        public void BackOffice()
        {
            Assert.NotNull(ExtensibilitySection.BackOffice);
        }

        /// TODO-remove when add LightInject
        [Test]
        public void Pluggable()
        {
            Assert.NotNull(ExtensibilitySection.Pluggable);
        }

        /// TODO-remove when add LightInject
        [Test]
        public void Strategies()
        {
            Assert.NotNull(ExtensibilitySection.Strategies);
        }


        [Test]
        public void TaskChains()
        {
            Assert.NotNull(ExtensibilitySection.TaskChains);
        }

        [Test]
        public void TypeFields()
        {
            Assert.NotNull(ExtensibilitySection.TypeFields);
        }
    }
}