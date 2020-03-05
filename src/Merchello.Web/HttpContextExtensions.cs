using System.Linq;
using System.Web;

namespace Merchello.Web
{
    /// <summary>
    /// Extension methods for the HttpContext class
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        ///     Gets the current users IP address
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetIpAddress(this HttpContext context)
        {
            var ipAddress = string.Empty;

            if (context != null)
            {
                var cfIp = context.Request.Headers["CF-CONNECTING-IP"];
                if (!string.IsNullOrWhiteSpace(cfIp))
                {
                    return cfIp;
                }

                ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrWhiteSpace(ipAddress))
                {
                    var addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        ipAddress = addresses[0];
                    }
                }

                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    ipAddress = context.Request.UserHostAddress;
                }

                if (ipAddress?.Count(x => x == ':') == 1)
                {
                    return ipAddress.Split(':')[0];
                }

            }

            return ipAddress;
        }
    }
}
