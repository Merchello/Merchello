namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoZeroZero
{
    using System;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Adds a redirect payment method type.
    /// </summary>
    [Migration("1.14.0", "2.0.0", 4, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddRedirectPaymentMethodType : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// The Umbraco database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddRedirectPaymentMethodType"/> class.
        /// </summary>
        public AddRedirectPaymentMethodType()
            : this(ApplicationContext.Current.DatabaseContext.SqlSyntax, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddRedirectPaymentMethodType"/> class.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AddRedirectPaymentMethodType(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _database = dbContext.Database;
        }

        /// <summary>
        /// Adds the PaymentMethodType field for redirect payments
        /// </summary>
        public override void Up()
        {
            var typeFields = _database.Fetch<TypeFieldDto>("SELECT * FROM merchTypeField");
            if (typeFields.Any(x => x.Key == Constants.TypeFieldKeys.PaymentMethod.RedirectKey)) return;
            var entity = new PaymentMethodTypeField();
            this._database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Redirect.TypeKey, Alias = entity.Redirect.Alias, Name = entity.Redirect.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.0.0 database to a prior version, the database schema has already been modified");
        }
    }
}