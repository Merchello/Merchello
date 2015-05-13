namespace Merchello.Web.Validation
{
    /// <summary>
    /// Defines a ValidationHelper.
    /// </summary>
    public interface IValidationHelper
    {
        /// <summary>
        /// Gets the <see cref="IBankingValidationHelper"/>.
        /// </summary>
        IBankingValidationHelper Banking { get; }
    }
}