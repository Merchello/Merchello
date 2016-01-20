namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneTenZero
{
    using System.Runtime.InteropServices;

    using Merchello.Core.Configuration;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Adds the productTaxMethod field to the merchelloTaxMethod table.
    /// </summary>
    [Migration("1.9.0", "1.10.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddTaxMethodColumn : MigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// The <see cref="DatabaseSchemaHelper"/>.
        /// </summary>
        private readonly DatabaseSchemaHelper _databaseSchemaHelper;

        /// <summary>
        /// The <see cref="ISqlSyntaxProvider"/>.
        /// </summary>
        private readonly ISqlSyntaxProvider _sqlSyntaxProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddTaxMethodColumn"/> class.
        /// </summary>
        public AddTaxMethodColumn()
            : this(ApplicationContext.Current.DatabaseContext.SqlSyntax, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddTaxMethodColumn"/> class.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AddTaxMethodColumn(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
            _sqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            _databaseSchemaHelper = new DatabaseSchemaHelper(ApplicationContext.Current.DatabaseContext.Database, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration(), _sqlSyntaxProvider);
        }

        /// <summary>
        /// Adds the productTaxation field on an upgrade
        /// </summary>
        public override void Up()
        {
            if (_databaseSchemaHelper.TableExist("merchTaxMethod"))
            this.Alter.Table("merchTaxMethod").AddColumn("productTaxMethod").AsBoolean().NotNullable().WithDefaultValue('0');
        }

        /// <summary>
        /// Removes the productTaxMethod field on a downgrade
        /// </summary>        
        public override void Down()
        {
            if (_databaseSchemaHelper.TableExist("merchTaxMethod"))
            this.Delete.Column("productTaxMethod").FromTable("merchTaxMethod");
        }
    }
}