namespace Merchello.Core.DependencyInjection
{
    using LightInject;

    using Merchello.Core.Configuration;
    using Merchello.Core.Configuration.Sections;

    /// <summary>
    /// Adds configurations to the service container
    /// </summary>
    public class ConfigurationCompositionRoot : ICompositionRoot
    {
        /// <summary>
        /// Composes configuration services by adding services to the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public void Compose(IServiceRegistry container)
        {
            // MerchelloSettings
            container.Register<IMerchelloSettingsSection>(factory => MerchelloConfig.For.MerchelloSettings());
            container.Register<IProductsSection>(factory => factory.GetInstance<IMerchelloSettingsSection>().Products);
            container.Register<ICheckoutSection>(factory => factory.GetInstance<IMerchelloSettingsSection>().Checkout);
            container.Register<ISalesSection>(factory => factory.GetInstance<IMerchelloSettingsSection>().Sales);
            container.Register<ICustomersSection>(factory => factory.GetInstance<IMerchelloSettingsSection>().Customers);
            container.Register<IFiltersSection>(factory => factory.GetInstance<IMerchelloSettingsSection>().Filters);
            container.Register<IMigrationsSection>(factory => factory.GetInstance<IMerchelloSettingsSection>().Migrations);
            container.Register<ICheckoutContextSection>(factory => factory.GetInstance<ICheckoutSection>().CheckoutContext);

            // MerchelloExtensibility
            container.Register<IMerchelloExtensibilitySection>(factory => MerchelloConfig.For.MerchelloExtensibility());
            container.Register<IMvcSection>(factory => factory.GetInstance<IMerchelloExtensibilitySection>().Mvc);
            container.Register<IViewsSection>(factory => factory.GetInstance<IMvcSection>().Views);
            container.Register<IVirtualContentSection>(factory => factory.GetInstance<IMvcSection>().VirtualContent);
            container.Register<IVirtualContentRoutingSection>(factory => factory.GetInstance<IVirtualContentSection>().Routing);
            container.Register<IBackOfficeSection>(factory => factory.GetInstance<IMerchelloExtensibilitySection>().BackOffice);
            container.Register<ITypeFieldsSection>(factory => factory.GetInstance<IMerchelloExtensibilitySection>().TypeFields);

            // MerchelloCountries
            container.Register<IMerchelloCountriesSection>(factory => MerchelloConfig.For.MerchelloCountries());
        }
    }
}