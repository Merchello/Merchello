namespace Merchello.Core.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Used for specifying default values for comma delimited config
    /// </summary>
    /// UMBRACO_SRC
    internal class OptionalCommaDelimitedConfigurationElement : CommaDelimitedConfigurationElement
    {
        /// <summary>
        /// The wrapped <see cref="CommaDelimitedConfigurationElement"/>.
        /// </summary>
        private readonly CommaDelimitedConfigurationElement _wrapped;

        /// <summary>
        /// An array of default values.
        /// </summary>
        private readonly string[] _defaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalCommaDelimitedConfigurationElement"/> class.
        /// </summary>
        public OptionalCommaDelimitedConfigurationElement()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalCommaDelimitedConfigurationElement"/> class.
        /// </summary>
        /// <param name="wrapped">
        /// The wrapped <see cref="CommaDelimitedConfigurationElement"/>.
        /// </param>
        /// <param name="defaultValue">
        /// An array of default values.
        /// </param>
        public OptionalCommaDelimitedConfigurationElement(CommaDelimitedConfigurationElement wrapped, string[] defaultValue)
        {
            this._wrapped = wrapped;
            this._defaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public override CommaDelimitedStringCollection Value
        {
            get
            {
                if (this._wrapped == null)
                {
                    return base.Value;
                }

                if (string.IsNullOrEmpty(this._wrapped.RawValue))
                {
                    var val = new CommaDelimitedStringCollection();
                    val.AddRange(this._defaultValue);
                    return val;
                }
                return this._wrapped.Value;
            }
        }
    }
}