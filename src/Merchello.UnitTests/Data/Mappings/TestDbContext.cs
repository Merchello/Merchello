namespace Merchello.UnitTests.Data.Mappings
{
    using System.Reflection;

    using JetBrains.Annotations;

    using Merchello.Core.Data.Mappings;
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class TestDbContext : DbContext
    {
        private readonly IDbEntityRegister entityRegister;

        public TestDbContext(DbContextOptions options, IDbEntityRegister register)
            : base(options)
        {
            this.entityRegister = register;
        }


        public virtual DbSet<AnonymousCustomerDto> MerchAnonymousCustomer { get; set; }

        protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
        {
            foreach (var entityMap in this.entityRegister.GetInstantiations())
            {
                entityMap.Configure(modelBuilder);
            }
        }
    }
}