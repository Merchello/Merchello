namespace Merchello.Core.Acquired.Configuration
{
    using System;
    using System.Xml;
    using System.Xml.Linq;

    using Merchello.Core.Configuration;

    /// <summary>
    /// A full config section is required for any full element and we have some elements that are defined like this:
    /// {element}MyValue{/element} instead of as attribute values.
    /// </summary>
    /// <typeparam name="T">
    /// Resulting type to convert the inner text into.
    /// </typeparam>
    /// UMBRACO
    internal class InnerTextConfigurationElement<T> : RawXmlConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InnerTextConfigurationElement{T}"/> class.
        /// </summary>
        public InnerTextConfigurationElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InnerTextConfigurationElement{T}"/> class.
        /// </summary>
        /// <param name="rawXml">
        /// The raw xml.
        /// </param>
        public InnerTextConfigurationElement(XElement rawXml) 
            : base(rawXml)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Throws an <see cref="InvalidCastException"/> if the raw value could not be converted to type T.
        /// </exception>
        public virtual T Value
        {
            get
            {
                var converted = this.RawValue.TryConvertTo<T>();
                if (converted.Success == false)
                    throw new InvalidCastException("Could not convert value " + this.RawValue + " to type " + typeof(T));
                return converted.Result;
            }
        }

        /// <summary>
        /// Gets or sets the raw string value
        /// </summary>
        internal string RawValue { get; set; }

        /// <summary>
        /// Implicit operator so we don't need to use the 'Value' property explicitly
        /// </summary>
        /// <param name="m">
        /// The <see cref="InnerTextConfigurationElement{T}"/>
        /// </param>
        /// <returns></returns>
        public static implicit operator T(InnerTextConfigurationElement<T> m)
        {
            return m.Value;
        }

        /// <summary>
        /// Return the string value of Value
        /// </summary>
        /// <returns>
        /// The value
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}", this.Value);
        }

        /// <summary>
        /// The deserialize element.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="serializeCollectionKey">
        /// The serialize collection key.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if the inner text contains additional elements.
        /// </exception>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            //// now validate and set the raw value
            if (this.RawXml.HasElements)
                throw new InvalidOperationException("An InnerTextConfigurationElement cannot contain any child elements, only attributes and a value");

            this.RawValue = this.RawXml.Value.Trim();
        }
    }
}