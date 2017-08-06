namespace Merchello.UnitTests.Registers
{
    using System.Linq;

    using FluentAssertions;

    using Merchello.Core.Data.Mappings;
    using Merchello.TestBase;
    using Merchello.UnitTests.Data.Mappings;

    using Microsoft.EntityFrameworkCore;

    using Xunit;
    using Xunit.Abstractions;

    public class DbEntityRegisterTests : OutputTestBase, IClassFixture<DbEntityRegisterFixture>
    {
        private readonly IDbEntityRegister register;

        private readonly TestDbContext dbContext;

        public DbEntityRegisterTests(ITestOutputHelper output, DbEntityRegisterFixture fixture)
            : base(output)
        {
            this.register = fixture.DbEntityRegister;

            var builder = new DbContextOptionsBuilder<TestDbContext>();
            builder.UseInMemoryDatabase();

            this.dbContext = new TestDbContext(builder.Options, this.register);

        }

        [Fact]
        public void DbEntityRegisterResolvesTypesOnInit()
        {
            // Arrange
            // handled in the IClassFixture instantiation

            // Act
            var types = this.register.InstanceTypes.ToArray();

            // Assert
            types.Any().Should().BeTrue();
            Output.WriteLine(types.Length.ToString());
        }

        //[Fact]
        //public void DbEntityRegisterCanInstantiateAllTypes()
        //{
        //    // Arrange
        //    // handled in the IClassFixture instantiation

        //    // Act
        //    var instances = this.register.GetInstantiations().ToArray();

        //    // Assert
        //    instances.Any().Should().BeTrue();
        //    instances.Any(i => i == null).Should().BeFalse();
        //    Output.WriteLine(instances.Length.ToString());

        //    dbContext.MerchAnonymousCustomer.Any().Should().BeFalse();
        //}
    }
}