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
        // Collection dictionary names

        /// <summary>
        /// The collection featured products.
        /// </summary>
        private const string CollectionFeaturedProducts = "collectionFeaturedProducts";

        /// <summary>
        /// The collection home page.
        /// </summary>
        private const string CollectionHomePage = "collectionHomePage";

        /// <summary>
        /// The collection main categories.
        /// </summary>
        private const string CollectionMainCategories = "collectionMainCategories";

        /// <summary>
        /// The collection funny.
        /// </summary>
        private const string CollectionFunny = "collectionFunny";

        /// <summary>
        /// The collection geeky.
        /// </summary>
        private const string CollectionGeeky = "collectionGeeky";

        /// <summary>
        /// The collection sad.
        /// </summary>
        private const string CollectionSad = "collectionSad";

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
            // Adds the Merchello Data
            LogHelper.Info<BazaarDataInstaller>("Starting to add example Merchello data");
            this.AddMerchelloData();

            // Adds the example Umbraco data
            LogHelper.Info<BazaarDataInstaller>("Starting to add example Merchello Umbraco data");
            return this.AddUmbracoData();
        }

        /// <summary>
        /// Adds the Umbraco content.
        /// </summary>
        /// <returns>
        /// The <see cref="IContent"/>.
        /// </returns>
        private IContent AddUmbracoData()
        {
            #region Store Root

            // Create the store root and add the initial data

            LogHelper.Info<BazaarDataInstaller>("Installing store root node");

            var storeRoot = _services.ContentService.CreateContent("Store", -1, "BazaarStore");

            // Default theme
            storeRoot.SetValue("themePicker", "Sandstone-3");
            storeRoot.SetValue("customerMemberType", "MerchelloCustomer");
            storeRoot.SetValue("storeTitle", "Merchello Bazaar");
            storeRoot.SetValue("overview", @"<p>The Merchello Bazaar is a simple, example store which has been developed to help get you up and running quickly with Merchello. 
                                            It's designed to show you how to implement common features, and <a href=""https://github.com/Merchello/Merchello"" target=""_blank"">
                                            you can grab the source code from here</a>, just fork/clone/download and open up Merchello.Bazaar.sln</p>");
            storeRoot.SetValue("featuredProducts", _collections["collectionFeaturedProducts"].ToString());

            _services.ContentService.SaveAndPublishWithStatus(storeRoot);

            #endregion

            #region Example Categories

            // Add the example categories
            LogHelper.Info<BazaarDataInstaller>("Adding example category page");

            // Create the root T-Shirt category
            var tShirtCategory = _services.ContentService.CreateContent("All T-Shirts", storeRoot.Id, "BazaarProductCollection");

            tShirtCategory.SetValue("products", string.Empty);
            _services.ContentService.SaveAndPublishWithStatus(tShirtCategory);
                
            // Create the sun categories
            var funnyTShirts = _services.ContentService.CreateContent("Funny T-Shirts", tShirtCategory.Id, "BazaarProductCollection");
            funnyTShirts.SetValue("products", _collections[CollectionFunny].ToString());
            funnyTShirts.SetValue("description", @"<p>Pinterest health goth stumptown before they sold out. Locavore banjo typewriter, street art viral XOXO kickstarter retro brooklyn direct trade. 
                                                    Gluten-free taxidermy messenger bag celiac, kinfolk pinterest affogato tattooed echo park chillwave chambray slow-carb freegan. Messenger bag kombucha hammock pabst twee yuccie.</p>");
            _services.ContentService.SaveAndPublishWithStatus(funnyTShirts);

            var geekyTShirts = _services.ContentService.CreateContent("Geeky T-Shirts", tShirtCategory.Id, "BazaarProductCollection");
            geekyTShirts.SetValue("products", _collections[CollectionGeeky].ToString());
            geekyTShirts.SetValue("description", @"<p>Forage master cleanse jean shorts knausgaard sustainable. Kale chips wayfarers pop-up selvage, hashtag tattooed yuccie mlkshk truffaut next level. 
                                                    Street art salvia drinking vinegar brunch kogi, put a bird on it farm-to-table food truck shoreditch next level vegan bespoke portland venmo helvetica.</p>");
            _services.ContentService.SaveAndPublishWithStatus(geekyTShirts);

            var sadTShirts = _services.ContentService.CreateContent("Sad T-Shirts", tShirtCategory.Id, "BazaarProductCollection");
            sadTShirts.SetValue("products", _collections[CollectionSad].ToString());
            sadTShirts.SetValue("description", @"<p>Fingerstache ramps lo-fi schlitz. Microdosing yr keffiyeh, ennui sartorial pork belly meditation polaroid. Tofu tilde occupy, photo booth single-origin coffee hammock gentrify. 
                                                    Farm-to-table PBR&amp;B 90's fashion axe sustainable blue bottle typewriter occupy, twee drinking vinegar yuccie. Pabst yr vegan, truffaut DIY slow-carb tumblr thundercats next level street 
                                                    art biodiesel leggings hella letterpress food truck. Gochujang vice normcore deep v, mustache PBR&amp;B drinking vinegar yr gentrify shoreditch.</p>");
            _services.ContentService.SaveAndPublishWithStatus(sadTShirts);

            #endregion

            #region Required Other Pages

            LogHelper.Info<BazaarDataInstaller>("Adding example eCommerce workflow pages");

            var basket = _services.ContentService.CreateContent("Basket", storeRoot.Id, "BazaarBasket");
            _services.ContentService.SaveAndPublishWithStatus(basket);

            var checkout = _services.ContentService.CreateContent("Checkout", storeRoot.Id, "BazaarCheckout");
            _services.ContentService.SaveAndPublishWithStatus(checkout);

            var checkoutConfirm = _services.ContentService.CreateContent("Confirm Sale", checkout.Id, "BazaarCheckoutConfirm");
            _services.ContentService.SaveAndPublishWithStatus(checkoutConfirm);

            var receipt = _services.ContentService.CreateContent("Receipt", checkout.Id, "BazaarReceipt");
            _services.ContentService.SaveAndPublishWithStatus(receipt);

            var registration = _services.ContentService.CreateContent("Registration / Login", storeRoot.Id, "BazaarRegistration");
            _services.ContentService.SaveAndPublishWithStatus(registration);

            var account = _services.ContentService.CreateContent("Account", storeRoot.Id, "BazaarAccount");
            _services.ContentService.SaveAndPublishWithStatus(account);

            var wishList = _services.ContentService.CreateContent("Wish List", account.Id, "BazaarWishList");
            _services.ContentService.SaveAndPublishWithStatus(wishList);

            var purchaseHistory = _services.ContentService.CreateContent("Purchase History", account.Id, "BazaarAccountHistory");
            _services.ContentService.SaveAndPublishWithStatus(purchaseHistory);

            #endregion

            #region Restrict Access to the Account page

            // Protect the page
            // OLD > Access.ProtectPage(false, account.Id, registration.Id, registration.Id);
            var entry = new PublicAccessEntry(account, registration, registration, new List<PublicAccessRule>());
            ApplicationContext.Current.Services.PublicAccessService.Save(entry);

            // Add the role to the document
            //Old > Access.AddMembershipRoleToDocument(account.Id, "MerchelloCustomers");
            ApplicationContext.Current.Services.PublicAccessService.AddRule(account,
                Umbraco.Core.Constants.Conventions.PublicAccess.MemberRoleRuleType,
                "MerchelloCustomers"); 

            #endregion

            //// TODO figure out why the index does not build on load
            //LogHelper.Info<BazaarDataInstaller>("Rebuilding Product Index");
            //ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();

            return storeRoot;
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

            #region Warehouse

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

            #endregion

            #region Shipping Data

            LogHelper.Info<BazaarDataInstaller>("Adding example shipping data");
            var catalog =
                warehouse.WarehouseCatalogs.FirstOrDefault(
                    x => x.Key == Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey);
            var country = merchelloServices.StoreSettingService.GetCountryByCode("US");

            // The following is internal to Merchello and not exposed in the public API
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

            #endregion

            #region Collections

            // Add the product collections
            LogHelper.Info<BazaarDataInstaller>("Adding example product collections");

            // Create a featured products with home page under it

            var featuredProducts =
                merchelloServices.EntityCollectionService.CreateEntityCollectionWithKey(
                    EntityType.Product,
                    Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                    "Featured Products");

            var homePage = merchelloServices.EntityCollectionService.CreateEntityCollectionWithKey(
                    EntityType.Product,
                    Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                    "Home Page");
            homePage.ParentKey = featuredProducts.Key;

            // Create a main categories collection with Funny, Geeky and Sad under it

            var mainCategories =
                merchelloServices.EntityCollectionService.CreateEntityCollectionWithKey(
                    EntityType.Product,
                    Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                    "Main Categories");

            var funny = merchelloServices.EntityCollectionService.CreateEntityCollection(
                EntityType.Product,
                Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                "Funny");
            funny.ParentKey = mainCategories.Key;

            var geeky = merchelloServices.EntityCollectionService.CreateEntityCollection(
                    EntityType.Product,
                    Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                    "Geeky");
            geeky.ParentKey = mainCategories.Key;

            var sad = merchelloServices.EntityCollectionService.CreateEntityCollection(
                        EntityType.Product,
                        Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                        "Geeky");
            sad.ParentKey = mainCategories.Key;

            // Save the collections
            merchelloServices.EntityCollectionService.Save(new[] { mainCategories, funny, geeky, sad });

            // Add the collections to the collections dictionary
            _collections.Add(CollectionFeaturedProducts, featuredProducts.Key);
            _collections.Add(CollectionHomePage, homePage.Key);
            _collections.Add(CollectionMainCategories, mainCategories.Key);
            _collections.Add(CollectionFunny, funny.Key);
            _collections.Add(CollectionGeeky, geeky.Key);
            _collections.Add(CollectionSad, sad.Key);

            #endregion

            #region Detached Content

            // Add the detached content type
            LogHelper.Info<BazaarDataInstaller>("Getting information for detached content");

            var contentType = _services.ContentTypeService.GetContentType("BazaarProductContent");
            var detachedContentTypeService =
                ((Core.Services.ServiceContext)merchelloServices).DetachedContentTypeService;
            var detachedContentType = detachedContentTypeService.CreateDetachedContentType(
                EntityType.Product,
                contentType.Key,
                "Bazaar Product");

            // Detached Content
            detachedContentType.Description = "Default Bazaar Product Content";
            detachedContentTypeService.Save(detachedContentType);

            #endregion

            #region Adding 6 Example Products

            // Product data fields
            const string productOverview = @"""<ul>
                                            <li>Leberkas strip steak tri-tip</li>
                                            <li>flank ham drumstick</li>
                                            <li>Bacon pastrami turkey</li>
                                            </ul>""";
            const string productDescription = @"""<p>Bacon ipsum dolor amet flank cupim filet mignon shoulder andouille kielbasa sirloin bacon spare ribs pork biltong rump ham. Meatloaf turkey tail, 
                                                        shoulder short loin capicola porchetta fatback. Jowl tenderloin sausage shank pancetta tongue.</p>
                                                <p>Ham hock spare ribs cow turducken porchetta corned beef, pastrami leberkas biltong meatloaf bacon shankle ribeye beef ribs. Picanha ham hock chicken 
                                                        biltong, ground round jowl meatloaf bacon short ribs tongue shoulder.</p>""";

            #region Add Product - Despite Shirt

            LogHelper.Info<BazaarDataInstaller>("Adding an example product - Despite Shirt");

            var despiteShirt = merchelloServices.ProductService.CreateProduct("Despite Shirt", "shrt-despite", 7.39M);
            despiteShirt.Shippable = true;
            despiteShirt.OnSale = false;
            despiteShirt.SalePrice = 6M;
            despiteShirt.CostOfGoods = 4M;
            despiteShirt.Taxable = true;
            despiteShirt.TrackInventory = false;
            despiteShirt.Available = true;
            despiteShirt.Weight = 1M;
            despiteShirt.AddToCatalogInventory(catalog);
            merchelloServices.ProductService.Save(despiteShirt, false);

            // add to collections
            despiteShirt.AddToCollection(funny);
            despiteShirt.AddToCollection(geeky);



            despiteShirt.DetachedContents.Add(
                new ProductVariantDetachedContent(
                    despiteShirt.ProductVariantKey,
                    detachedContentType,
                    "en-US",
                    new DetachedDataValuesCollection(
                        new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("overview", productOverview),
                                new KeyValuePair<string, string>("image", "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1066/despite.jpg\" }")
                            }))
                {
                    CanBeRendered = true
                });

            merchelloServices.ProductService.Save(despiteShirt, false);

            #endregion

            #region Add Product - Element Meh Shirt

            LogHelper.Info<BazaarDataInstaller>("Adding an example product  - Element Meh Shirt");
            var elementMehShirt = merchelloServices.ProductService.CreateProduct("Element Meh Shirt", "tshrt-meh", 12.99M);
            elementMehShirt.OnSale = false;
            elementMehShirt.Shippable = true;
            elementMehShirt.SalePrice = 10M;
            elementMehShirt.CostOfGoods = 8.50M;
            elementMehShirt.Taxable = true;
            elementMehShirt.TrackInventory = false;
            elementMehShirt.Available = true;
            elementMehShirt.Weight = 1M;
            elementMehShirt.AddToCatalogInventory(catalog);
            merchelloServices.ProductService.Save(elementMehShirt, false);

            AddOptionsToProduct(elementMehShirt);
            merchelloServices.ProductService.Save(elementMehShirt, false);

            // add to collections
            elementMehShirt.AddToCollection(featuredProducts);
            elementMehShirt.AddToCollection(geeky);

            elementMehShirt.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   elementMehShirt.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("overview", productOverview),
                                new KeyValuePair<string, string>("image", "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1067/element.jpg\" }")
                            }))
               {
                   CanBeRendered = true
               });

            merchelloServices.ProductService.Save(elementMehShirt, false);

            #endregion

            #region Add Product - Evolution Shirt

            LogHelper.Info<BazaarDataInstaller>("Adding an example product  - Evolution Shirt");
            var evolutionShirt = merchelloServices.ProductService.CreateProduct("Evolution Shirt", "shrt-evo", 13.99M);
            evolutionShirt.OnSale = true;
            evolutionShirt.Shippable = true;
            evolutionShirt.SalePrice = 11M;
            evolutionShirt.CostOfGoods = 10M;
            evolutionShirt.Taxable = true;
            evolutionShirt.TrackInventory = false;
            evolutionShirt.Available = true;
            evolutionShirt.Weight = 1M;
            evolutionShirt.AddToCatalogInventory(catalog);
            merchelloServices.ProductService.Save(evolutionShirt, false);

            AddOptionsToProduct(evolutionShirt);
            merchelloServices.ProductService.Save(evolutionShirt, false);

            // add to collections
            evolutionShirt.AddToCollection(funny);
            evolutionShirt.AddToCollection(geeky);
            evolutionShirt.AddToCollection(homePage);
            evolutionShirt.AddToCollection(sad);

            evolutionShirt.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   evolutionShirt.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("overview", productOverview),
                                new KeyValuePair<string, string>("image", "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1063/evolution.jpg\" }")
                            }))
               {
                   CanBeRendered = true
               });

            merchelloServices.ProductService.Save(evolutionShirt, false);

            #endregion

            #region Add Product - Flea Shirt

            LogHelper.Info<BazaarDataInstaller>("Adding an example product  - Flea Shirt");
            var fleaShirt = merchelloServices.ProductService.CreateProduct("Flea Shirt", "shrt-flea", 11.99M);
            fleaShirt.OnSale = false;
            fleaShirt.Shippable = true;
            fleaShirt.SalePrice = 10M;
            fleaShirt.CostOfGoods = 8.75M;
            fleaShirt.Taxable = true;
            fleaShirt.TrackInventory = false;
            fleaShirt.Available = true;
            fleaShirt.Weight = 1M;
            fleaShirt.AddToCatalogInventory(catalog);
            merchelloServices.ProductService.Save(fleaShirt, false);

            // add to collections
            fleaShirt.AddToCollection(funny);
            fleaShirt.AddToCollection(homePage);

            fleaShirt.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   fleaShirt.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("overview", productOverview),
                                new KeyValuePair<string, string>("image", "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1068/dog-fleas.jpg\" }")
                            }))
               {
                   CanBeRendered = true
               });

            merchelloServices.ProductService.Save(fleaShirt, false);

            #endregion

            #region Add Product - Paranormal Shirt

            LogHelper.Info<BazaarDataInstaller>("Adding an example product  - Paranormal Shirt");
            var paranormalShirt = merchelloServices.ProductService.CreateProduct("Paranormal Shirt", "shrt-para", 14.99M);
            paranormalShirt.OnSale = false;
            paranormalShirt.Shippable = true;
            paranormalShirt.SalePrice = 12M;
            paranormalShirt.CostOfGoods = 9.75M;
            paranormalShirt.Taxable = true;
            paranormalShirt.TrackInventory = false;
            paranormalShirt.Available = true;
            paranormalShirt.Weight = 1M;
            paranormalShirt.AddToCatalogInventory(catalog);
            merchelloServices.ProductService.Save(paranormalShirt, false);

            // add to collections
            paranormalShirt.AddToCollection(geeky);
            paranormalShirt.AddToCollection(homePage);

            paranormalShirt.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   paranormalShirt.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("overview", productOverview),
                                new KeyValuePair<string, string>("image", "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1065/paranormal.jpg\" }")
                            }))
               {
                   CanBeRendered = true
               });

            merchelloServices.ProductService.Save(paranormalShirt, false);

            #endregion

            #region Add Product - Plan Ahead Shirt

            LogHelper.Info<BazaarDataInstaller>("Adding an example product  - Plan Ahead Shirt");
            var planAheadShirt = merchelloServices.ProductService.CreateProduct("Plan Ahead Shirt", "shrt-plan", 8.99M);
            planAheadShirt.OnSale = true;
            planAheadShirt.Shippable = true;
            planAheadShirt.SalePrice = 7.99M;
            planAheadShirt.CostOfGoods = 4M;
            planAheadShirt.Taxable = true;
            planAheadShirt.TrackInventory = false;
            planAheadShirt.Available = true;
            planAheadShirt.Weight = 1M;
            planAheadShirt.AddToCatalogInventory(catalog);
            merchelloServices.ProductService.Save(planAheadShirt, false);

            // add to collections
            planAheadShirt.AddToCollection(geeky);

            //// {"Key":"relatedProducts","Value":"[\r\n  \"a2d7c2c0-ebfa-4b8b-b7cb-eb398a24c83d\",\r\n  \"86e3c576-3f6f-45b8-88eb-e7b90c7c7074\"\r\n]"}

            planAheadShirt.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   planAheadShirt.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("overview", productOverview),
                                new KeyValuePair<string, string>("image", "{ \"focalPoint\": { \"left\": 0.5, \"top\": 0.5 }, \"src\": \"/media/1069/planahead.jpg\" }"),
                                new KeyValuePair<string, string>("relatedProucts", string.Format("[ \"{0}\", \"{1}\"]", paranormalShirt.Key, elementMehShirt.Key))
                            }))
               {
                   CanBeRendered = true
               });

            merchelloServices.ProductService.Save(planAheadShirt, false);

            #endregion 

            #endregion

        }

        /// <summary>
        /// Adds example options to product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        private void AddOptionsToProduct(IProduct product)
        {
            product.ProductOptions.Add(new ProductOption("Color"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Blue", "Blue"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Red", "Red"));
            product.ProductOptions.First(x => x.Name == "Color").Choices.Add(new ProductAttribute("Green", "Green"));
            product.ProductOptions.Add(new ProductOption("Size"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Small", "Small"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Medium", "Medium"));
            product.ProductOptions.First(x => x.Name == "Size").Choices.Add(new ProductAttribute("Large", "Large"));
        }
    }
}