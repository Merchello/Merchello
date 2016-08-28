namespace Merchello.Core.Events
{
    /// <summary>
    /// Event arguments for updating line items.
    /// </summary>
    /// <typeparam name="T">
    /// The type of updated item
    /// </typeparam>
    public class UpdateItemEventArgs<T> : CancellableObjectEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateItemEventArgs{T}"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public UpdateItemEventArgs(T result)            
            : base(result, true)
        {
            this.Result = result;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public T Result { get; }
    }
}
