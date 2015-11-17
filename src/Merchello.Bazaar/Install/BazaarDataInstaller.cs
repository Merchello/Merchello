namespace Merchello.Bazaar.Install
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Examine;

    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping.FixedRate;
    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;

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
        /// The collections.
        /// </summary>
        /// <remarks>
        /// Introduced in 1.12.0
        /// </remarks>
        private readonly IDictionary<string, Guid> _collections = new Dictionary<string, Guid>(); 

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
            this.AddMerchelloData();

            LogHelper.Info<BazaarDataInstaller>("Adding Example Merchello Data");

            LogHelper.Info<BazaarDataInstaller>("Installing store root node");
            var root = _services.ContentService.CreateContent("Store", -1, "BazaarStore");

            // Default theme
            root.SetValue("themePicker", "Sandstone-3");
            root.SetValue("customerMemberType", "MerchelloCustomer");
            root.SetValue("storeTitle", "Merchello Bazaar");
            root.SetValue("tagLine", "Get Shopping");
            root.SetValue("featuredProducts", _collections["featuredProducts"].ToString());

            _services.ContentService.SaveAndPublishWithStatus(root);

            LogHelper.Info<BazaarDataInstaller>("Adding example category page");
            var pg = _services.ContentService.CreateContent("Specialized Soap", root.Id, "BazaarProductCollection");
            pg.SetValue("products", _collections["specializedSoap"].ToString());
            _services.ContentService.SaveAndPublishWithStatus(pg);

            var gen = _services.ContentService.CreateContent("Generic", root.Id, "BazaarProductCollection");
            gen.SetValue("products", _collections["generic"].ToString());
            _services.ContentService.SaveAndPublishWithStatus(gen);

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

            // TODO figure out why the index does not build on load
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();

            return root;
        }

        /// <summary>
        /// The add merchello data.
        /// </summary>
        private void AddMerchelloData()
        {
            if (!MerchelloContext.HasCurrent)
            {
                LogHelper.Info<BazaarDataInstaller>("MerchelloContext was null");
            }

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
            var catalog =
                warehouse.WarehouseCatalogs.FirstOrDefault(
                    x => x.Key == Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey);
            var country = merchelloServices.StoreSettingService.GetCountryByCode("US");

            // The follow is internal to Merchello and not exposed in the public API
            var shipCountry =
                ((Core.Services.ServiceContext)merchelloServices).ShipCountryService.GetShipCountryByCountryCode(
                    catalog.Key,
                    "US");

            // Add the ship country
            if (shipCountry == null || shipCountry.CountryCode == "ELSE")
            {
                shipCountry = new ShipCountry(catalog.Key, country);
                ((Core.Services.ServiceContext)merchelloServices).ShipCountryService.Save(shipCountry);
            }

            // Associate the fixed rate Shipping Provider to the ShipCountry
            var key = Core.Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider =
                (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.GetProviderByKey(key);
            var gatewayShipMethod =
                (FixedRateShippingGatewayMethod)
                rateTableProvider.CreateShipMethod(
                    FixedRateShippingGatewayMethod.QuoteType.VaryByWeight,
                    shipCountry,
                    "Ground");

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

            // Add the product collections
            LogHelper.Info<BazaarDataInstaller>("Adding example product collections");
            var featuredProducts =
                merchelloServices.EntityCollectionService.CreateEntityCollectionWithKey(
                    EntityType.Product,
                    Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                    "Featured Products");
            var soap =
                merchelloServices.EntityCollectionService.CreateEntityCollectionWithKey(
                    EntityType.Product,
                    Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                    "Soap");
            
            var specializedSoap = merchelloServices.EntityCollectionService.CreateEntityCollection(
                EntityType.Product,
                Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                "Specialized Soap");
            specializedSoap.ParentKey = soap.Key;

            var generic =
                merchelloServices.EntityCollectionService.CreateEntityCollection(
                    EntityType.Product,
                    Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                    "Generic");
            generic.ParentKey = soap.Key;
            merchelloServices.EntityCollectionService.Save(new[] { specializedSoap, generic });

            _collections.Add("featuredProducts", featuredProducts.Key);
            _collections.Add("specializedSoap", specializedSoap.Key);
            _collections.Add("generic", generic.Key);

            // Add the detached content type
            LogHelper.Info<BazaarDataInstaller>("Getting information for detached content");
            var contentType = _services.ContentTypeService.GetContentType("BazaarProductContent");
            var detachedContentTypeService =
                ((Core.Services.ServiceContext)merchelloServices).DetachedContentTypeService;
            var detachedContentType = detachedContentTypeService.CreateDetachedContentType(
                EntityType.Product,
                contentType.Key,
                "Bazaar Product");
            detachedContentType.Description = "Default Bazaar Product Content";
            detachedContentTypeService.Save(detachedContentType);            
            LogHelper.Info<BazaarDataInstaller>("Adding an example product Avocado Bar");
            var avocadoBar = merchelloServices.ProductService.CreateProduct("Avocado Bar", "avocadobar", 5M);
            avocadoBar.Shippable = true;
            avocadoBar.OnSale = false;
            avocadoBar.SalePrice = 3M;
            avocadoBar.Taxable = true;
            avocadoBar.TrackInventory = false;
            avocadoBar.Available = true;
            avocadoBar.Weight = 1M;
            avocadoBar.AddToCatalogInventory(catalog);
            merchelloServices.ProductService.Save(avocadoBar, false);

            // add to collections
            avocadoBar.AddToCollection(featuredProducts);
            avocadoBar.AddToCollection(specializedSoap);
            
            avocadoBar.DetachedContents.Add(
                new ProductVariantDetachedContent(
                    avocadoBar.ProductVariantKey,
                    detachedContentType,
                    "en-US",                    
                    new DetachedDataValuesCollection(
                        new[]
                            {
                                new KeyValuePair<string, string>(
                                    "description",
                                    "\"<p><span>Made with real avocados, this Avocado Moisturizing Bar is great for dry skin. Layers of color are achieved by using oxide colorants. Scented with Wasabi Fragrance Oil, this soap smells slightly spicy, making it a great choice for both men and women. To ensure this soap does not overheat, place in the freezer to keep cool and prevent gel phase.</span></p>\""),
                                new KeyValuePair<string, string>(
                                    "brief",
                                    "\"Avocado Moisturizing Bar is great for dry skin.\""),
                                new KeyValuePair<string, string>(
                                    "image",
                                    "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1035/avocadobars.jpg\" }"), 
                            }))
                    {
                        CanBeRendered = true
                    });            
            merchelloServices.ProductService.Save(avocadoBar);

            LogHelper.Info<BazaarDataInstaller>("Adding an example product Liquid Soap");
            var liquidSoap = merchelloServices.ProductService.CreateProduct("Liquid Soap", "liquidsoap", 16M);
            liquidSoap.OnSale = true;
            liquidSoap.SalePrice = 12M;
            liquidSoap.Taxable = true;
            liquidSoap.TrackInventory = false;
            liquidSoap.Available = true;
            liquidSoap.Weight = 1M;
            liquidSoap.AddToCatalogInventory(catalog);
            merchelloServices.ProductService.Save(liquidSoap, false);

            // add to collections
            liquidSoap.AddToCollection(featuredProducts);
            liquidSoap.AddToCollection(specializedSoap);

            liquidSoap.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   liquidSoap.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>(
                                    "description",
                                    "\"<p>Soap is better liquefied.</p>\""),
                                new KeyValuePair<string, string>(
                                    "brief",
                                    "\"Liquid Soap.\""),
                                new KeyValuePair<string, string>(
                                    "image",
                                    "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1037/beeswaxlotionbase_lotionbee_main_a.jpg\" }"), 
                            }))
                   {
                       CanBeRendered = true
                   });
            merchelloServices.ProductService.Save(liquidSoap);

            LogHelper.Info<BazaarDataInstaller>("Adding an example product Generic Soap");
            var genericSoap = merchelloServices.ProductService.CreateProduct("Generic Soap", "generic", 3M);
            genericSoap.OnSale = false;
            genericSoap.SalePrice = 2M;
            genericSoap.Taxable = true;
            genericSoap.TrackInventory = false;
            genericSoap.Available = true;
            genericSoap.Weight = 1M;
            genericSoap.AddToCatalogInventory(catalog);
            genericSoap.ProductOptions.Add(new ProductOption("Color"));
            genericSoap.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("White", "White"));
            genericSoap.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Green", "Green"));
            genericSoap.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blue"));
            genericSoap.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Pink", "Pink"));
            genericSoap.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Purple", "Purple"));
            merchelloServices.ProductService.Save(genericSoap, false);

            // add to collections
            genericSoap.AddToCollection(generic);

            genericSoap.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   genericSoap.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>(
                                    "description",
                                    "\"<p>A substance used with water for washing and cleaning, made of a compound of natural oils or fats with sodium hydroxide or another strong alkali, and typically having perfume and coloring added.</p>\""),
                                new KeyValuePair<string, string>(
                                    "brief",
                                    "\"Generic soap.\""),
                                new KeyValuePair<string, string>(
                                    "image",
                                    "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1038/bathbombs.jpg\" }"), 
                            }))
                   {
                       CanBeRendered = true
                   });

            merchelloServices.ProductService.Save(genericSoap);
        }
    }
}