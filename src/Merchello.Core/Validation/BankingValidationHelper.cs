namespace Merchello.Core.Validation
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents BankingValidationHelper.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    internal class BankingValidationHelper : IBankingValidationHelper
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
        public bool IbanBanknrValid(string iban)
        {
            bool ibanValid = false;

            ////IBAN consists of 2 letters for countrycode (ISO 3166-1 alpha-2). For example `US`, `GB`, `DK`, or `NL`
            ////Then 2 check digits
            ////Followed by the Basic Bank Account Number (BBAN) up to 30 alphanumeric characters, country-specific!

            ////From Wikipedia: "Permitted IBAN characters are the digits 0 to 9 and the 26 upper-case Latin alphabetic characters A to Z.
            ////This applies even in countries (e.g., Thailand) where these characters are not used in the national language."

            ////The IBAN should not contain spaces when transmitted electronically.
            ////When printed it is expressed in groups of four characters separated by a single space,
            ////the last group being of variable length as shown in the example below:
            ////Country 	        IBAN formatting example
            ////Greece 	        GR16 0110 1250 0000 0001 2300 695
            ////United Kingdom    GB29 NWBK 6016 1331 9268 19
            ////Saudi Arabia      SA03 8000 0000 6080 1016 7519
            ////Switzerland       CH93 0076 2011 6238 5295 7

            if ((!string.IsNullOrEmpty(iban)) && (System.Text.RegularExpressions.Regex.IsMatch(iban, @"[A-Z]{2}[0-9]{2}\s?[A-Za-z0-9 ]{11,45}")))
            {
                ////Ok, so there is a IBAN (!string.IsNullOrEmpty) and it validates initial `quick Regex test`
                ////Each country has its own length for the BBAN number (the part after the 2 country chars and 2 check digits)
                ////Norway [NO] has the shortest IBAN with a total of 15 (for example `NO 93 86011117947`)
                ////Malta  [MT] has the longest  IBAN with a total of 31 (for example `MT 84 MALT011000012345MTLCAST001S`)
                ////Netherlands [NL] has an IBAN with a total of 18 where the 4 alphas after the checkdigits identify the bank
                ////(for example `NL 91 ABNA 0417 1643 00` where the `ABNA`-part stands for the ABN Amro Bank)

                var countryLength = new Dictionary<string, int> 
                {
                    { "AD", 24 }, { "AE", 23 }, { "AL", 28 }, { "AT", 20 }, { "AZ", 28 }, { "BA", 20 }, { "BE", 16 }, { "BG", 22 }, { "BH", 22 }, { "BR", 29 }, { "CH", 21 }, { "CR", 21 }, { "CY", 28 }, { "CZ", 24 }, { "DE", 22 }, { "DK", 18 }, { "DO", 28 }, { "EE", 20 }, { "ES", 24 }, { "FI", 18 }, { "FO", 18 }, { "FR", 27 }, { "GB", 22 }, { "GE", 22 }, { "GI", 23 }, { "GL", 18 }, { "GR", 27 }, { "GT", 28 }, { "HR", 21 }, { "HU", 28 }, { "IE", 22 }, { "IL", 23 }, { "IS", 26 }, { "IT", 27 }, { "JO", 30 }, { "KW", 30 }, { "KZ", 20 }, { "LB", 28 }, { "LI", 21 }, { "LT", 20 }, { "LU", 20 }, { "LV", 21 }, { "MC", 27 }, { "MD", 24 }, { "ME", 22 }, { "MK", 19 }, { "MR", 27 }, { "MT", 31 }, { "MU", 30 }, { "NL", 18 }, { "NO", 15 }, { "PK", 24 }, { "PL", 28 }, { "PS", 29 }, { "PT", 25 }, { "QA", 29 }, { "RO", 24 }, { "RS", 22 }, { "SA", 24 }, { "SE", 24 }, { "SI", 19 }, { "SK", 24 }, { "SM", 27 }, { "TN", 24 }, { "TR", 26 }, { "VG", 24 }
                    //, { "TL", 23 }, //Timor-Leste
                    //,{ "XK", 20 } The code XK is being used by the European Commission, Switzerland, the Deutsche Bundesbank, SWIFT and other organizations as a temporary country code for Kosovo.
                };

                if (countryLength.ContainsKey(iban.Substring(0, 2)))
                {   
                    ////IBAN starts with existing, valid (ISO 3166-1 alpha-2) countrycode

                    var alphaNumIban = System.Text.RegularExpressions.Regex.Replace(iban, "[^A-Za-z0-9]", string.Empty);
                    if (alphaNumIban.Length == countryLength[iban.Substring(0, 2)])
                    { 
                        ////The length of the IBAN is valid (corresponding to the length set for the country!)
                        ////So now, validate the BBAN part with the check digits

                        ////From Wikipedia; "Validating the IBAN"
                        ////An IBAN is validated by converting it into an integer and performing a basic mod-97 operation (as described in ISO 7064) on it.
                        ////If the IBAN is valid, the remainder equals 1.[Note 1]
                        ////The algorithm of IBAN validation is as follows:
                        ////Check that the total IBAN length is correct as per the country. If not, the IBAN is invalid (done)
                        ////Move the four initial characters to the end of the string
                        ////Replace each letter in the string with two digits, thereby expanding the string, where A = 10, B = 11, ..., Z = 35
                        ////Interpret the string as a decimal integer and compute the remainder of that number on division by 97
                        ////If the remainder is 1, the check digit test is passed and the IBAN might be valid.

                        ////Example (fictitious United Kingdom bank, sort code 12-34-56, account number 98765432):
                        ////  (0) IBAN:                   GB 82 WEST  1234 5698 7654 32
                        ////  (1) Rearrange:              W  E  S  T  1234 5698 7654 32 G  B  82
                        ////  (2) Convert to integer:     32 14 28 29 1234 5698 7654 32 16 11 82
                        ////  (3) Compute remainder:      32 14 28 29 1234 5698 7654 32 16 11 82 	mod 97 == 1 !

                        ////Step (1) `Rearrange` - move country and check digits (first 4) to the back
                        iban = (alphaNumIban.Substring(4) + alphaNumIban.Substring(0, 4)).ToUpper();

                        ////Step (2) `Convert to integer`- replace each letter in the string with two digits, where A = 10, B = 11, ..., Z = 35
                        ////Possible approach, first string to: byte[] asciiBytes = Encoding.ASCII.GetBytes(iban);
                        for (int i = 0; i < iban.Length; i++)
                        {
                            ////A-Z! Translate!
                            if ((int)iban[i] >= 65)
                            {                                
                                if (i == 0)
                                {   
                                    ////First kar needs to be `translated`
                                    iban = ((int)iban[i] - 55) + iban.Substring(1); // 1?
                                }
                                else
                                {
                                    iban = iban.Substring(0, i) + ((int)iban[i] - 55) + iban.Substring(i + 1);
                                }
                            }
                        }

                        ////Step (3) `Compute remainder` - interpret the string as a decimal integer and compute the remainder of that number on division by 97
                        ////.NET 4.0 has System.Numerics.BigInteger - use this to parse (big) int value
                        System.Numerics.BigInteger bigAssInt = 0;
                        char[] zero = { '0' };
                        bool parsed = System.Numerics.BigInteger.TryParse(iban.TrimStart(zero), out bigAssInt);
                        if (parsed && (bigAssInt > 0))
                        {
                            ibanValid = bigAssInt % 97 == 1;
                        }
                    }
                }
            }

            return ibanValid;
        }
    }
}