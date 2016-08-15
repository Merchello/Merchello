namespace Merchello.Tests.IntegrationTests.DisplayClasses
{
    using Merchello.Core.Models;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web.Models.ContentEditing;

    using Newtonsoft.Json;

    using NUnit.Framework;

    public class ProductAttributeSerialization : MerchelloAllInTestBase
    {
        [TestFixture]
        public class ProductAttributeSerializationTests
        {
            [TestFixtureSetUp]
            public void Init()
            {
                AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            }

            [Test]
            public void Can_Serialize_And_Deserialize_A_ProductAttribute()
            {

                var att = new ProductAttribute("test", "test") as IProductAttribute;
                var display = att.ToProductAttributeDisplay();

                var serialized = JsonConvert.SerializeObject(display);


                var deserialized = JsonConvert.DeserializeObject<ProductAttributeDisplay>(serialized);

                Assert.NotNull(deserialized);

            }

            [Test]
            public void Can_Serialize_And_Deserialize_A_ProductAttributeWithDetachedValues()
            {
                
                var att = new ProductAttribute("test", "test") as IProductAttribute;
                att.DetachedDataValues.AddOrUpdate("key", "value", (x, y) => "value");
                var display = att.ToProductAttributeDisplay();

                var serialized = JsonConvert.SerializeObject(display);


                var deserialized = JsonConvert.DeserializeObject<ProductAttributeDisplay>(serialized);

                Assert.NotNull(deserialized);

            }
        }
    }
}