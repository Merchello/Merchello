using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
            transaction["AccountNum"] = creditCard.CardNumber.Replace(" ", "").Replace("-", "").Replace("|", "");

            transaction["OrderID"] = invoice.InvoiceNumber.ToString(CultureInfo.InstalledUICulture);

            transaction["Amount"] = string.Format("{0:0}", amount * 100);
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

            transaction["TraceNumber"] = invoice.InvoiceNumber.ToString();

            if (string.IsNullOrEmpty(creditCard.CreditCardType))
            {
                creditCard.CreditCardType = GetCreditCardType(creditCard.CardNumber);
            }

            if (creditCard.CreditCardType.ToLower().Contains("visa") || creditCard.CreditCardType.ToLower().Contains("chase"))
            {
                transaction["CAVV"] = creditCard.AuthenticationVerification;

                // If no value for creditCard.CardCode, then CardSecValInd cannot be 1.  Send 2 or 9 instead
                if (string.IsNullOrEmpty(creditCard.CardCode))
                {
                    transaction["CardSecValInd"] = "9";
                }
                else
                {
                    transaction["CardSecValInd"] = "1";
                }
            }
            else if (creditCard.CreditCardType.ToLower().Contains("mastercard"))
            {
                transaction["AAV"] = creditCard.AuthenticationVerification;
                transaction["CardSecValInd"] = "";
            }
            transaction["AuthenticationECIInd"] = creditCard.AuthenticationVerificationEci;

            /*
                * CardSecValInd
                * 1 - Value is Present
                * 2 - Value on card but illegible
                * 9 - Cardholder states data not available              
            */
            // Only send if ChaseNet, Visa and Discover transactions
            // transaction["CardSecValInd"] = "1";

            /*
                * CardSecValInd                               
                * A – Auto Generate the CustomerRefNum
                * S – Use CustomerRefNum Element            
            */

            // profile management will not be supported, do not send CustomerProfileFromOrderInd
			//transaction["CustomerProfileFromOrderInd"] = "A";

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

            string approvalStatus = "";
            if (response.XML != null)
            {
                var xml = XDocument.Parse(response.MaskedXML);
                if (xml.Descendants("ApprovalStatus").FirstOrDefault() != null)
                {
                    approvalStatus = xml.Descendants("ApprovalStatus").First().Value;
                }
            }


            if (response.Error)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
            } 
            if (response.Declined)
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizeDeclinedResult, string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber, response.TxRefNum);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1},{2}", response.AuthCode, response.ResponseCode, approvalStatus));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.Cvv2Result, string.Format("{0},{1}", response.CVV2RespCode, response.CVV2ResponseCode));
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
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1},{2}", response.AuthCode, response.ResponseCode, approvalStatus));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.Cvv2Result, string.Format("{0},{1}", response.CVV2RespCode, response.CVV2ResponseCode));

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

            string approvalStatus = "";
            if (response.XML != null)
            {
                var xml = XDocument.Parse(response.MaskedXML);
                approvalStatus = xml.Descendants("ApprovalStatus").First().Value;
            }


            if (response.Error)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
            }
            if (response.Declined)
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizeDeclinedResult, string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber, response.TxRefNum);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1},{2}", response.AuthCode, response.ResponseCode, approvalStatus));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.Cvv2Result, string.Format("{0},{1}", response.CVV2RespCode, response.CVV2ResponseCode));

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
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1},{2}", response.AuthCode, response.ResponseCode, approvalStatus));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.Cvv2Result, string.Format("{0},{1}", response.CVV2RespCode, response.CVV2ResponseCode));
                
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
            var transaction = new Transaction(RequestType.ECOMMERCE_REFUND);

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

            transaction["Amount"] = string.Format("{0:0}", amount * 100);
            transaction["OrderID"] = invoice.InvoiceNumber.ToString(CultureInfo.InstalledUICulture);
            transaction["TxRefNum"] = txRefNum;                

            response = transaction.Process();

            // API Error
            if (response == null)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception("Chase Paymentech unknown error")), invoice, false);
            }

            string approvalStatus = "";
            if (response.XML != null)
            {
                var xml = XDocument.Parse(response.MaskedXML);
                approvalStatus = xml.Descendants("ApprovalStatus").First().Value;
            }

            if (response.Error)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
            }
            if (response.Declined)
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizeDeclinedResult, string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message)); 
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber, response.TxRefNum);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1},{2}", response.AuthCode, response.ResponseCode, approvalStatus));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.Cvv2Result, string.Format("{0},{1}", response.CVV2RespCode, response.CVV2ResponseCode));

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
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1},{2}", response.AuthCode, response.ResponseCode, approvalStatus));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.Cvv2Result, string.Format("{0},{1}", response.CVV2RespCode, response.CVV2ResponseCode));

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
            var transaction = new Transaction(RequestType.VOID);

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
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber, response.TxRefNum);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1},{2}", response.AuthCode, response.ResponseCode, "NA"));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.Cvv2Result, string.Format("{0},{1}", response.CVV2RespCode, response.CVV2ResponseCode));

                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Declined ({0} : {1})", response.ResponseCode, response.Message))), invoice, false);
            }
            if (response.Approved)
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber, response.TxRefNum);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1},{2}", response.AuthCode, response.ResponseCode, "NA"));
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, response.AVSRespCode);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.Cvv2Result, string.Format("{0},{1}", response.CVV2RespCode, response.CVV2ResponseCode));
                
                payment.Collected = false;

                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
            }

            var procStatus = "";
            if (response.XML != null)
            {
                var xml = XDocument.Parse(response.MaskedXML);
                procStatus = xml.Descendants("ProcStatus").First().Value;
            }

            if (!string.IsNullOrEmpty(procStatus) && procStatus == "0")
            {
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.VoidProcStatus, procStatus);                                                               
                
                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);  
            }
            return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Error {0}", response))), invoice, false);
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

        protected const String AMEXPattern = @"^3[47][0-9]{13}$";
        protected const String MasterCardPattern = @"^5[1-5][0-9]{14}$";
        protected const String VisaCardPattern = @"^4[0-9]{12}(?:[0-9]{3})?$";
        protected const String DinersClubCardPattern = @"^3(?:0[0-5]|[68][0-9])[0-9]{11}$";
        protected const String enRouteCardPattern = @"^(2014|2149)";
        protected const String DiscoverCardPattern = @"^6(?:011|5[0-9]{2})[0-9]{12}$";
        protected const String JCBCardPattern = @"^(?:2131|1800|35\d{3})\d{11}$";

        protected static NameValueCollection Patterns;
        protected static NameValueCollection CardPatterns
        {
            get
            {
                if (Patterns == null)
                {
                    Patterns = new NameValueCollection
                    {
                        {"AMEX", AMEXPattern},
                        {"MasterCard", MasterCardPattern},
                        {"Visa", VisaCardPattern},
                        {"DinersClub", DinersClubCardPattern},
                        {"enRoute", enRouteCardPattern},
                        {"Discover", DiscoverCardPattern},
                        {"JCB", JCBCardPattern}
                    };
                }
                return Patterns;
            }
            set
            {
                Patterns = value;
            }
        }

        protected static string GetCreditCardType(string cardNumber)
        {
            var cardType = "Unknown";

            try
            {
                var cardNum = cardNumber.Replace(" ", "").Replace("-", "").Replace("|","");
                foreach (String cardTypeName in CardPatterns.Keys)
                {
                    var regex = new Regex(CardPatterns[cardTypeName]);
                    if (regex.IsMatch(cardNum))
                    {
                        cardType = cardTypeName;
                        break;
                    }
                }
            }
            catch
            {
            }

            return cardType.ToUpper();
        }

    }
}