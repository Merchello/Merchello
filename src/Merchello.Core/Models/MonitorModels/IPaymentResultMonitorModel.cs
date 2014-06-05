namespace Merchello.Core.Models.MonitorModels
{
    /// <summary>
    /// Defines a PaymentResultMonitorModel
    /// </summary>
    public interface IPaymentResultMonitorModel : INotifyModel
    {
        /// <summary>
        /// Gets/Sets the <see cref="IPayment"/>
        /// </summary>
        IPayment Payment { get; set; }

        /// <summary>
        /// Get/sets the <see cref="IInvoice"/>
        /// </summary>
        IInvoice Invoice { get; set; }

        /// <summary>
        /// True/false indicating whether or not the payment was successful
        /// </summary>
        bool PaymentSuccess { get; set; }

        /// <summary>
        /// True/false indicating whether or not the payment result approved the order creation
        /// </summary>
        bool ApproveOrderCreation { get; set; }
    }
}