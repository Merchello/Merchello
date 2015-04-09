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

    }
}