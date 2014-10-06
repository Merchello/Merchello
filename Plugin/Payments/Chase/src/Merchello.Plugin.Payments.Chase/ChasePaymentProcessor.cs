using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Plugin.Payments.Chase.Models;
using Paymentech;
using Umbraco.Core;

namespace Merchello.Plugin.Payments.Chase
{
    /// <summary>
    /// The Authorize.Net payment processor
    /// </summary>
    public class ChasePaymentProcessor
    {
        private readonly ChaseProcessorSettings _settings;

        public ChasePaymentProcessor(ChaseProcessorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Processes the Authorize and AuthorizeAndCapture transactions
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to be paid</param>
        /// <param name="payment">The <see cref="IPayment"/> record</param>
        /// <param name="transactionMode">Authorize or AuthorizeAndCapture</param>
        /// <param name="amount">The money amount to be processed</param>
        /// <param name="creditCard">The <see cref="CreditCardFormData"/></param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult ProcessPayment(IInvoice invoice, IPayment payment, TransactionMode transactionMode, decimal amount, CreditCardFormData creditCard)
        {
            var address = invoice.GetBillingAddress();
                                                                                
            var names = creditCard.CardholderName.Split(' ');

            // Declare a response
			Paymentech.Response response;

			// Create an authorize transaction
            var transaction = new Transaction(RequestType.NEW_ORDER_TRANSACTION);
			// Populate the required fields for the given transaction type. You can use’
			// the Paymentech Transaction Appendix to help you populate the transaction’

            transaction["OrbitalConnectionUsername"] = _settings.Username;
            transaction["OrbitalConnectionPassword"] = _settings.Password;
            /*
                * Message Types
                * MO – Mail Order transaction
                * RC – Recurring Payment
                * EC– eCommerce transaction
                * IV – IVR [PINLess Debit Only]
            */
			transaction["IndustryType"] = "EC";

            /*
                * Message Types
                * A – Authorization request
                * AC – Authorization and Mark for Capture
                * FC – Force-Capture request
                * R – Refund request
            */           
			transaction["MessageType"] = transactionMode == TransactionMode.Authorize ? "A" : "AC";

			transaction["MerchantID"] = _settings.MerchantId;
			transaction["BIN"] = _settings.Bin;
            
            // Credit Card Number
            transaction["AccountNum"] = creditCard.CardNumber;

            transaction["OrderID"] = invoice.InvoiceNumber.ToString(CultureInfo.InstalledUICulture);

			transaction["Amount"] = amount.ToString();

            // Expiration date
            var creditCardExpMonth = creditCard.ExpireMonth;
            var creditCardExpYear = creditCard.ExpireYear.Length > 2
                ? creditCard.ExpireYear.Substring(2, 2)
                : creditCard.ExpireYear;

			transaction["Exp"] = creditCardExpMonth.PadLeft(2) + creditCardExpYear;

			transaction["AVSname"] = address.Name;
			transaction["AVSaddress1"] = address.Address1;  
			transaction["AVSaddress2"] = address.Address2;
			transaction["AVScity"] = address.Locality;
			transaction["AVSstate"] = address.Region;
			transaction["AVSzip"] = address.PostalCode;
			transaction["AVScountryCode"] = address.CountryCode;
			transaction["CardSecVal"] = creditCard.CardCode;

            /*
                * CardSecValInd
                * 1 - Value is Present
                * 2 - Value on card but illegible
                * 9 - Cardholder states data not available              
            */
            transaction["CardSecValInd"] = "1";

            /*
                * CardSecValInd                               
                * A – Auto Generate the CustomerRefNum
                * S – Use CustomerRefNum Element            
            */
			transaction["CustomerProfileFromOrderInd"] = "A";

            /*
                * CustomerProfileOrderOverrideInd                   
                * NO No mapping to order data
                * OI Use <CustomerRefNum> for <OrderID> and <ECOrderNum> or <MailOrderNum>
                * OD Use <CustomerReferNum> for <Comments> 
                * OA Use <CustomerRefNum> for <OrderID> and <Comments>
            */
			transaction["CustomerProfileOrderOverrideInd"] = "NO";

            transaction["Comments"] = invoice.InvoiceNumber.ToString(CultureInfo.InstalledUICulture);

			response = transaction.Process();

            // API Error
            if (response == null)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception("Chase Paymentech unknown error")), invoice, false);
            }


            if (response.Error)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
            } 
            if (response.Declined)
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizeDeclinedResult, string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message));
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message))), invoice, false);
            }
            if (response.Approved)
            {
                var txRefIdx = "";
                if (response.XML != null)
                {
                    var xml = XDocument.Parse(response.MaskedXML);
                    txRefIdx = xml.Descendants("TxRefIdx").First().Value;
                }

                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber, response.TxRefNum);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceIndex, txRefIdx);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1}", response.AuthCode, response.ResponseCode));

             
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                payment.Authorized = true;
                if (transactionMode == TransactionMode.AuthorizeAndCapture)
                {
                    payment.Collected = true;
                }
                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
            }

            return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
		}

        /// <summary>
        /// Captures a previously authorized payment
        /// </summary>
        /// <param name="invoice">The invoice associated with the <see cref="IPayment"/></param>
        /// <param name="payment">The <see cref="IPayment"/> to capture</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult PriorAuthorizeCapturePayment(IInvoice invoice, IPayment payment)
        {   
            var address = invoice.GetBillingAddress();

            // Declare a response
            Paymentech.Response response;

            // Create an authorize transaction
            var transaction = new Transaction(RequestType.MARK_FOR_CAPTURE_TRANSACTION);

            var txRefNum = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber);


            if (!payment.Authorized || string.IsNullOrEmpty(txRefNum))
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new InvalidOperationException("Payment is not Authorized or TransactionCodes not present")), invoice, false);
            }
            transaction["OrbitalConnectionUsername"] = _settings.Username;
            transaction["OrbitalConnectionPassword"] = _settings.Password;

            transaction["MerchantID"] = _settings.MerchantId;
            transaction["BIN"] = _settings.Bin;

            transaction["OrderID"] = invoice.InvoiceNumber.ToString(CultureInfo.InstalledUICulture);
            transaction["TaxInd"] = "1";
            transaction["Tax"] = invoice.TotalTax().ToString(CultureInfo.InstalledUICulture);     
            transaction["PCOrderNum"] = invoice.InvoiceNumber.ToString(CultureInfo.InstalledUICulture);
            transaction["PCDestZip"] = address.PostalCode;
            transaction["PCDestAddress1"] = address.Address1;
            transaction["PCDestAddress2"] = address.Address2;
            transaction["PCDestCity"] = address.Locality;
            transaction["PCDestState"] = address.Region;
            transaction["TxRefNum"] = txRefNum;                

            response = transaction.Process();

            // API Error
            if (response == null)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception("Chase Paymentech unknown error")), invoice, false);
            }


            if (response.Error)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
            }
            if (response.Declined)
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizeDeclinedResult, string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message));
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message))), invoice, false);
            }
            if (response.Approved)
            {
                var txRefIdx = "";
                if (response.XML != null)
                {
                    var xml = XDocument.Parse(response.MaskedXML);
                    txRefIdx = xml.Descendants("TxRefIdx").First().Value;
                }

                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber, response.TxRefNum);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceIndex, txRefIdx);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1}", response.AuthCode, response.ResponseCode));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                
                payment.Collected = true;
                
                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
            }

            return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
        }

        /// <summary>
        /// Refunds a payment amount
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> associated with the payment</param>
        /// <param name="payment">The <see cref="IPayment"/> to be refunded</param>
        /// <param name="amount">The amount of the <see cref="IPayment"/> to be refunded</param>
        /// <returns></returns>
        public IPaymentResult RefundPayment(IInvoice invoice, IPayment payment, decimal amount)
        {

            var address = invoice.GetBillingAddress();

            // Declare a response
            Paymentech.Response response;

            // Create an authorize transaction
            var transaction = new Transaction(RequestType.NEW_ORDER_TRANSACTION);

            var txRefNum = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber);


            if (!payment.Authorized || string.IsNullOrEmpty(txRefNum))
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new InvalidOperationException("Payment is not Authorized or TransactionCodes not present")), invoice, false);
            }
            transaction["OrbitalConnectionUsername"] = _settings.Username;
            transaction["OrbitalConnectionPassword"] = _settings.Password;

            transaction["IndustryType"] = "EC";
            transaction["MessageType"] = "R";

            transaction["MerchantID"] = _settings.MerchantId;
            transaction["BIN"] = _settings.Bin;
 
            transaction["OrderID"] = invoice.InvoiceNumber.ToString(CultureInfo.InstalledUICulture);
            transaction["TxRefNum"] = txRefNum;                

            response = transaction.Process();

            // API Error
            if (response == null)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception("Chase Paymentech unknown error")), invoice, false);
            }


            if (response.Error)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
            }
            if (response.Declined)
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizeDeclinedResult, string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message));
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message))), invoice, false);
            }
            if (response.Approved)
            {
                var txRefIdx = "";
                if (response.XML != null)
                {
                    var xml = XDocument.Parse(response.MaskedXML);
                    txRefIdx = xml.Descendants("TxRefIdx").First().Value;
                }

                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber, response.TxRefNum);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceIndex, txRefIdx);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1}", response.AuthCode, response.ResponseCode));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);

                payment.Collected = false;

                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
            }

            return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
        }

        public IPaymentResult VoidPayment(IInvoice invoice, IPayment payment)
        {
            var address = invoice.GetBillingAddress();

            // Declare a response
            Paymentech.Response response;

            // Create an authorize transaction
            var transaction = new Transaction(RequestType.NEW_ORDER_TRANSACTION);

            var txRefNum = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber);


            if (!payment.Authorized || string.IsNullOrEmpty(txRefNum))
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new InvalidOperationException("Payment is not Authorized or TransactionCodes not present")), invoice, false);
            }

            transaction["OrbitalConnectionUsername"] = _settings.Username;
            transaction["OrbitalConnectionPassword"] = _settings.Password;

            transaction["IndustryType"] = "EC";
            transaction["MessageType"] = "R";

            transaction["MerchantID"] = _settings.MerchantId;
            transaction["BIN"] = _settings.Bin;

            transaction["OrderID"] = invoice.InvoiceNumber.ToString(CultureInfo.InstalledUICulture);
            transaction["TxRefNum"] = txRefNum; 

            response = transaction.Process();

            // API Error
            if (response == null)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception("Chase Paymentech unknown error")), invoice, false);
            }


            if (response.Error)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
            }
            if (response.Declined)
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizeDeclinedResult, string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message));
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message))), invoice, false);
            }
            if (response.Approved)
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber, response.TxRefNum);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1}", response.AuthCode, response.ResponseCode));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);

                payment.Collected = false;

                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
            }

            return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
        }

         
        private string GetChaseReply(NameValueCollection form)
        {
            try
            {
                var postData = form.AllKeys.Aggregate("", (current, key) => current + (key + "=" + HttpUtility.UrlEncode(form[key]) + "&")).TrimEnd('&');

                var request = (HttpWebRequest)WebRequest.Create(GetChaseUrl());
                request.Method = "POST";
                request.ContentLength = postData.Length;
                request.ContentType = "application/x-www-form-urlencoded";

                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(postData);
                }

                var response = (HttpWebResponse)request.GetResponse();

                if (response == null) throw new NullReferenceException("Gateway response was null");
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private NameValueCollection GetInitialRequestForm(string currencyCode)
        {
            return new NameValueCollection()
            {
                { "x_login", _settings.MerchantId },
                { "x_delim_data", _settings.DelimitedData.ToString().ToUpperInvariant() },
                { "x_delim_char", _settings.DelimitedChar },
                { "x_encap_char", _settings.EncapChar },
                { "x_version", _settings.ApiVersion },
                { "x_relay_response", _settings.RelayResponse.ToString().ToUpperInvariant() },
                { "x_method", _settings.Method },
                { "x_currency_code", currencyCode }
            };
        }

        /// <summary>
        /// Gets the Authorize.Net Url
        /// </summary>
        private string GetChaseUrl()
        {
            return _settings.UseSandbox
                ? "orbitalvar1.paymentech.net"
                : "orbital1.paymentech.net";
        }



        /// <summary>
        /// The Authorize.Net API version
        /// </summary>
        public static string ApiVersion
        {
            get { return "7.3.0"; }
        }
    }
}