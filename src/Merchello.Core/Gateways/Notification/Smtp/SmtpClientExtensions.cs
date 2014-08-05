namespace Merchello.Core.Gateways.Notification.Smtp
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;

    /// <summary>
    /// SMTP Client extension methods
    /// </summary>
    internal static class SmtpClientExtensions
    {
        /// <summary>
        /// Return the Task for sending a <see cref="MailMessage"/> asynchronously
        /// </summary>
        /// <param name="client">The <see cref="SmtpClient"/> object to extend</param>
        /// <param name="message">The <see cref="MailMessage"/> to send</param>
        /// <returns>The <see cref="Task"/></returns>
        /// <remarks>
        /// http://stackoverflow.com/questions/15140308/how-to-send-mails-asynchronous/
        /// http://stackoverflow.com/questions/3408397/asynchronously-sending-emails-in-c
        /// </remarks>
        public static Task SendAsync(this SmtpClient client, MailMessage message)
        {
            var tcs = new TaskCompletionSource<object>();
            var sendGuid = Guid.NewGuid();

            SendCompletedEventHandler handler = null;

            handler = (o, ea) =>
            {
                if (!(ea.UserState is Guid) || ((Guid) ea.UserState) != sendGuid) return;

                client.SendCompleted -= handler;
                if (ea.Cancelled)
                {
                    tcs.SetCanceled();
                }
                else if (ea.Error != null)
                {
                    tcs.SetException(ea.Error);
                }
                else
                {
                    tcs.SetResult(null);
                }
            };

            client.SendCompleted += handler;
            client.SendAsync(message, sendGuid);
            return tcs.Task;
        }
    }
}