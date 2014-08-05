namespace Merchello.Core.Models.MonitorModels
{
    /// <summary>
    /// Represents a PaymentResultMonitorModel
    /// </summary>
    internal class PaymentResultNotifyModel : NotifyModelBase, IPaymentResultMonitorModel
    {
        /// <summary>
        /// Gets or sets the <see cref="IPayment"/>
        /// </summary>
        public IPayment Payment { get; set; }
        
        /// <summary>
        /// Gets or sets the <see cref="IInvoice"/>
        /// </summary>
        public IInvoice Invoice { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether or not the payment was successful
        /// </summary>
        public bool PaymentSuccess { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether or not the payment result approved the order creation
        /// </summary>
        public bool ApproveOrderCreation { get; set; }
    }
}