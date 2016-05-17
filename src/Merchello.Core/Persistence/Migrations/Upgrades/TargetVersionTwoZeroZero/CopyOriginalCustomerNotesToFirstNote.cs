namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoZeroZero
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Copies original customer notes to first note.
    /// </summary>
    [Migration("1.14.0", "2.0.0", 4, MerchelloConfiguration.MerchelloMigrationName)]
    public class CopyOriginalCustomerNotesToFirstNote : MerchelloMigrationBase, IMerchelloMigration
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
        /// The _note service.
        /// </summary>
        private readonly INoteService _noteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyOriginalCustomerNotesToFirstNote"/> class.
        /// </summary>
        public CopyOriginalCustomerNotesToFirstNote()
            : base(ApplicationContext.Current.DatabaseContext.SqlSyntax, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _database = dbContext.Database;
            _sqlSyntax = dbContext.SqlSyntax;

            _noteService = MerchelloContext.Current.Services.NoteService;
        }

        /// <summary>
        /// Copies data from customer.note field into a new note in the collection.
        /// </summary>
        public override void Up()
        {
            //// Don't exeucte if the column is already there
            var columns = _sqlSyntax.GetColumnsInSchema(_database).ToArray();
            if (
                columns.Any(
                    x => x.TableName.InvariantEquals("merchCustomer") && x.ColumnName.InvariantEquals("notes")))
            {
                var dtos = _database.Fetch<CustomerDto>("SELECT * FROM merchCustomer WHERE notes IS NOT NULL");
                var notes = new List<INote>();
                foreach (var dto in dtos)
                {
                    var note = _noteService.CreateNote(
                        dto.Key,
                        Core.Constants.TypeFieldKeys.Entity.CustomerKey,
                        dto.Notes);

                    note.InternalOnly = true;
                    notes.Add(note);
                }

                if (notes.Any()) _noteService.Save(notes);

                //// now remove the notes column from the merchCustomer table
                //// We can't do this without messing up the Migration.
                //Delete.Column("notes").FromTable("merchCustomer");
            }
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