namespace Merchello.Core.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Models;
    using Sales;
    using Umbraco.Core;

    /// <summary>
    /// Represents an invoice builder
    /// </summary>
    internal sealed class InvoiceBuilderChain : BuildChainBase<IInvoice>
    {
        /// <summary>
        /// The sale preparation.
        /// </summary>
        private readonly SalePreparationBase _salePreparation;

        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceBuilderChain"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The sale preparation.
        /// </param>
        internal InvoiceBuilderChain(SalePreparationBase salePreparation)
        {
            Mandate.ParameterNotNull(salePreparation, "salesPreparation");
            _salePreparation = salePreparation;

            ResolveChain(Core.Constants.TaskChainAlias.SalesPreparationInvoiceCreate);
        }

        /// <summary>
        /// Gets the count of tasks - Used for testing
        /// </summary>
        internal int TaskCount
        {
            get { return TaskHandlers.Count(); }
        }

        /// <summary>
        /// Gets the constructor argument values.
        /// </summary>
        protected override IEnumerable<object> ConstructorArgumentValues
        {
            get
            {
                return _constructorParameters ??
                    (_constructorParameters = new List<object>(new object[] { _salePreparation }));
            }
        }

        /// <summary>
        /// Builds the invoice
        /// </summary>
        /// <returns>The Attempt{IInvoice} representing the successful creation of an invoice</returns>
        public override Attempt<IInvoice> Build()
        {
            var unpaid =
                _salePreparation.MerchelloContext.Services.InvoiceService.GetInvoiceStatusByKey(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);

            if (unpaid == null)
                return Attempt<IInvoice>.Fail(new NullReferenceException("Unpaid invoice status query returned null"));

            var invoice = new Invoice(unpaid) { VersionKey = _salePreparation.ItemCache.VersionKey };

            // Associate a customer with the invoice if it is a known customer.
            if (!_salePreparation.Customer.IsAnonymous) invoice.CustomerKey = _salePreparation.Customer.Key;

            var attempt = TaskHandlers.Any()
                       ? TaskHandlers.First().Execute(invoice)
                       : Attempt<IInvoice>.Fail(new InvalidOperationException("The configuration Chain Task List could not be instantiated"));

            if (!attempt.Success) return attempt;


            var charges = attempt.Result.Items.Where(x => x.LineItemType != LineItemType.Discount).Sum(x => x.TotalPrice);
            var discounts = attempt.Result.Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => x.TotalPrice);

            // total the invoice
            decimal converted;
            attempt.Result.Total = decimal.TryParse((charges - discounts).ToString(CultureInfo.InvariantCulture), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out converted) ? converted : 0;
               
            return attempt;
        }
    }
}