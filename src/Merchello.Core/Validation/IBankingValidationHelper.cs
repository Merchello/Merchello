namespace Merchello.Core.Validation
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines the BankingValidationHelper interface.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public interface IBankingValidationHelper
    {
        /// <summary>
        /// Validate an International Bank Account Number (IBAN)
        /// </summary>
        /// <param name="iban">International Bank Account Number (IBAN) to validate</param>
        /// <returns>[true|false] whether IBAN is valid or not</returns>
        /// <see>http://en.wikipedia.org/wiki/International_Bank_Account_Number</see>
        /// <see>http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2</see>
        /// <see>http://en.wikipedia.org/wiki/ISO_7064</see>
        /// <example>See http://www.tbg5-finance.org/?ibandocs.shtml</example>               
        bool IbanBanknrValid(string iban);
    }
}