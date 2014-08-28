namespace Merchello.Web.Editors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Models.SaleHistory;
    using Merchello.Web.WebApi;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The audit log api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class AuditLogApiController : MerchelloApiController
    {
        /// <summary>
        /// The audit log service.
        /// </summary>
        private readonly IAuditLogService _auditLogService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchelloHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogApiController"/> class.
        /// </summary>
        public AuditLogApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public AuditLogApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _auditLogService = merchelloContext.Services.AuditLogService;
            _merchelloHelper = new MerchelloHelper(merchelloContext.Services);
        }

        /// <summary>
        /// The get by entity key.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<AuditLogDisplay> GetByEntityKey(Guid id)
        {
            return _auditLogService.GetAuditLogsByEntityKey(id).Select(x => x.ToAuditLogDisplay());
        }

        /// <summary>
        /// Gets the sales history by invoice key.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="SalesHistoryDisplay"/>.
        /// </returns>
        public SalesHistoryDisplay GetSalesHistoryByInvoiceKey(Guid id)
        {
            var list = new List<AuditLogDisplay>();

            var invoice = _merchelloHelper.Query.Invoice.GetByKey(id);

            if (invoice != null)
            {
                var invoiceLogs = _auditLogService.GetAuditLogsByEntityKey(invoice.Key).Select(x => x.ToAuditLogDisplay()).ToArray();

                if (invoiceLogs.Any()) list.AddRange(invoiceLogs);

                foreach (var orderLogs in invoice.Orders.Select(order => this._auditLogService.GetAuditLogsByEntityKey(order.Key).Select(x => x.ToAuditLogDisplay()).ToArray()).Where(orderLogs => orderLogs.Any()))
                {
                    list.AddRange(orderLogs);
                }

                var paymentKeys = MerchelloContext.Services.PaymentService.GetPaymentsByInvoiceKey(invoice.Key).Select(x => x.Key);

                foreach (var paymentLogs in paymentKeys.Select(x => _auditLogService.GetAuditLogsByEntityKey(x).Select(log => log.ToAuditLogDisplay())).ToArray())
                {
                    list.AddRange(paymentLogs);
                }
            }
            return list.ToSalesHistoryDisplay();
        }
    }
}