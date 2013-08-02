﻿using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Querying
{
    /// <summary>
    /// Represents the Sql Translator for translating a IQuery object to Sql
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SqlTranslator<T>
    {
        private readonly Sql _sql;

        public SqlTranslator(Sql sql, IQuery<T> query)
        {
            if (sql == null)
                throw new Exception("Sql cannot be null");

            var query1 = query as Query<T>;
            if (query1 == null)
                throw new Exception("Query cannot be null");

            _sql = sql;
            foreach (var clause in query1.WhereClauses())
            {
                _sql.Where(clause);
            }
        }

        public Sql Translate()
        {
            return _sql;
        }

        public override string ToString()
        {
            return _sql.SQL;
        }
    }
}