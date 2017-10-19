namespace Merchello.UnitTests.Registers
{
    using System.Linq;

    using FluentAssertions;

    using Merchello.Core.Data.Contexts;
    using Merchello.Core.Data.Mappings;
    using Merchello.TestBase;

    using Microsoft.EntityFrameworkCore;

    using Xunit;
    using Xunit.Abstractions;

    public class DbEntityRegisterTests : OutputTestBase, IClassFixture<DbEntityRegisterFixture>
    {
        private readonly IDbEntityRegister register;

        private readonly MerchelloDbContext dbContext;

        public DbEntityRegisterTests(ITestOutputHelper output, DbEntityRegisterFixture fixture)
            : base(output)
        {
            this.register = fixture.DbEntityRegister;

            var builder = new DbContextOptionsBuilder<MerchelloDbContext>();
            builder.UseInMemoryDatabase();

            this.dbContext = new MerchelloDbContext(builder.Options, this.register);

        }

        [Fact]
        public void DbEntityRegisterResolvesTypesOnInit()
        {
            // Arrange
            const int expected = 50;

            // Act
            var types = this.register.InstanceTypes.ToArray();

            // Assert
            types.Should().NotBeEmpty();
            types.Count().Should().Be(expected);
            
            foreach (var t in types)
            {
                Output.WriteLine(t.Name);
            }
        }

        [Fact]
        public void DbEntityRegisterCanInstantiateAllTypes()
        {
            // Arrange
            // handled in the IClassFixture instantiation

            // Act
            var instances = this.register.GetInstantiations().ToArray();

            // Assert
            instances.Any().Should().BeTrue();
            instances.Any(i => i == null).Should().BeFalse();
            Output.WriteLine(instances.Length.ToString());

           dbContext.MerchAnonymousCustomer.Any().Should().BeFalse();
        }
    }
}