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
            Url = "https://secure.shippingapis.com/ShippingAPI.dll";
            // Url = "http://production.shippingapis.com/ShippingAPI.dll";
        }

        public HttpRequestHandler(string url)
        {
            Url = url;
        }

        #endregion

                #region Methods
        public string Post(string body)
        {
            return Post(body, Encoding.GetEncoding("ISO-8859-1"));
        }

        public string Post(string body, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(body);
            var wr = (HttpWebRequest)WebRequest.Create(new Uri(Url));
            wr.Method = "POST";
            wr.KeepAlive = false;
            wr.UserAgent = "HGIWEB";
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.ContentLength = body.Length;
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
