namespace Merchello.Tests.Avalara.Integration.TestBase
{
    using Umbraco.Core.Persistence.SqlSyntax;

    internal class SqlSyntaxProviderTestHelper
    {
        public static void EstablishSqlSyntax(DbSyntax syntax = DbSyntax.SqlCe)
        {
            if (Resolution.IsFrozen) return;
            SqlSyntaxContext.SqlSyntaxProvider = SqlSyntaxProvider(syntax);
            
            Resolution.Freeze();
        }

        public static ISqlSyntaxProvider SqlSyntaxProvider(DbSyntax syntax = DbSyntax.SqlCe)
        {
            return syntax == DbSyntax.SqlServer ? new SqlServerSyntaxProvider() : (ISqlSyntaxProvider)new SqlCeSyntaxProvider();
        }
    }
}