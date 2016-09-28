namespace Merchello.Core.Persistence
{
    using NPoco;

    /// <summary>
    /// Extension methods for <see cref="IMerchelloDatabase"/>.
    /// </summary>
    internal static class MerchelloDatabaseExtensions
    {
        /// <summary>
        /// Creates NPoco SQL Builder.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <returns>
        /// The <see cref="NPoco.Sql"/>.
        /// </returns>
        public static Sql<SqlContext> Sql(this IMerchelloDatabase db)
        {
            return NPoco.Sql.BuilderFor(new SqlContext(db));
        }

        /// <summary>
        /// Creates NPoco SQL Builder.
        /// </summary>
        /// <param name="db">
        /// The db.
        /// </param>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="NPoco.Sql"/>.
        /// </returns>
        public static Sql<SqlContext> Sql(this IMerchelloDatabase db, string sql, params object[] args)
        {
            return NPoco.Sql.BuilderFor(new SqlContext(db)).Append(sql, args);
        }
    }
}