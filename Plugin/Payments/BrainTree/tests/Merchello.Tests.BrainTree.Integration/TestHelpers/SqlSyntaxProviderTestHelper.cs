namespace Merchello.Tests.Braintree.Integration.TestHelpers
{
    using System;
    using System.Configuration;

    using Merchello.Tests.Avalara.Integration.TestBase;

    using Umbraco.Core.Persistence.SqlSyntax;

    internal class SqlSyntaxProviderTestHelper
    {
        public static void EstablishSqlSyntax()
        {
            var syntax = (DbSyntax)Enum.Parse(typeof(DbSyntax), ConfigurationManager.AppSettings["syntax"]);

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