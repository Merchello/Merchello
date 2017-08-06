namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class GatewayProviderSettingsDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GatewayProviderSettingsDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchGatewayProviderSettings");

                    entity.ToTable("merchGatewayProviderSettings");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Description)
                        .HasColumnName("description")
                        .HasMaxLength(255);

                    entity.Property(e => e.EncryptExtendedData)
                        .HasColumnName("encryptExtendedData")
                        .HasDefaultValueSql("'0'");

                    entity.Property(e => e.ExtendedData)
                        .HasColumnName("extendedData")
                        .HasColumnType("ntext");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.ProviderTfKey).HasColumnName("providerTfKey");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");
                });
        }
    }
}