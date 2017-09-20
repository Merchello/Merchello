using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Querying
{
    using System.Text;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// Represents the Query Builder for building LINQ translatable queries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Query<T> : IQuery<T>
    {
        private readonly List<Tuple<string, object[]>> _wheres = new List<Tuple<string, object[]>>();

        /// <summary>
        /// Helper method to be used instead of manually creating an instance
        /// </summary>
        public static IQuery<T> Builder
        {
            get { return new Query<T>(); }
        }

        /// <inheritdoc />
        public virtual IQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                var expressionHelper = new ModelToSqlExpressionHelper<T>();
                string whereExpression = expressionHelper.Visit(predicate);

                _wheres.Add(new Tuple<string, object[]>(whereExpression, expressionHelper.GetSqlParameters()));
            }
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> WhereAny(IEnumerable<Expression<Func<T, bool>>> predicates)
        {
            if (predicates == null) return this;

            StringBuilder sb = null;
            List<object> parameters = null;
            Sql sql = null;
            foreach (var predicate in predicates)
            {
                // see notes in Where()
                var expressionHelper = new ModelToSqlExpressionHelper<T>();
                var whereExpression = expressionHelper.Visit(predicate);

                if (sb == null)
                {
                    sb = new StringBuilder("(");
                    parameters = new List<object>();
                    sql = new Sql();
                }
                else
                {
                    sb.Append(" OR ");
                    sql.Append(" OR ");
                }

                sb.Append(whereExpression);
                parameters.AddRange(expressionHelper.GetSqlParameters());
                sql.Append(whereExpression, expressionHelper.GetSqlParameters());
            }

            if (sb == null) return this;

            sb.Append(")");
            //_wheres.Add(Tuple.Create(sb.ToString(), parameters.ToArray()));
            _wheres.Add(Tuple.Create("(" + sql.SQL + ")", sql.Arguments));

            return this;
        }

        /// <summary>
        /// Returns all translated where clauses and their sql parameters
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<string, object[]>> GetWhereClauses()
        {
            return _wheres;
        }

        [Obsolete("This is no longer used, use the GetWhereClauses method which includes the SQL parameters")]
        public List<string> WhereClauses()
        {
            return _wheres.Select(x => x.Item1).ToList();
        }
    }
}