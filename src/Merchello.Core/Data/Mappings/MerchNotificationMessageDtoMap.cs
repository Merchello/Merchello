namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchNotificationMessageDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchNotificationMessage>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchNotificationMessage");

                    entity.ToTable("merchNotificationMessage");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.BodyText)
                        .HasColumnName("bodyText")
                        .HasColumnType("ntext");

                    entity.Property(e => e.BodyTextIsFilePath).HasColumnName("bodyTextIsFilePath");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Description)
                        .HasColumnName("description")
                        .HasMaxLength(255);

                    entity.Property(e => e.Disabled).HasColumnName("disabled");

                    entity.Property(e => e.FromAddress)
                        .HasColumnName("fromAddress")
                        .HasMaxLength(255);

                    entity.Property(e => e.MaxLength).HasColumnName("maxLength");

                    entity.Property(e => e.MethodKey).HasColumnName("methodKey");

                    entity.Property(e => e.MonitorKey).HasColumnName("monitorKey");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.Recipients)
                        .IsRequired()
                        .HasColumnName("recipients")
                        .HasMaxLength(255);

                    entity.Property(e => e.ReplyTo)
                        .HasColumnName("replyTo")
                        .HasMaxLength(255);

                    entity.Property(e => e.SendToCustomer).HasColumnName("sendToCustomer");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.MethodKeyNavigation)
                        .WithMany(p => p.MerchNotificationMessage)
                        .HasForeignKey(d => d.MethodKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchNotificationMessage_merchNotificationMethod");
                });
        }
    }
}