using NUnit.Framework;

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

        //protected static Sql TranslateQuery(Sql sqlClause, IQuery<T> query)
        //{
        //    var translator = new SqlTranslator<T>(sqlClause, query);
        //    var sql = translator.Translate();
        //    return sql;
        //}
    }
    
}
