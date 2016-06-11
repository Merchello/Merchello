namespace Merchello.Core.Events
{
    using System;

    using Umbraco.Core.Events;

    /// <summary>
    /// The validation event args.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result
    /// </typeparam>
    public class ValidationEventArgs<T> : CancellableObjectEventArgs<T>
    {
        /// <summary>
        /// The validation result.
        /// </summary>
        private readonly T _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationEventArgs{T}"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public ValidationEventArgs(T result)
            : base(result, true)
        {
            this._result = result;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public T Result
        {
            get
            {
                return this._result;
            }
        }
    }
}