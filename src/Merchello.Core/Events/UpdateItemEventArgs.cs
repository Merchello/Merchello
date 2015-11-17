namespace Merchello.Core.Events
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core.Events;

    /// <summary>
    /// Event arguments for updating line items.
    /// </summary>
    /// <typeparam name="T">
    /// The type of updated item
    /// </typeparam>
    public class UpdateItemEventArgs<T> : CancellableObjectEventArgs<T>
    {
        /// <summary>
        /// The result.
        /// </summary>
        private readonly T _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateItemEventArgs{T}"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public UpdateItemEventArgs(T result)            
            : base(result, true)
        {
            _result = result;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public T Result
        {
            get
            {
                return _result;
            }
        }
    }
}
