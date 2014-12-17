namespace Merchello.Plugin.Payments.Epay.Services.Interfaces
{
    using System;

    /// <summary>
    /// Defines a EpayPaymentRequestApiService.
    /// </summary>
    public interface IEpayPaymentRequestApiService
    {
        void CreatePaymentRequest(string reference, DateTime? exactClosingDate = null, int closeAfterXPayments = 1);
    }
}