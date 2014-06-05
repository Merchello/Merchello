namespace Merchello.Core.Models.MonitorModels
{
    /// <summary>
    /// Represents a PaymentResultMonitorModel
    /// </summary>
    internal class PaymentResultMonitorModel : MonitorModelBase, IPaymentResultMonitorModel
    {
        /// <summary>
        /// Gets/Sets the <see cref="IPayment"/>
        /// </summary>
        public IPayment Payment { get; set; }
        
        /// <summary>
        /// Gets/sets the <see cref="IInvoice"/>
        /// </summary>
        public IInvoice Invoice { get; set; }
        
        /// <summary>
        /// True/false indicating whether or not the payment was successful
        /// </summary>
        public bool PaymentSuccess { get; set; }
        
        /// <summary>
        /// True/false indicating whether or not the payment result approved the order creation
        /// </summary>
        public bool ApproveOrderCreation { get; set; }
    }
}