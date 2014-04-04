using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Plugin.Payments.AuthorizeNet.Models;
using Umbraco.Core;

namespace Merchello.Plugin.Payments.AuthorizeNet
{
    /// <summary>
    /// The Authorize.Net payment processor
    /// </summary>
    public class AuthorizeNetPaymentProcessor
    {
        private readonly AuthorizeNetProcessorSettings _settings;

        public AuthorizeNetPaymentProcessor(AuthorizeNetProcessorSettings settings)
        {
            _settings = settings;
        }


        public IPaymentResult ProcessPayment(IInvoice invoice, IPayment payment, TransactionMode transactionMode, decimal amount, CreditCardFormData creditCard)
        {
            var address = invoice.GetBillingAddress();
            var customerKey = invoice.CustomerKey;

            var form = GetInitialRequestForm(invoice.CurrencyCode());

            var names = creditCard.CardholderName.Split(' ');

            form.Add("x_type", 
                    transactionMode == TransactionMode.Authorize ?
                    "AUTH_ONLY" : "AUTH_CAPTURE"
                );
           
            // Credit card information            
            form.Add("x_card_num", creditCard.CardNumber);
            form.Add("x_exp_date", creditCard.ExpireMonth.PadLeft(2) + creditCard.ExpireYear);
            form.Add("x_card_code", creditCard.CardCode);
            form.Add("x_customer_ip", creditCard.CustomerIp);

            // Billing address
            form.Add("x_first_name", names.Count() > 1 ? names[0] : creditCard.CardholderName);
            form.Add("x_last_name", names.Count() > 1 ? names[1] : string.Empty);
            form.Add("x_email", address.Email);
            if(!string.IsNullOrEmpty(address.Organization)) form.Add("x_company", address.Organization);
            form.Add("x_address", address.Address1);
            form.Add("x_city", address.Locality);
            if(!string.IsNullOrEmpty(address.Region)) form.Add("x_state", address.Region);
            form.Add("x_zip", address.PostalCode);
            if(!string.IsNullOrEmpty(address.CountryCode)) form.Add("x_country", address.CountryCode);

            // Invoice information
            form.Add("x_amount", amount.ToString("0.00", CultureInfo.InstalledUICulture));
            form.Add("x_invoice_num", invoice.PrefixedInvoiceNumber());
            form.Add("x_description", string.Format("Full invoice #{0}", invoice.PrefixedInvoiceNumber()));

            var reply = GetAuthorizeNetReply(form);

            // API Error
            if(string.IsNullOrEmpty(reply)) return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception("Authorize.NET unknown error")), invoice, false);

            var fields = reply.Split('|');

            switch (fields[0])
            {
                case "3" :
                    return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(reply)), invoice, false);
                case "2" :                    
                    payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.DeclinedResult, string.Format("Payment Declined ({0} : {1})", fields[2], fields[3]));
                    return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(string.Format("Payment Declined ({0} : {1})", fields[2], fields[3]))), invoice, false);
                case "1" :
                    payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode, string.Format("{0},{1}", fields[6], fields[4]));
                    payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationTransactionResult, string.Format("Approved ({0}: {1})", fields[2], fields[3]));
                    payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AvsResult, fields[5]);
                    payment.Authorized = true;
                    if (transactionMode == TransactionMode.AuthorizeAndCapture) payment.Collected = true;                    
                    return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
            }

            return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception("Authorize.NET unknown error")), invoice, false);
        }

        public IPaymentResult PriorAuthorizeCapturePayment(IPayment payment, ProcessorArgumentCollection args)
        {
            throw new NotImplementedException();
        }


        public IPaymentResult RefundPayment(IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            throw new NotImplementedException();
        }

        public IPaymentResult VoidPayment(IPayment payment)
        {
            throw new NotImplementedException();
        }

        private string GetAuthorizeNetReply(NameValueCollection form)
        {
            
            var postData = form.AllKeys.Aggregate("", (current, key) => current + (key + "=" + HttpUtility.UrlEncode(form[key]) + "&")).TrimEnd('&');

            var request = (HttpWebRequest) WebRequest.Create(GetAuthorizeNetUrl());
            request.Method = "POST";
            request.ContentLength = postData.Length;
            request.ContentType = "application/x-www-form-urlencoded";

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postData);   
            }

            var response = (HttpWebResponse) request.GetResponse();
           
            if(response == null) throw  new NullReferenceException("Gateway response was null");
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private NameValueCollection GetInitialRequestForm(string currencyCode)
        {
            return new NameValueCollection()
            {
                { "x_login", _settings.LoginId },
                { "x_tran_key", _settings.TransactionKey },
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
        private string GetAuthorizeNetUrl()
        {
            return _settings.UseSandbox
                ? "https://test.authorize.net/gateway/transact.dll"
                : "https://secure.authorize.net/gateway/transact.dll";
        }



        /// <summary>
        /// The Authorize.Net API version
        /// </summary>
        public static string ApiVersion
        {
            get { return "3.1"; }
        }
    }
}