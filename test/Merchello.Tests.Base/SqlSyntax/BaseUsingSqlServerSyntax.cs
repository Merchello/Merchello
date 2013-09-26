using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Querying;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Tests.Base.SqlSyntax
{
    [TestFixture]
    public abstract class BaseUsingSqlServerSyntax<T>
    {

        [SetUp]
        public virtual void Initialize()
        {
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();
            SetUp();
            //Resolution.Freeze();            
        }

        public virtual void SetUp()
        { }

        //[TearDown]
        //public virtual void TearDown()
        //{
        //    SqlSyntaxContext.SqlSyntaxProvider = null;
        //    Resolution.Reset();
        //}

        protected Sql TranslateQuery(Sql sqlClause, IQuery<T> query)
        {
            var translator = new SqlTranslator<T>(sqlClause, query);
            var sql = translator.Translate();
            return sql;
        }
    }
    
}
