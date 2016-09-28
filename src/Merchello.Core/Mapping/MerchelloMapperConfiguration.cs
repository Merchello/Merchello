namespace Merchello.Core.Mapping
{
    using AutoMapper;

    /// <summary>
    /// Represents a AutoMapper Mapper Configuration.
    /// </summary>
    public abstract class MerchelloMapperConfiguration
    {
        /// <summary>
        /// Configures AutoMapper mappings.
        /// </summary>
        /// <param name="config">
        /// The <see cref="IMapperConfiguration"/>.
        /// </param>
        public abstract void ConfigureMappings(IMapperConfiguration config);
    }
}