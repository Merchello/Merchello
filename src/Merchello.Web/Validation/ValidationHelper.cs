namespace Merchello.Web.Validation
{
    using System;

    using Merchello.Core.Validation;

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
        /// The <see cref="IEmailValidationHelper"/>.
        /// </summary>
        private readonly Lazy<IEmailValidationHelper> _email = new Lazy<IEmailValidationHelper>(() => new EmailValidationHelper());

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

        /// <summary>
        /// Gets the <see cref="IEmailValidationHelper"/>.
        /// </summary>
        public IEmailValidationHelper Email
        {
            get
            {
                return _email.Value;
            }
        }
    }
}