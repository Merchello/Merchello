namespace Merchello.Bazaar.Install
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping.FixedRate;
    using Merchello.Core.Models;

    using umbraco.cms.businesslogic.web;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// The bazaar data installer.
    /// </summary>
    internal class BazaarDataInstaller
    {
        /// <summary>
        /// The service context.
        /// </summary>
        private readonly ServiceContext _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="BazaarDataInstaller"/> class.
        /// </summary>
        public BazaarDataInstaller()
        {
            _services = ApplicationContext.Current.Services;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <returns>
        /// The <see cref="IContent"/>.
        /// </returns>
        public IContent Execute()
        {
            var product = this.AddMerchelloData();

            LogHelper.Info<BazaarDataInstaller>("Adding the MerchelloCustomer MemberType");
            var memberType = AddMerchelloCustomerMemberType();

            LogHelper.Info<BazaarDataInstaller>("Adding the MerchelloCustomers MemberGroup");
            var memberGroup = AddMerchelloCustomersMemberGroup();

            LogHelper.Info<BazaarDataInstaller>("Adding Example Merchello Data");

            LogHelper.Info<BazaarDataInstaller>("Installing store root node");
            var root = _services.ContentService.CreateContent("Store", -1, "BazaarStore");

            // Default theme
            root.SetValue("themePicker", "Flatly");
            root.SetValue("customerMemberType", "MerchelloCustomer");
            root.SetValue("storeTitle", "Merchello Bazaar");
            root.SetValue("tagLine", "Get Shopping");


            _services.ContentService.SaveAndPublishWithStatus(root);

            LogHelper.Info<BazaarDataInstaller>("Adding Example ProductGroup and Products");
            var pg = _services.ContentService.CreateContent("Soap", root.Id, "BazaarProductGroup");
            pg.SetValue("image", @"{  'focalPoint': { 'left': 0.5, 'top': 0.5 }, 'src': '/media/1005/soapcategory.jpg', 'crops': [] }'");
            pg.SetValue("brief", "Avocado Moisturizing Bar is great for dry skin.");
            _services.ContentService.SaveAndPublishWithStatus(pg);

            var prod = _services.ContentService.CreateContent("Bar of Soap", pg.Id, "BazaarProduct");
            prod.SetValue("merchelloProduct", product.Key.ToString());
            prod.SetValue("description", "<p><span>Made with real avocados, this Avocado Moisturizing Bar is great for dry skin. Layers of color are achieved by using oxide colorants. Scented with Wasabi Fragrance Oil, this soap smells slightly spicy, making it a great choice for both men and women. To ensure this soap does not overheat, place in the freezer to keep cool and prevent gel phase.</span></p>");
            prod.SetValue("brief", "Avocado Moisturizing Bar is great for dry skin.");
            prod.SetValue("image", "{ 'focalPoint': { 'left': 0.5, 'top': 0.5 }, 'src': '/media/1009/avocadobars.jpg', 'crops': [] }");
            _services.ContentService.SaveAndPublishWithStatus(prod);

            LogHelper.Info<BazaarDataInstaller>("Adding example eCommerce workflow pages");
            var basket = _services.ContentService.CreateContent("Basket", root.Id, "BazaarBasket");
            _services.ContentService.SaveAndPublishWithStatus(basket);

            var checkout = _services.ContentService.CreateContent("Checkout", root.Id, "BazaarCheckout");
            _services.ContentService.SaveAndPublishWithStatus(checkout);

            var checkoutConfirm = _services.ContentService.CreateContent("Confirm Sale", checkout.Id, "BazaarCheckoutConfirm");
            _services.ContentService.SaveAndPublishWithStatus(checkoutConfirm);

            var receipt = _services.ContentService.CreateContent("Receipt", checkout.Id, "BazaarReceipt");
            _services.ContentService.SaveAndPublishWithStatus(receipt);

            var registration = _services.ContentService.CreateContent("Registration / Login", root.Id, "BazaarRegistration");
            _services.ContentService.SaveAndPublishWithStatus(registration);

            var account = _services.ContentService.CreateContent("Account", root.Id, "BazaarAccount");
            _services.ContentService.SaveAndPublishWithStatus(account);

            var wishList = _services.ContentService.CreateContent("Wish List", account.Id, "BazaarWishList");
            _services.ContentService.SaveAndPublishWithStatus(wishList);

            var purchaseHistory = _services.ContentService.CreateContent("Purchase History", account.Id, "BazaarAccountHistory");
            _services.ContentService.SaveAndPublishWithStatus(purchaseHistory);

            // TODO look for a V6 method
            // This uses the old API
            Access.ProtectPage(false, account.Id, registration.Id, registration.Id);
            Access.AddMembershipRoleToDocument(account.Id, "MerchelloCustomers");

            return root;
        }



        /// <summary>
        /// The add merchello customer member type.
        /// </summary>
        /// <returns>
        /// The <see cref="MemberType"/>.
        /// </returns>
        private MemberType AddMerchelloCustomerMemberType()
        {
            var dtd = _services.DataTypeService.GetDataTypeDefinitionById(-88);

            // Create the MerchelloCustomer MemberType
            var mt = new MemberType(-1)
            {
                Alias = "MerchelloCustomer",
                Name = "MerchelloCustomer",
                AllowedAsRoot = true
            };

            var fn = new PropertyType(dtd) { Alias = "firstName", Name = "First name" };
            var ln = new PropertyType(dtd) { Alias = "lastName", Name = "Last name" };

            mt.AddPropertyType(fn);
            mt.AddPropertyType(ln);

            mt.SetMemberCanEditProperty("firstName", true);
            mt.SetMemberCanEditProperty("lastName", true);
            mt.SetMemberCanViewProperty("firstName", true);
            mt.SetMemberCanViewProperty("lastName", true);

            _services.MemberTypeService.Save(mt);

            return mt;
        }

        /// <summary>
        /// Ads the MerchelloCustomers MemberGroup.
        /// </summary>
        /// <returns>
        /// The <see cref="MemberGroup"/>.
        /// </returns>
        private MemberGroup AddMerchelloCustomersMemberGroup()
        {
            var mg = new MemberGroup() { Name = "MerchelloCustomers" };

            _services.MemberGroupService.Save(mg);

            return mg;
        }

        /// <summary>
        /// The add merchello data.
        /// </summary>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        private IProduct AddMerchelloData()
        {
            var merchelloServices = MerchelloContext.Current.Services;

            LogHelper.Info<BazaarDataInstaller>("Updating Default Warehouse Address");
            var warehouse = merchelloServices.WarehouseService.GetDefaultWarehouse();
            warehouse.Name = "Merchello";
            warehouse.Address1 = "114 W. Magnolia St.";
            warehouse.Address2 = "Suite 300";
            warehouse.Locality = "Bellingham";
            warehouse.Region = "WA";
            warehouse.PostalCode = "98225";
            warehouse.CountryCode = "US";
            merchelloServices.WarehouseService.Save(warehouse);

            LogHelper.Info<BazaarDataInstaller>("Adding example shipping data");
            var catalog = warehouse.WarehouseCatalogs.FirstOrDefault(x => x.Key == Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey);
            var country = merchelloServices.StoreSettingService.GetCountryByCode("US");

            // The follow is internal to Merchello and not exposed in the public API
          
            // Add the ship country
            var shipCountry = new ShipCountry(catalog.Key, country);
            ((Core.Services.ServiceContext)merchelloServices).ShipCountryService.Save(shipCountry);

            // Associate the fixed rate Shipping Provider to the ShipCountry
            var key = Core.Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.GetProviderByKey(key);
            var gatewayShipMethod = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, shipCountry, "Ground");

            // Add rate adjustments for Hawaii and Alaska
            gatewayShipMethod.ShipMethod.Provinces["HI"].RateAdjustmentType = RateAdjustmentType.Numeric;
            gatewayShipMethod.ShipMethod.Provinces["HI"].RateAdjustment = 3M;
            gatewayShipMethod.ShipMethod.Provinces["AK"].RateAdjustmentType = RateAdjustmentType.Numeric;
            gatewayShipMethod.ShipMethod.Provinces["AK"].RateAdjustment = 2.5M;

            // Add a few of rate tiers to the rate table.
            gatewayShipMethod.RateTable.AddRow(0, 10, 5);
            gatewayShipMethod.RateTable.AddRow(10, 15, 10);
            gatewayShipMethod.RateTable.AddRow(15, 25, 25);
            gatewayShipMethod.RateTable.AddRow(25, 10000, 100);
            rateTableProvider.SaveShippingGatewayMethod(gatewayShipMethod);

            LogHelper.Info<BazaarDataInstaller>("Adding an example product");
            var product = merchelloServices.ProductService.CreateProduct("Bar of Soap", "soapbar", 5M);
            product.Shippable = true;
            product.Taxable = true;
            product.TrackInventory = false;
            product.Available = true;
            product.Weight = 1M;
            product.AddToCatalogInventory(catalog);
            merchelloServices.ProductService.Save(product, false);
            product.CatalogInventories.FirstOrDefault().Count = 10;
            merchelloServices.ProductService.Save(product, false);
            return product;
        }

    }
}