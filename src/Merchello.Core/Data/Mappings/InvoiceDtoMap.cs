namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class InvoiceDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchInvoice");

                    entity.ToTable("merchInvoice");

                    entity.HasIndex(e => e.BillToPostalCode)
                        .HasName("IX_merchInvoiceBillToPostalCode");

                    entity.HasIndex(e => e.InvoiceDate)
                        .HasName("IX_merchInvoiceDate");

                    entity.HasIndex(e => e.InvoiceNumber)
                        .HasName("IX_merchInvoiceNumber")
                        .IsUnique();

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.Archived).HasColumnName("archived");

                    entity.Property(e => e.BillToAddress1)
                        .HasColumnName("billToAddress1")
                        .HasMaxLength(255);

                    entity.Property(e => e.BillToAddress2)
                        .HasColumnName("billToAddress2")
                        .HasMaxLength(255);

                    entity.Property(e => e.BillToCompany)
                        .HasColumnName("billToCompany")
                        .HasMaxLength(255);

                    entity.Property(e => e.BillToCountryCode)
                        .HasColumnName("billToCountryCode")
                        .HasMaxLength(255);

                    entity.Property(e => e.BillToEmail)
                        .HasColumnName("billToEmail")
                        .HasMaxLength(255);

                    entity.Property(e => e.BillToLocality)
                        .HasColumnName("billToLocality")
                        .HasMaxLength(255);

                    entity.Property(e => e.BillToName)
                        .HasColumnName("billToName")
                        .HasMaxLength(255);

                    entity.Property(e => e.BillToPhone)
                        .HasColumnName("billToPhone")
                        .HasMaxLength(255);

                    entity.Property(e => e.BillToPostalCode)
                        .HasColumnName("billToPostalCode")
                        .HasMaxLength(255);

                    entity.Property(e => e.BillToRegion)
                        .HasColumnName("billToRegion")
                        .HasMaxLength(255);

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.CurrencyCode)
                        .IsRequired()
                        .HasColumnName("currencyCode")
                        .HasMaxLength(3);

                    entity.Property(e => e.CustomerKey).HasColumnName("customerKey");

                    entity.Property(e => e.Exported).HasColumnName("exported");

                    entity.Property(e => e.InvoiceDate)
                        .HasColumnName("invoiceDate")
                        .HasColumnType("datetime");

                    entity.Property(e => e.InvoiceNumber).HasColumnName("invoiceNumber");

                    entity.Property(e => e.InvoiceNumberPrefix)
                        .HasColumnName("invoiceNumberPrefix")
                        .HasMaxLength(255);

                    entity.Property(e => e.InvoiceStatusKey).HasColumnName("invoiceStatusKey");

                    entity.Property(e => e.PoNumber)
                        .HasColumnName("poNumber")
                        .HasMaxLength(255);

                    entity.Property(e => e.Total)
                        .HasColumnName("total")
                        .HasColumnType("numeric");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.VersionKey)
                        .HasColumnName("versionKey")
                        .HasDefaultValueSql("'newid()'");

                    entity.HasOne(d => d.CustomerDtoKeyNavigation)
                        .WithMany(p => p.MerchInvoice)
                        .HasForeignKey(d => d.CustomerKey)
                        .HasConstraintName("FK_merchInvoice_merchCustomer");

                    entity.HasOne(d => d.InvoiceStatusDtoKeyNavigation)
                        .WithMany(p => p.MerchInvoice)
                        .HasForeignKey(d => d.InvoiceStatusKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchInvoice_merchInvoiceStatus");
                });
        }
    }
}