namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoThreeZero
{
	using Merchello.Core.Configuration;

	using Umbraco.Core;
	using Umbraco.Core.Logging;
	using Umbraco.Core.Persistence;
	using Umbraco.Core.Persistence.Migrations;
	using Umbraco.Core.Persistence.SqlSyntax;

	/// <summary>
	/// Migration to update the description field size in the merchAppliedPayment table
	/// </summary>
	/// <remarks>
	/// See issue http://issues.merchello.com/youtrack/issue/M-682 for details
	/// </remarks>
	/// <seealso cref="Umbraco.Core.Persistence.Migrations.MigrationBase" />
	/// <seealso cref="Merchello.Core.Persistence.Migrations.IMerchelloMigration" />
	[Migration("2.3.0", 1, MerchelloConfiguration.MerchelloMigrationName)]
	public class AlterAppliedPaymentDescriptionFieldSize : MerchelloMigrationBase, IMerchelloMigration
	{
		/// <summary>
		/// Updates the column size
		/// </summary>
		public override void Up()
		{
			var database = ApplicationContext.Current.DatabaseContext.Database;
            
			var databaseSchemaHelper = new DatabaseSchemaHelper(database, this.Logger, this.SqlSyntax);
			if (databaseSchemaHelper.TableExist("merchAppliedPayment"))
			{
                // Add another assertion that the field size has not already been changed to 500 
                // and some other migration failed which flagged the version to 2.2.0 or earlier
			    var size = database.GetDbTableColumnSize("merchAppliedPayment", "description");

                if (size == 500) return;

                // Update the column to allow for 500 characters instead of 255 which proved too small
                Alter.Table("merchAppliedPayment").AlterColumn("description").AsString(500).NotNullable();
			}
		}

		/// <summary>
		/// Undoes the column size update
		/// </summary>
		/// <exception cref="DataLossException">Cannot downgrade from a version 2.3.0 database to a prior version, the database schema has already been modified</exception>
		public override void Down()
		{
			throw new DataLossException("Cannot downgrade from a version 2.3.0 database to a prior version, the database schema has already been modified");
		}
	}
}
