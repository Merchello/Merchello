using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class TransactionServiceTests : ServiceIntegrationTestBase
    {
        private ITransactionService _transactionService;
        private IPayment _payment;
        private IInvoice _invoice;
        private ICustomer _customer;

        [SetUp]
        public void Initialize()
        {
            

            _transactionService = new TransactionService();

            _customer = MockCustomerDataMaker.CustomerForInserting();
            var customerService = new CustomerService();
            customerService.Save(_customer);

            var invoiceService = new InvoiceService();
                       

            _invoice = invoiceService.CreateInvoice(_customer, MockInvoiceDataMaker.InvoiceStatusUnpaidMock(), Guid.NewGuid().ToString().Substring(0, 5), "Joe",
                "somewhere", string.Empty, "somewhere", "us", "98225", "us", "temp@temp.com", "5555555555",
                "company name");
            invoiceService.Save(_invoice);

            var paymentService = new PaymentService();
            _payment = paymentService.CreatePayment(_customer, "DemoGateway", PaymentMethodType.Cash, "Cash", "Complete", 12.00m);
            paymentService.Save(_payment);

            

            customerService.Save(_customer);

        }

        [Test]
        public void Can_Create_And_Save_Transaction()
        {
            var transaction = _transactionService.CreateTransaction(_payment, _invoice, TransactionType.Credit, 12.00m);

            _transactionService.Save(transaction);

            Assert.IsTrue(transaction.Id > 0);
        }


        [Test]
        public void Can_Create_And_Save_A_List_Of_Transactions()
        {
            var transactions = new List<ITransaction>()
            {
                _transactionService.CreateTransaction(_payment, _invoice, TransactionType.Credit, 4.00m),
                _transactionService.CreateTransaction(_payment, _invoice, TransactionType.Credit, 6.00m),
                _transactionService.CreateTransaction(_payment, _invoice, TransactionType.Credit, 2.00m)
            };

            _transactionService.Save(transactions);

            Assert.IsTrue(transactions[0].Id > 0);
        }

        [Test]
        public void When_A_Payment_Is_Deleted_So_Are_The_Transactions()
        {
            var transactions = new List<ITransaction>()
            {
                _transactionService.CreateTransaction(_payment, _invoice, TransactionType.Credit, 4.00m),
                _transactionService.CreateTransaction(_payment, _invoice, TransactionType.Credit, 6.00m),
                _transactionService.CreateTransaction(_payment, _invoice, TransactionType.Credit, 2.00m)
            };

            _transactionService.Save(transactions);

            var paymentService = new PaymentService();
            paymentService.Delete(_payment);

        }

        //[Test]
        //public void Can_Retrieve_A_List_Of_All_Transactions()
        //{
        //    var transactions = ((TransactionService)_transactionService).GetAll();

        //    Assert.IsTrue(transactions.Any());
        //}

        [Test]
        public void Can_Delete_A_List_Of_Transactions()
        {
            var transactions = ((TransactionService) _transactionService).GetAll();

            if (transactions.Any())
            {
                Console.Write("Deleting {0} transactions", transactions.Count());
                _transactionService.Delete(transactions);

                transactions = ((TransactionService)_transactionService).GetAll();

                Assert.IsFalse(transactions.Any());
            }

            Assert.Pass();
        }

    }
}
