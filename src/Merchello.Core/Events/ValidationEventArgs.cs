namespace Merchello.Core.Events
{
    /// <summary>
    /// The validation event args.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result
    /// </typeparam>
    public class ValidationEventArgs<T> : CancellableObjectEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationEventArgs{T}"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public ValidationEventArgs(T result)
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