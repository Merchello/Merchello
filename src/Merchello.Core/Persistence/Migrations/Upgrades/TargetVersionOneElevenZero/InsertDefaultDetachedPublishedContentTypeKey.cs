namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneElevenZero
{
    using Merchello.Core.Configuration;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations;

    [Migration("1.10.0", "1.10.0.1", 4, MerchelloConfiguration.MerchelloMigrationName)]
    public class InsertDefaultDetachedPublishedContentTypeKey : MigrationBase
    {
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            database.Execute(
                "UPDATE [merchProductVariant] SET [merchProductVariant].[detachedContentTypeKey] = '"
                + Core.Constants.DefaultKeys.DetachedPublishedContentType
                      .DefaultProductVariantDetachedPublishedContentTypeKey.ToString() + "'");

            this.Alter.Table("merchProductVariant")
                .AlterColumn("detachedContentTypeKey")
                .AsGuid()
                .NotNullable();
        }

        public override void Down()
        {
            throw new System.NotImplementedException();
        }
    }
}