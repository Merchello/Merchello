namespace Merchello.Providers.Payment.PayPal.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Events;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Providers.Exceptions;
    using Merchello.Providers.Payment.PayPal.Factories;
    using Merchello.Providers.Payment.PayPal.Models;

    using global::PayPal.PayPalAPIInterfaceService;

    using global::PayPal.PayPalAPIInterfaceService.Model;


    using Umbraco.Core.Events;

    /// <summary>
    /// Represents a PayPalExpressCheckoutService.
    /// </summary>
    public class PayPalExpressCheckoutService : PayPalApiServiceBase, IPayPalExpressCheckoutService
    {
        /// <summary>
        /// The default return URL.
        /// </summary>
        private const string DefaultReturnUrl = "{0}/umbraco/merchello/paypalexpress/success?invoiceKey={1}&paymentKey={2}";

        /// <summary>
        /// The default cancel URL.
        /// </summary>
        private const string DefaultCancelUrl = "{0}/umbraco/merchello/paypalexpress/success?invoiceKey={1}&paymentKey={2}";

        /// <summary>
        /// The version.
        /// </summary>
        private const string Version = "104.0";

        /// <summary>
        /// The Website URL.
        /// </summary>
        private readonly string _websiteUrl;

        /// <summary>
        /// The <see cref="ExpressCheckoutResponseFactory"/>.
        /// </summary>
        private readonly ExpressCheckoutResponseFactory _responseFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalExpressCheckoutService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PayPalExpressCheckoutService(PayPalProviderSettings settings)
            : base(settings)
        {
            _websiteUrl = PayPalApiHelper.GetBaseWebsiteUrl();
            _responseFactory = new ExpressCheckoutResponseFactory();
        }

        /// <summary>
        /// Occurs before PayPalExpressCheckoutService is completely initialized.
        /// </summary>
        /// <remarks>
        /// Allows for overriding defaults
        /// </remarks>
        public static event TypedEventHandler<PayPalExpressCheckoutService, ObjectEventArgs<PayPalExpressCheckoutUrls>> InitializingExpressCheckout;

        /// <summary>
        /// Occurs before adding the PayPal Express Checkout details to the request
        /// </summary>
        public static event TypedEventHandler<IPayPalExpressCheckoutService, ObjectEventArgs<PayPalExpressCheckoutRequestDetailsOverride>> SettingCheckoutRequestDetails;

        ///// <summary>
        ///// Occurs before adding the PayPal Express Checkout Details to the DoExpressCheckoutExpress.
        ///// </summary>
        //public static event TypedEventHandler<IPayPalApiPaymentService, ObjectEventArgs<PayPalExpressCheckoutRequestDetailsOverride>> SettingDoExpressCheckoutRequestDetails;

        /// <summary>
        /// Performs the setup for an express checkout.
        /// </summary>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/>.
        /// </param>
        /// <param name="payment">
        /// The <see cref="IPayment"/>
        /// </param>
        /// <returns>
        /// The <see cref="SetExpressCheckoutResponseType"/>.
        /// </returns>
        public PayPalExpressTransactionRecord SetExpressCheckout(IInvoice invoice, IPayment payment)
        {
            var urls = new PayPalExpressCheckoutUrls
                {
                    ReturnUrl = string.Format(DefaultReturnUrl, _websiteUrl, invoice.Key, payment.Key),
                    CancelUrl = string.Format(DefaultCancelUrl, _websiteUrl, invoice.Key, payment.Key)
                };

            // Raise the event so that the urls can be overridden
            InitializingExpressCheckout.RaiseEvent(new ObjectEventArgs<PayPalExpressCheckoutUrls>(urls), this);
            return SetExpressCheckout(invoice, payment, urls.ReturnUrl, urls.CancelUrl);
        }


        /// <summary>
        /// The capture success.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="isPartialPayment">
        /// The is partial payment.
        /// </param>
        /// <returns>
        /// The <see cref="ExpressCheckoutResponse"/>.
        /// </returns>
        public PayPalExpressTransactionRecord Capture(IInvoice invoice, IPayment payment, decimal amount, bool isPartialPayment)
        {
            // Get the transaction record
            var record = payment.GetPayPalTransactionRecord();

            ExpressCheckoutResponse result = null;

            try
            {
                var amountFactory = new PayPalBasicAmountTypeFactory(PayPalApiHelper.GetPayPalCurrencyCode(invoice.CurrencyCode));

                var authorizationId = record.Data.AuthorizationTransactionId;

                // do express checkout
                var request = new DoCaptureRequestType()
                {
                    AuthorizationID = authorizationId,
                    Amount = amountFactory.Build(amount),
                    CompleteType = isPartialPayment ? CompleteCodeType.NOTCOMPLETE : CompleteCodeType.COMPLETE
                };

                var doCaptureReq = new DoCaptureReq() { DoCaptureRequest = request };

                var service = GetPayPalService();
                var doCaptureResponse = service.DoCapture(doCaptureReq);
                result = _responseFactory.Build(doCaptureResponse, record.Data.Token);

                if (result.Success())
                {
                    var transactionId = doCaptureResponse.DoCaptureResponseDetails.PaymentInfo.TransactionID;
                    record.Data.CaptureTransactionId = transactionId;
                }
            }
            catch (Exception ex)
            {
                result = _responseFactory.Build(ex);
            }

            record.DoCapture = result;
            record.Success = result.Success();
            return record;
        }

        /// <summary>
        /// Refunds or partially refunds a payment.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="amount">
        /// The amount of the refund.
        /// </param>
        /// <returns>
        /// The <see cref="PayPalExpressTransactionRecord"/>.
        /// </returns>
        public ExpressCheckoutResponse Refund(IInvoice invoice, IPayment payment, decimal amount)
        {
            var record = payment.GetPayPalTransactionRecord();

            // Ensure the currency code
            if (record.Data.CurrencyCode.IsNullOrWhiteSpace())
            {
                var ex = new PayPalApiException("CurrencyCode was not found in payment extended data PayPal transaction data record.  Cannot perform refund.");
                return _responseFactory.Build(ex);
            }

            // Ensure the transaction id
            if (record.Data.CaptureTransactionId.IsNullOrWhiteSpace())
            {
                var ex = new PayPalApiException("CaptureTransactionId was not found in payment extended data PayPal transaction data record.  Cannot perform refund.");
                return _responseFactory.Build(ex);
            }

            // Get the decimal configuration for the current currency
            var currencyCodeType = PayPalApiHelper.GetPayPalCurrencyCode(record.Data.CurrencyCode);
            var basicAmountFactory = new PayPalBasicAmountTypeFactory(currencyCodeType);

            ExpressCheckoutResponse result = null;

            if (amount > payment.Amount) amount = payment.Amount;

            try
            {
                var request = new RefundTransactionRequestType
                    {
                        InvoiceID = invoice.PrefixedInvoiceNumber(),
                        PayerID = record.Data.PayerId,
                        RefundSource = RefundSourceCodeType.DEFAULT,
                        Version = record.DoCapture.Version,
                        TransactionID = record.Data.CaptureTransactionId,
                        Amount = basicAmountFactory.Build(amount)
                    };

                var wrapper = new RefundTransactionReq { RefundTransactionRequest = request };

                var refundTransactionResponse = GetPayPalService().RefundTransaction(wrapper);

                result = _responseFactory.Build(refundTransactionResponse, record.Data.Token);
            }
            catch (Exception ex)
            {
                result = _responseFactory.Build(ex);
            }

            return result;
        }

        /// <summary>
        /// Performs the GetExpressCheckoutDetails operation.
        /// </summary>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// The <see cref="PayPalExpressTransactionRecord"/>.
        /// </returns>
        internal PayPalExpressTransactionRecord GetExpressCheckoutDetails(IPayment payment, string token, PayPalExpressTransactionRecord record)
        {
            record.Success = true;
            ExpressCheckoutResponse result = null;
            try
            {
                var getDetailsRequest = new GetExpressCheckoutDetailsReq
                {
                    GetExpressCheckoutDetailsRequest = new GetExpressCheckoutDetailsRequestType(token)
                };

                var service = GetPayPalService();
                var response = service.GetExpressCheckoutDetails(getDetailsRequest);
                result = _responseFactory.Build(response, token);

                if (result.Success())
                {
                    record.Data.PayerId = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.PayerID;
                }
            }
            catch (Exception ex)
            {
                result = _responseFactory.Build(ex);
            }

            record.GetExpressCheckoutDetails = result;
            record.Success = result.Success();
            return record;
        }

        /// <summary>
        /// The do express checkout payment.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="payerId">
        /// The payer id.
        /// </param>
        /// <param name="record">
        /// The record of the transaction.
        /// </param>
        /// <returns>
        /// The <see cref="PayPalExpressTransactionRecord"/>.
        /// </returns>
        internal PayPalExpressTransactionRecord DoExpressCheckoutPayment(IInvoice invoice, IPayment payment, string token, string payerId, PayPalExpressTransactionRecord record)
        {
            var factory = new PayPalPaymentDetailsTypeFactory();

            ExpressCheckoutResponse result = null;
            try
            {
                // do express checkout
                var request = new DoExpressCheckoutPaymentRequestType(
                        new DoExpressCheckoutPaymentRequestDetailsType
                        {
                            Token = token,
                            PayerID = payerId,
                            PaymentDetails =
                                    new List<PaymentDetailsType>
                                        {
                                            factory.Build(invoice, PaymentActionCodeType.ORDER)
                                        }
                        });

                var doExpressCheckoutPayment = new DoExpressCheckoutPaymentReq
                {
                    DoExpressCheckoutPaymentRequest = request
                };

                var service = GetPayPalService();
                var response = service.DoExpressCheckoutPayment(doExpressCheckoutPayment);
                result = _responseFactory.Build(response, token);

                var transactionId = response.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID;
                var currency = response.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].GrossAmount.currencyID;
                var amount = response.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].GrossAmount.value;

                record.Data.CheckoutPaymentTransactionId = transactionId;
                record.Data.CurrencyId = currency.ToString();
                record.Data.AuthorizedAmount = amount;

            }
            catch (Exception ex)
            {
                result = _responseFactory.Build(ex);
            }

            record.DoExpressCheckoutPayment = result;
            record.Success = result.Success();

            return record;
        }

        /// <summary>
        /// Validates a successful response.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="payerId">
        /// The payer id.
        /// </param>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// The <see cref="ExpressCheckoutResponse"/>.
        /// </returns>
        /// <remarks>
        /// PayPal returns to the success URL even if the payment was declined. e.g.  The success URL represents a successful transaction
        /// not a successful payment so we need to do another request to verify the payment was completed and get additional information
        /// such as the transaction id so we can do refunds etc.
        /// </remarks>
        internal PayPalExpressTransactionRecord Authorize(IInvoice invoice, IPayment payment, string token, string payerId, PayPalExpressTransactionRecord record)
        {
            // Now we have to get the transaction details for the successful payment
            ExpressCheckoutResponse result = null;
            try
            {
                // do authorization
                var service = GetPayPalService();
                var doAuthorizationResponse = service.DoAuthorization(new DoAuthorizationReq
                {
                    DoAuthorizationRequest = new DoAuthorizationRequestType
                    {
                        TransactionID = record.Data.CheckoutPaymentTransactionId,
                        Amount = new BasicAmountType(PayPalApiHelper.GetPayPalCurrencyCode(record.Data.CurrencyId), record.Data.AuthorizedAmount)
                    }
                });

                result = _responseFactory.Build(doAuthorizationResponse, token);
                if (result.Success())
                {
                    record.Data.Authorized = true;
                    record.Data.AuthorizationTransactionId = doAuthorizationResponse.TransactionID;
                }
            }
            catch (Exception ex)
            {
                result = _responseFactory.Build(ex);
            }

            record.DoAuthorization = result;

            return record;
        }



        /// <summary>
        /// Performs the setup for an express checkout.
        /// </summary>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/>.
        /// </param>
        /// <param name="payment">
        /// The <see cref="IPayment"/>
        /// </param>
        /// <param name="returnUrl">
        /// The return URL.
        /// </param>
        /// <param name="cancelUrl">
        /// The cancel URL.
        /// </param>
        /// <returns>
        /// The <see cref="ExpressCheckoutResponse"/>.
        /// </returns>
        protected virtual PayPalExpressTransactionRecord SetExpressCheckout(IInvoice invoice, IPayment payment, string returnUrl, string cancelUrl)
        {
            var record = new PayPalExpressTransactionRecord
                             {
                                 Success = true,
                                 Data = { Authorized = false, CurrencyCode = invoice.CurrencyCode }
                             };

            var factory = new PayPalPaymentDetailsTypeFactory(new PayPalFactorySettings { WebsiteUrl = _websiteUrl });
            var paymentDetailsType = factory.Build(invoice, PaymentActionCodeType.ORDER);

            // The API requires this be in a list
            var paymentDetailsList = new List<PaymentDetailsType>() { paymentDetailsType };

            // ExpressCheckout details
            var ecDetails = new SetExpressCheckoutRequestDetailsType()
                    {
                        ReturnURL = returnUrl,
                        CancelURL = cancelUrl,
                        PaymentDetails = paymentDetailsList,
                        AddressOverride = "1"
                    };

            // Trigger the event to allow for overriding ecDetails
            var ecdOverride = new PayPalExpressCheckoutRequestDetailsOverride(invoice, payment, ecDetails);
            SettingCheckoutRequestDetails.RaiseEvent(new ObjectEventArgs<PayPalExpressCheckoutRequestDetailsOverride>(ecdOverride), this);

            // The ExpressCheckoutRequest
            var request = new SetExpressCheckoutRequestType
                    {
                        Version = Version,
                        SetExpressCheckoutRequestDetails = ecdOverride.ExpressCheckoutDetails
                    };

            // Crete the wrapper for Express Checkout
            var wrapper = new SetExpressCheckoutReq
                              {
                                  SetExpressCheckoutRequest = request
                              };

            try
            {
                var service = GetPayPalService();
                var response = service.SetExpressCheckout(wrapper);

                record.SetExpressCheckout = _responseFactory.Build(response, response.Token);
                if (record.SetExpressCheckout.Success())
                {
                    record.Data.Token = response.Token;
                    record.SetExpressCheckout.RedirectUrl = GetRedirectUrl(response.Token);
                }
                else
                {
                    foreach (var et in record.SetExpressCheckout.ErrorTypes)
                    {
                        var code = et.ErrorCode;
                        var sm = et.ShortMessage;
                        var lm = et.LongMessage;
                        MultiLogHelper.Warn<PayPalExpressCheckoutService>(string.Format("{0} {1} {2}", code, lm, sm));
                    }

                    record.Success = false;
                }
            }
            catch (Exception ex)
            {
                record.Success = false;
                record.SetExpressCheckout = _responseFactory.Build(ex);
            }

            return record;
        }
        
        /// <summary>
        /// Gets the <see cref="PayPalAPIInterfaceServiceService"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="PayPalAPIInterfaceServiceService"/>.
        /// </returns>
        /// <exception cref="PayPalApiException">
        /// Throws an exception if 
        /// </exception>
        protected virtual PayPalAPIInterfaceServiceService GetPayPalService()
        {
            // We are getting the SDK authentication values from the settings saved through the back office
            // and stored in ExtendedData.  This returns an Attempt<PayPalSettings> so that we can 
            // assert that the values have been entered into the back office.
            var attemptSdk = Settings.GetExpressCheckoutSdkConfig();
            if (!attemptSdk.Success) throw attemptSdk.Exception;

            EnsureSslTslChannel();
            return new PayPalAPIInterfaceServiceService(attemptSdk.Result);
        }

        /// <summary>
        /// Gets the redirection URL for PayPal.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <remarks>
        /// This value will be put into the <see cref="IPayment"/>'s <see cref="ExtendedDataCollection"/>
        /// </remarks>
        private string GetRedirectUrl(string token)
        {
            return string.Format("https://www.{0}paypal.com/cgi-bin/webscr?cmd=_express-checkout&token={1}", Settings.Mode == PayPalMode.Live ? string.Empty : "sandbox.", token);
        }
    }
}