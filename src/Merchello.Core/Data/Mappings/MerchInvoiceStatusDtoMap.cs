namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class MerchInvoiceStatusDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceStatusDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchInvoiceStatus");

                    entity.ToTable("merchInvoiceStatus");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.Active).HasColumnName("active");

                    entity.Property(e => e.Alias)
                        .IsRequired()
                        .HasColumnName("alias")
                        .HasMaxLength(255);

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.Reportable).HasColumnName("reportable");

                    entity.Property(e => e.SortOrder).HasColumnName("sortOrder");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");
                });
        }
    }
}