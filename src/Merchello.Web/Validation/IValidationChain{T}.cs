namespace Merchello.Web.Validation
{
    using Umbraco.Core;

    /// <summary>
    /// Defines a validation chain.
    /// </summary>
    /// <typeparam name="T">
    /// The value to validate
    /// </typeparam>
    public interface IValidationChain<T>
    {
        /// <summary>
        /// Performs the validation of T.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{T}"/>.
        /// </returns>
        Attempt<ValidationResult<T>> Validate(T value);
    }
}