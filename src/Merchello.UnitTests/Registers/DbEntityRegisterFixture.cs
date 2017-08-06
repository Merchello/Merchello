namespace Merchello.UnitTests.Registers
{
    using Merchello.Core.Data.Mappings;
    using Merchello.Core.Logging;
    using Merchello.TestBase.Fixtures;

    using Moq;

    public class DbEntityRegisterFixture : TestFixtureBase
    {
        public DbEntityRegisterFixture()
            :base(new Mock<ILogger>().Object)
        {
            this.DbEntityRegister = new DbEntityRegister(Logger);
        }

        public IDbEntityRegister DbEntityRegister { get; }
    }
}