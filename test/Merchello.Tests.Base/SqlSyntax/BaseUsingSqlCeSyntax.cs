using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.ObjectResolution;
using Merchello.Core.Persistence.Mappers;
using NUnit.Framework;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Tests.Base.SqlSyntax
{

    [TestFixture]
    public abstract class BaseUsingSqlCeSyntax
    {
        [SetUp]
        public virtual void Initialize()
        {
            SqlSyntaxContext.SqlSyntaxProvider = new SqlCeSyntaxProvider();
           
            MappingResolver.Current = new MappingResolver(
                () => new List<Type>()
                {
                    typeof(Customer)
                });

            Resolution.Freeze();
            SetUp();
        }

        public virtual void SetUp()
        { }

        [TearDown]
        public virtual void TearDown()
        {
            SqlSyntaxContext.SqlSyntaxProvider = null;
  
        }
    }
    
}
