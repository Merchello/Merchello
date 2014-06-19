namespace Merchello.Core.Models.MonitorModels
{
    /// <summary>
    /// Defines a PaymentResultMonitorModel
    /// </summary>
    public interface IPaymentResultMonitorModel : INotifyModel
    {
        /// <summary>
        /// Gets or sets the <see cref="IPayment"/>
        /// </summary>
        IPayment Payment { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IInvoice"/>
        /// </summary>
        IInvoice Invoice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the payment was successful
        /// </summary>
        bool PaymentSuccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the payment result approved the order creation
        /// </summary>
        bool ApproveOrderCreation { get; set; }
    }
}