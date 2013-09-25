﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Querying
{
    /// <summary>
    /// Represents the Query Builder for building LINQ translatable queries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Query<T> : IQuery<T>
    {
        private readonly ModelToSqlExpressionHelper<T> _expresionist = new ModelToSqlExpressionHelper<T>();
        private readonly List<string> _wheres = new List<string>();

        public Query()
            : base()
        { }

        // TODO : figure out if this is an error when call simultaniously using different types.
        // Unit tests error if called all at the same time
        public static IQuery<T> Builder
        {
            get { return new Query<T>(); }
        }

        public virtual IQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                string whereExpression = _expresionist.Visit(predicate);
                _wheres.Add(whereExpression);
            }
            return this;
        }
        
        public List<string> WhereClauses()
        {
            return _wheres;
        }
    }
}