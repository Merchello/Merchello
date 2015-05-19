namespace Merchello.Core.Marketing.Offer
{
    /// <summary>
    /// The OfferConfigurationContract interface.
    /// </summary>
    /// <typeparam name="T">
    /// The type of OfferConfiguration
    /// </typeparam>
    public interface IOfferConfigurationDataSerilizer<T> where T : class, new()
    {
        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="json">
        /// The JSON value.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Deserialize(string json);

        /// <summary>
        /// The serialize.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Serialize(T configuration);
    }
}