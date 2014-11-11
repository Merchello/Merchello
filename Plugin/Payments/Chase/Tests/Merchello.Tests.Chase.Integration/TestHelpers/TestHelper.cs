using System;
using System.Configuration;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Plugin.Payments.Chase;
using Merchello.Plugin.Payments.Chase.Models;

namespace Merchello.Tests.Chase.Integration.TestHelpers
{
    public class TestHelper
    {
        public static ChaseProcessorSettings GetChaseProviderSettings()
        {
            return new ChaseProcessorSettings()
            {
                MerchantId = ConfigurationManager.AppSettings["merchantId"],
                Bin = ConfigurationManager.AppSettings["bin"],
                Username = ConfigurationManager.AppSettings["username"],
                Password = ConfigurationManager.AppSettings["password"],
            };
        }

        public static string PaymentMethodNonce
        {
            get
            {
                return "nonce-from-the-client";
            }
        }

        public static void LogInformation(string name, IPaymentResult result)
        {
            var txRefNum = result.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.TransactionReferenceNumber);
            var authTransacCode = result.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode);
            var avsResult = result.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.AvsResult);
            var cvv2Result = result.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.Cvv2Result);
            
            Console.WriteLine("TxRefNum:{0}", txRefNum);
            Console.WriteLine("Auth Code:{0}", authTransacCode.Split(',')[0]);
            Console.WriteLine("Response Code:{0}", authTransacCode.Split(',')[1]);
            Console.WriteLine("Approval Status:{0}", authTransacCode.Split(',')[2]);
            Console.WriteLine("avsResult:{0}", avsResult);
            Console.WriteLine("CVV2 Response:{0}", cvv2Result);

            var responseInfo = string.Format("{0}, TxRefNum:{1}, AuthCode:{2}, ResponseCode:{3}, AvsResult:{4}, Cvv2Result:{5}, ApprovalStatus:{6}", name, txRefNum, authTransacCode.Split(',')[0], authTransacCode.Split(',')[1], avsResult, cvv2Result, authTransacCode.Split(',')[2]);

            using (var file = new System.IO.StreamWriter("Certification.csv", true))
            {
                logging.LogMessageToFile(responseInfo,"certification");
                //file.WriteLine(responseInfo);
            }
        }

        public static CreditCardFormData GetCreditCardFormData(string cardType, string card, string cardCode)
        {
            return new CreditCardFormData()
            {
                CreditCardType = cardType,
                CardholderName = "Test User",
                CardNumber = card,
                CardCode = cardCode,
                ExpireMonth = "09",
                ExpireYear = "20",
                CustomerIp = "10.0.0.15"
            };
        }
    }
}