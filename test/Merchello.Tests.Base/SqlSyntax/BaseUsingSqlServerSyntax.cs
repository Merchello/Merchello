using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.ObjectResolution;
using Merchello.Core.Persistence.Mappers;
using Merchello.Core.Persistence.SqlSyntax;
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
            PluginSqlSyntaxContext.SqlSyntaxProvider = new SqlServerSyntaxProvider();
           
            Resolution.Freeze();
            SetUp();
        }

        public virtual void SetUp()
        { }

        [TearDown]
        public virtual void TearDown()
        {
            PluginSqlSyntaxContext.SqlSyntaxProvider = null;
            Resolution.Reset();
        }
    }
    
}
