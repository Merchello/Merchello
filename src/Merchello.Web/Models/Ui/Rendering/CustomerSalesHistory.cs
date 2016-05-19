namespace Merchello.Web.Models.Ui.Rendering
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Represents a customer sales history.
    /// </summary>
    public class CustomerSalesHistory : IEnumerable<InvoiceDisplay>, ICustomerSalesHistory
    {
        /// <summary>
        /// The valid invoices statuses.
        /// </summary>
        private readonly Guid[] _validStatuses =
            {
                Core.Constants.DefaultKeys.InvoiceStatus.Paid,
                Core.Constants.DefaultKeys.InvoiceStatus.Partial,
                Core.Constants.DefaultKeys.InvoiceStatus.Unpaid
            };

        /// <summary>
        /// The collection of invoices to enumerate.
        /// </summary>
        private IEnumerable<InvoiceDisplay> _invoices;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerSalesHistory"/> class.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        public CustomerSalesHistory(ICustomer customer)
        {
            this.Initialize(customer);
        }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        public Guid CustomerKey { get; set; }

        /// <summary>
        /// Gets the total purchases.
        /// </summary>
        public decimal TotalPurchases { get; private set; }

        /// <summary>
        /// Gets the total paid.
        /// </summary>
        public decimal TotalPaid { get; private set; }

        /// <summary>
        /// Gets the total outstanding.
        /// </summary>
        public decimal TotalOutstanding
        {
            get
            {
                return TotalPurchases - TotalPaid;
            }
        }

        /// <summary>
        /// Implements GetEnumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<InvoiceDisplay> GetEnumerator()
        {
            return this._invoices.GetEnumerator();
        }

        /// <summary>
        /// Implements IEnumerable.GetEnumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="customer">
        /// The <see cref="ICustomer"/>.
        /// </param>
        private void Initialize(ICustomer customer)
        {
            var invoices = customer.Invoices().ToArray();
            CustomerKey = customer.Key;
            TotalPurchases = invoices.Sum(x => x.Total);
            TotalPaid = invoices
                .Where(x => _validStatuses
                            .Any(y => y == x.InvoiceStatusKey))
                            .Select(x => x.Payments()
                                .Where(pay => pay.Collected)
                                .Sum(pay => pay.Amount)).Sum();

            _invoices = invoices.Select(x => x.ToInvoiceDisplay());
        }
    }
}