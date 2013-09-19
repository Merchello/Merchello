using NUnit.Framework;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Tests.Base.SqlSyntax
{
    [TestFixture]
    public abstract class BaseUsingSqlServerSyntax
    {

        [SetUp]
        public virtual void Initialize()
        {            
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();
            SetUp();
        }

        public virtual void SetUp()
        { }

        [TearDown]
        public virtual void TearDown()
        {
            SqlSyntaxContext.SqlSyntaxProvider = null;
            Resolution.Reset();
        }
    }
    
}
