namespace Merchello.Web.Validation
{
    using System.Collections.Generic;

    /// <summary>
    /// The validation result.
    /// </summary>
    /// <typeparam name="T">
    /// The type to be validated
    /// </typeparam>
    public class ValidationResult<T>
    {
        /// <summary>
        /// Validation messages.
        /// </summary>
        private readonly List<string> _messages = new List<string>();

        /// <summary>
        /// Gets or sets the validated object
        /// </summary>
        public T Validated { get; set; }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public List<string> Messages
        {
            get
            {
                return _messages;
            }
        }
    }
}