namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchInvoiceItemDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceItemDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchInvoiceItem");

                    entity.ToTable("merchInvoiceItem");

                    entity.HasIndex(e => e.Sku)
                        .HasName("IX_merchInvoiceItemSku");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Exported).HasColumnName("exported");

                    entity.Property(e => e.ExtendedData)
                        .HasColumnName("extendedData")
                        .HasColumnType("ntext");

                    entity.Property(e => e.InvoiceKey).HasColumnName("invoiceKey");

                    entity.Property(e => e.LineItemTfKey).HasColumnName("lineItemTfKey");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.Price)
                        .HasColumnName("price")
                        .HasColumnType("numeric");

                    entity.Property(e => e.Quantity).HasColumnName("quantity");

                    entity.Property(e => e.Sku)
                        .IsRequired()
                        .HasColumnName("sku")
                        .HasMaxLength(255);

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.InvoiceDtoKeyNavigation)
                        .WithMany(p => p.MerchInvoiceItem)
                        .HasForeignKey(d => d.InvoiceKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchInvoiceItem_merchInvoice");
                });
        }
    }
}