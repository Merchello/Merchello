namespace Merchello.Core.Validation
{
    /// <summary>
    /// A validator for email addresses.
    /// </summary>
    public interface IEmailValidationHelper
    {
        /// <summary>
        /// Validates whether or not a string is in a valid email format.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsValidEmail(string value);
    }
}