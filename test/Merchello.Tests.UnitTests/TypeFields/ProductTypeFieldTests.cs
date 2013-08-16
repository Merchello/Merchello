using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
{
    [TestFixture]
    [Category("TypeField")]
    public class ProductTypeFieldTests
    {
        [Test]
        public void Empty_custom_configurations_returns_null_type()
        {
            var type = ProductTypeField.Custom("empty");

            Assert.AreEqual(TypeFieldMock.NullTypeField.Alias, type.Alias);
            Assert.AreEqual(TypeFieldMock.NullTypeField.TypeKey, type.TypeKey);
        }
    }
}
