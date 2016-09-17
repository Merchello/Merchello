namespace Merchello.Core.DependencyInjection
{
    using LightInject;

    /// <summary>
    /// Sets the IoC container for the Merchello data layer/repositories/sql/database/etc...
    /// </summary>
    public sealed class RepositoryCompositionRoot : ICompositionRoot
    {
        /// <summary>
        /// Composes configuration services by adding services to the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public void Compose(IServiceRegistry container)
        {
            throw new System.NotImplementedException();
        }
    }
}