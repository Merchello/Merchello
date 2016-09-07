namespace Merchello.Core.Acquired.Configuration
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;

    using Merchello.Core.Configuration;

    /// <summary>
    /// Defines a configuration section that contains inner text that is comma delimited
    /// </summary>
    /// UMBRACO_SRC
    internal class CommaDelimitedConfigurationElement : InnerTextConfigurationElement<CommaDelimitedStringCollection>, IEnumerable<string>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public override CommaDelimitedStringCollection Value
        {
            get 
            { 
                var converter = new CommaDelimitedStringCollectionConverter();
                return (CommaDelimitedStringCollection) converter.ConvertFrom(this.RawValue);
            }
        }

        /// <summary>
        /// Gets the enumerator for the collection.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return new InnerEnumerator(this.Value.GetEnumerator());
        }

        /// <summary>
        /// Gets the enumerator for the collection.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new InnerEnumerator(this.Value.GetEnumerator());
        }

        /// <summary>
        /// A wrapper for StringEnumerator since it doesn't explicitly implement IEnumerable
        /// </summary>
        private class InnerEnumerator : IEnumerator<string>
        {
            /// <summary>
            /// The inner string enumerator.
            /// </summary>
            private readonly StringEnumerator _stringEnumerator;

            /// <summary>
            /// Initializes a new instance of the <see cref="InnerEnumerator"/> class.
            /// </summary>
            /// <param name="stringEnumerator">
            /// The string enumerator.
            /// </param>
            public InnerEnumerator(StringEnumerator stringEnumerator)
            {
                this._stringEnumerator = stringEnumerator;
            }

            /// <summary>
            /// Gets the current enumerator as a string value.
            /// </summary>
            string IEnumerator<string>.Current
            {
                get { return this._stringEnumerator.Current; }
            }

            /// <summary>
            /// Gets the current enumerator.
            /// </summary>
            public object Current
            {
                get { return this._stringEnumerator.Current; }
            }

            /// <summary>
            /// Moves the the next item in the collection.
            /// </summary>
            /// <returns>
            /// A value indicating whether or not the move was successful.
            /// </returns>
            public bool MoveNext()
            {
                return this._stringEnumerator.MoveNext();
            }

            /// <summary>
            /// Resets the enumerator.
            /// </summary>
            public void Reset()
            {
                this._stringEnumerator.Reset();
            }

            public void Dispose()
            {
                this._stringEnumerator.DisposeIfDisposable();
            }
        }
    }
}