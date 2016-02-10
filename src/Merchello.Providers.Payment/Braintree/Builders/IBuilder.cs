namespace Merchello.Providers.Payment.Braintree.Builders
{
    /// <summary>
    /// Defines a build.
    /// </summary>
    /// <typeparam name="T">
    /// The type to be built
    /// </typeparam>
    public interface IBuilder<out T>
    {
        /// <summary>
        /// Builds an object of type T.
        /// </summary>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Build();
    }
}