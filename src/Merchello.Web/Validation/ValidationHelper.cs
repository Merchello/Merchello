namespace Merchello.Web.Validation
{
    using System;

    /// <summary>
    /// Represents a ValidationHelper.
    /// </summary>
    internal class ValidationHelper : IValidationHelper
    {
        /// <summary>
        /// The <see cref="IBankingValidationHelper"/>.
        /// </summary>
        private readonly Lazy<IBankingValidationHelper> _banking = new Lazy<IBankingValidationHelper>(() => new BankingValidationHelper());

        /// <summary>
        /// Gets the <see cref="IBankingValidationHelper"/>.
        /// </summary>
        public IBankingValidationHelper Banking
        {
            get
            {
                return _banking.Value;
            }
        }
    }
}