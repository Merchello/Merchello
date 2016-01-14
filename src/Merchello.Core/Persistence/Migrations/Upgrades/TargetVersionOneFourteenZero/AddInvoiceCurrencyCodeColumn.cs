namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneFourteenZero
{
    using System;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Alters the merchInvoice table to add a currency code column.
    /// </summary>
    [Migration("1.13.0", "1.13.4", 0, MerchelloConfiguration.MerchelloMigrationName)]
    internal class AddInvoiceCurrencyCodeColumn : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// The Umbraco database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// The SQL syntax provider.
        /// </summary>
        private readonly ISqlSyntaxProvider _sqlSyntax;

        /// <summary>
        /// The <see cref="InvoiceService"/>.
        /// </summary>
        private readonly InvoiceService _invoiceService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AddInvoiceCurrencyCodeColumn"/> class.
        /// </summary>
        public AddInvoiceCurrencyCodeColumn()
            : base(
                ApplicationContext.Current.DatabaseContext.SqlSyntax,
                Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _database = dbContext.Database;
            _sqlSyntax = dbContext.SqlSyntax;

            _invoiceService = (InvoiceService)MerchelloContext.Current.Services.InvoiceService;
        }

        /// <summary>
        /// Upgrades the database.
        /// </summary>
        public override void Up()
        {
            //// Don't exeucte if the column is already there
            var columns = _sqlSyntax.GetColumnsInSchema(_database).ToArray();
            if (
                columns.Any(
                    x => x.TableName.InvariantEquals("merchInvoice") && x.ColumnName.InvariantEquals("currencyCode"))
                == false)
            {
                Logger.Info(typeof(AddInvoiceCurrencyCodeColumn), "Adding currencyCode column to merchInvoice table.");

                //// Add the new currency code column
                Create.Column("currencyCode").OnTable("merchInvoice").AsString(3).Nullable();

                if (_sqlSyntax is SqlCeSyntaxProvider)
                {
                    SqlCe();
                }
                else
                {
                    SqlServer();
                }

                ////// Populate the values from the line items
                //var sql = @"SELECT T1.pk,
                //    currencyCode = SUBSTRING(
                //    (SELECT TOP 1 extendedData FROM merchInvoiceItem WHERE invoiceKey = T1.pk), 
                //    PATINDEX('%<merchCurrencyCode>%', (SELECT TOP 1 extendedData FROM merchInvoiceItem WHERE invoiceKey = T1.pk)) + 19
                //    , 3) FROM merchInvoice T1";


                //var dtos = _database.Fetch<InvoiceCurrencyDto>(sql);

                //foreach (var dto in dtos)
                //{
                //    Update.Table("merchInvoice")
                //        .Set(new { currencyCode = dto.CurrencyCode })
                //        .Where(new { pk = dto.InvoiceKey });
                //}


                //// Set the column to not null
                Alter.Table("merchInvoice").AlterColumn("currencyCode").AsString(3).NotNullable();
            }
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.14.0 database to a prior version, the database schema has already been modified");
        }


        private void SqlCe()
        {
            var invoices = _invoiceService.GetAll();

            foreach (var inv in invoices)
            {
                if (inv.Items.Any())
                {
                    AddUpdateToTransaction(
                        inv.Items.First().ExtendedData.GetValue(Core.Constants.ExtendedDataKeys.CurrencyCode),
                        inv.Key);
                }
            }
        }

        private void SqlServer()
        {

            //// Populate the values from the line items
            var sql = @"SELECT T1.pk,
                    currencyCode = SUBSTRING(
                    (SELECT TOP 1 extendedData FROM merchInvoiceItem WHERE invoiceKey = T1.pk), 
                    PATINDEX('%<merchCurrencyCode>%', (SELECT TOP 1 extendedData FROM merchInvoiceItem WHERE invoiceKey = T1.pk)) + 19
                    , 3) FROM merchInvoice T1";


            var dtos = _database.Fetch<InvoiceCurrencyDto>(sql);

            foreach (var dto in dtos)
            {
                AddUpdateToTransaction(dto.CurrencyCode, dto.InvoiceKey);
            }
        }

        private void AddUpdateToTransaction(string code, Guid key)
        {
            Update.Table("merchInvoice")
                        .Set(new { currencyCode = code })
                        .Where(new { pk = key });
        }

        /// <summary>
        /// A simple DTO for updating the currency codes in the invoice table.
        /// </summary>
        private class InvoiceCurrencyDto
        {
            /// <summary>
            /// Gets or sets the invoice key.
            /// </summary>
            [Column("pk")]
            public Guid InvoiceKey { get; set; }

            /// <summary>
            /// Gets or sets the currency code.
            /// </summary>
            [Column("currencyCode")]
            public string CurrencyCode { get; set; }
        }
    }
}