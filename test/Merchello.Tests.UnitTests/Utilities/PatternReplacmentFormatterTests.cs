using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Formatters;
using Merchello.Core.Models;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Utilities
{
    [TestFixture]
    public class PatternReplacmentFormatterTests
    {
        private IInvoice _invoice;
        private string _message;

        [TestFixtureSetUp]
        public void Init()
        {
            var address = new Address()
            {
                Name = "Mindfly",
                Address1 = "114 W. Magnolia St.",
                Address2 = "Suite 300",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                Email = "debug@mindfly.com",
                Phone = "555-555-5555"
            };

            _invoice = MockInvoiceDataMaker.InvoiceForInserting(address, 599.99M);
            ((Invoice) _invoice).InvoiceNumber = 123;
            _invoice.Items.Add(new InvoiceLineItem(LineItemType.Product, "Xbox One", "Xbox1", 1, 486M));
            _invoice.Items.Add(new InvoiceLineItem(LineItemType.Product, "Xbox One TitanFall", "XB1-TitanFall", 2, 49.99M));
            _invoice.Items.Add(new InvoiceLineItem(LineItemType.Shipping, "Shipping", "Shipping", 1, 30.00M));
            _invoice.Items.Add(new InvoiceLineItem(LineItemType.Tax, "Sales Tax", "Tax", 1, 14.01M));


            _message 
                = @"{{BillToName}}
Your address
{{BillToAddress1}}
{{BillToAddress2}}
{{BillToLocality}}, {{BillToRegion}} {{BillToPostalCode}}

Email : {{BillToEmail}}
Phone : {{BillToPhone}}

Invoice Number : {{InvoiceNumber}}

Items Purchased:

{{IterationStart[Invoice.Items]}}
+ {{Item.Name}} -> {{Item.Sku}} -> {{Item.UnitPrice}} -> {{Item.Quantity}} -> {{Item.TotalPrice}}
{{IterationEnd[Invoice.Items]}}

Thanks for the order.
";
        }

        /// <summary>
        /// Test confirms that the PatternReplaceFormatter can be created with default data (empty replacements)
        /// </summary>
        [Test]       
        public void Can_Get_A_PatternReplaceFormatter_Populated_With_Configuration_Patterns()
        {
            //// Arrange
            
            //// Act
            var formatter = (PatternReplaceFormatter)PatternReplaceFormatter.GetPatternReplaceFormatter();

            //// Assert
            Assert.IsTrue(formatter.Patterns.Any()); 
        }

        /// <summary>
        /// Test asserts taht invoice data can be added to the PatternReplaceFormatter
        /// </summary>
        [Test]
        public void Can_Update_PatternReplaceFormatter_Default_Data_With_Invoice_Data()
        {
            //// Arrange
            var formatter = (PatternReplaceFormatter)PatternReplaceFormatter.GetPatternReplaceFormatter();

            //// Act
            formatter.AddOrUpdateReplaceablePattern(_invoice.ReplaceablePatterns());

            //// Assert
            Assert.AreEqual(formatter.Patterns["InvoiceNumber"].Replacement, "123", "InvoiceNumber does not match");
            Assert.AreEqual(formatter.Patterns["BillToName"].Replacement, "Mindfly", "BillToname does not match");
            Assert.AreEqual(formatter.Patterns["BillToAddress1"].Replacement, "114 W. Magnolia St.", "BillToAddress1 does not match");
            Assert.AreEqual(formatter.Patterns["BillToAddress2"].Replacement, "Suite 300", "BillToAddress2 does not match");
            Assert.AreEqual(formatter.Patterns["BillToLocality"].Replacement, "Bellingham", "BillToLocality does not match");
            Assert.AreEqual(formatter.Patterns["BillToRegion"].Replacement, "WA", "BillToRegion does not match");
            Assert.AreEqual(formatter.Patterns["BillToPostalCode"].Replacement, "98225", "BillToPostalCode does not match");
            Assert.AreEqual(formatter.Patterns["BillToEmail"].Replacement, "debug@mindfly.com", "BillToEmail does not match");
            Assert.AreEqual(formatter.Patterns["BillToPhone"].Replacement, "555-555-5555", "BillToPhone does not match");
        }

        [Test]
        public void Can_Format_A_Message()
        {
            //// Arrange
            var formatter = PatternReplaceFormatter.GetPatternReplaceFormatter();
            formatter.AddOrUpdateReplaceablePattern(_invoice.ReplaceablePatterns());

            //// Act

            var text = formatter.Format(_message);

            Console.Write(text);

            //// Assert
            Assert.IsTrue(text.Contains("Mindfly"));
        }

    }
}