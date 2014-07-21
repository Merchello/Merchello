using System;
using System.IO;
using System.Net;
using System.Text;

namespace Merchello.Plugin.Shipping.USPS
{
    public class HttpRequestHandler
    {
        #region Variables

        public string Url { get; set; }
        
        #endregion

        #region Constructors
        public HttpRequestHandler()
        {
            //Url = "https://onlinetools.ups.com/ups.app/xml/Rate"; 
            //Url = "http://testing.shippingapis.com/ShippingAPITest.dll?API=RateV4";
            Url = "http://production.shippingapis.com/ShippingAPITest.dll?API=RateV4";
        }

        public HttpRequestHandler(string url)
        {
            Url = url;
        }

        #endregion

                #region Methods
        public string Post(string xmlPostData)
        {
            return Post(xmlPostData, Encoding.UTF8);
        }

        public string Post(string xmlPostData, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(xmlPostData);
            var wr = (HttpWebRequest)WebRequest.Create(new Uri(Url));
            wr.Method = "POST";
            wr.KeepAlive = false;
            wr.UserAgent = "HGIWEB";
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.ContentLength = xmlPostData.Length;
            Stream sendStream = wr.GetRequestStream();
            sendStream.Write(bytes, 0, bytes.Length);
            sendStream.Close();

            try
            {
                string reply;
                using (var response = (HttpWebResponse)wr.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    reply = reader.ReadToEnd();
                }

                return reply;
            }
            catch (Exception ex)
            {
                throw new Exception("Http Request has failed. Exception: " + ex.ToString());
            }
        }
        #endregion
    }
}
