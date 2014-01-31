using System;
using System.Collections.Specialized;
using System.Xml.Linq;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Models
{
    [TestFixture]
    [Category("Models")]
    public class ExtendedDataCollectionTests
    {
        private ExtendedDataCollection _extendedData;

        [SetUp]
        public void Init()
        {
            _extendedData = new ExtendedDataCollection();
        }

        /// <summary>
        /// Test verifies that an item can be added to extended data and events are triggered
        /// </summary>
        [Test]
        public void Can_Add_An_Item_To_Extended_Data_With_Events_Triggered()
        {
            //// Arrange
            var eventTriggered = false;
            var actionIsAdd = false;            
            _extendedData.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs args)
            {
                eventTriggered = true;
                actionIsAdd = args.Action == NotifyCollectionChangedAction.Add;
            };

            //// Act
            _extendedData.SetValue("test", "test");

            //// Assert
            Assert.IsFalse(_extendedData.IsEmpty);
            Assert.IsTrue(eventTriggered);
            Assert.IsTrue(actionIsAdd);
        }

        /// <summary>
        /// Test verifies that an item can be retrieved by its key
        /// </summary>
        [Test]
        public void Can_Verify_A_Value_Can_Be_Retrieved_By_A_Key()
        {
            //// Arrange
            const string key = "key";
            const string value = "Yep";

            //// Act
            _extendedData.SetValue(key, value);
            var retrieved = _extendedData.GetValue(key);

            //// Assert
            Assert.AreEqual(value, retrieved);
        }

        [Test]
        public void Can_Serialize_ExtendedData_To_Xml()
        {
            //// Arrange
            _extendedData.SetValue("key1", "value1");
            _extendedData.SetValue("key2", "value2");
            _extendedData.SetValue("key3", "value3");
            _extendedData.SetValue("key4", "value4");
            _extendedData.SetValue("key5", "value5");

            //// Act
            var xml = _extendedData.SerializeToXml();

            //// Assert
            Console.Write(xml);
            Assert.DoesNotThrow(() => XDocument.Parse(xml));
        }

        [Test]
        public void Can_Deserialize_ExtendedData_To_Dictionary()
        {
            //// Arrange
            var persisted = @"<?xml version=""1.0"" encoding=""utf-16""?><extendedData><key3>value3</key3><key2>value2</key2><key1>value1</key1><key5>value5</key5><key4>value4</key4></extendedData>";

            //// Act
            var extended = new ExtendedDataCollection(persisted);

            //// Assert
            Assert.IsTrue(5 == extended.Count);
            Assert.AreEqual("value3", extended.GetValue("key3"));
        }
    }
}