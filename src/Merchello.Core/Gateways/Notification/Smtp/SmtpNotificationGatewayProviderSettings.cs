namespace Merchello.Core.Gateways.Notification.Smtp
{
    using System.Net;

    /// <summary>
    /// Represents SMTP Notification Gateway Provider Settings
    /// </summary>
    public class SmtpNotificationGatewayProviderSettings
    {
        /// <summary>
        /// The credentials.
        /// </summary>
        private NetworkCredential _credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpNotificationGatewayProviderSettings"/> class.
        /// </summary>
        public SmtpNotificationGatewayProviderSettings()
            : this("127.0.0.1")
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpNotificationGatewayProviderSettings"/> class.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        public SmtpNotificationGatewayProviderSettings(string host)
        {
            Mandate.ParameterNotNullOrEmpty(host, "host");
            Host = host;
            Port = 25;
        }

        /// <summary>
        /// Gets or sets the SMTP Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to enable SSL
        /// </summary>
        public bool EnableSsl { get; set; }
        
        /// <summary>
        /// Gets or sets the port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not credentials are present
        /// </summary>
        public virtual bool HasCredentials
        {
            get { return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password); }
        }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        public virtual NetworkCredential Credentials
        {
            get
            {
                if (!HasCredentials) return null;
                return _credentials ?? (_credentials = new NetworkCredential(UserName, Password));
            }
        }
    }
}