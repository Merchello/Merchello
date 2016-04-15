namespace Merchello.Core.Validation
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using Umbraco.Core.Logging;

    /// <summary>
    /// A validator for email addresses.
    /// </summary>
    internal class EmailValidationHelper : IEmailValidationHelper
    {
        /// <summary>
        /// A value indicating whether or not the current email is valid.
        /// </summary>
        private bool invalid = false;

        /// <summary>
        /// Validates whether or not a string is in a valid email format.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsValidEmail(string value)
        {
            this.invalid = false;
            if (string.IsNullOrEmpty(value))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                value = Regex.Replace(value, @"(@)(.+)$", this.DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (this.invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(
                      value,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase,
                      TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Assists in domain mapping.
        /// </summary>
        /// <param name="match">
        /// The match.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            var idn = new IdnMapping();

            var domainName = match.Groups[2].Value;

            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                LogHelper.Debug<EmailValidationHelper>("Invalid email address");
                this.invalid = true;
            }

            return match.Groups[1].Value + domainName;
        }
    }
}