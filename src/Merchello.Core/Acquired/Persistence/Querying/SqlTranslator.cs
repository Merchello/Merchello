namespace Merchello.Core.Acquired.Persistence.Querying
{
    using System;

    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;

    using NPoco;

    /// <summary>
    /// Represents the Sql Translator for translating a IQuery object to Sql
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity
    /// </typeparam>
    internal class SqlTranslator<T>
    {
        /// <summary>
        /// The NPoco SQL builder.
        /// </summary>
        private readonly Sql<SqlContext> _sql;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTranslator{T}"/> class.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        public SqlTranslator(Sql<SqlContext> sql, IQuery<T> query)
        {
            if (sql == null)
                throw new Exception("Sql cannot be null");

            this._sql = sql;
            foreach (var clause in query.GetWhereClauses())
            {
                this._sql.Where(clause.Item1, clause.Item2);
            }
        }

        /// <summary>
        /// Gets the translated (mapped) SQL.
        /// </summary>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        public Sql<SqlContext> Translate()
        {
            return this._sql;
        }

        public override string ToString()
        {
            return this._sql.SQL;
        }
    }
}