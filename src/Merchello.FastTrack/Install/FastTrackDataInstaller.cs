namespace Merchello.FastTrack.Install
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping.FixedRate;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// A class responsible for installing FastTrack example site default data.
    /// </summary>
    internal class FastTrackDataInstaller
    {
        #region "Private fields"

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

        #endregion

        /// <summary>
        /// The collections.
        /// </summary>
        /// <remarks>
        /// Introduced in 1.12.0
        /// </remarks>
        private readonly IDictionary<string, Guid> _collections = new Dictionary<string, Guid>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FastTrackDataInstaller"/> class.
        /// </summary>
        public FastTrackDataInstaller()
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
            MultiLogHelper.Info<FastTrackDataInstaller>("Starting to add example FastTrack data");
            

            // Adds the example Umbraco data
            MultiLogHelper.Info<FastTrackDataInstaller>("Starting to add example Merchello Umbraco data");
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
            throw new NotImplementedException();
        }



        /// <summary>
        /// The add merchello data.
        /// </summary>
        private void AddMerchelloData()
        {
            if (!MerchelloContext.HasCurrent)
            {
                LogHelper.Info<FastTrackDataInstaller>("MerchelloContext was null");
            }

            var merchelloServices = MerchelloContext.Current.Services;


            LogHelper.Info<FastTrackDataInstaller>("Updating Default Warehouse Address");
            var warehouse = merchelloServices.WarehouseService.GetDefaultWarehouse();
            warehouse.Name = "Merchello";
            warehouse.Address1 = "114 W. Magnolia St.";
            warehouse.Locality = "Bellingham";
            warehouse.Region = "WA";
            warehouse.PostalCode = "98225";
            warehouse.CountryCode = "US";
            merchelloServices.WarehouseService.Save(warehouse);


            LogHelper.Info<FastTrackDataInstaller>("Adding example shipping data");
            var catalog =
                warehouse.WarehouseCatalogs.FirstOrDefault(
                    x => x.Key == Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey);
            var country = merchelloServices.StoreSettingService.GetCountryByCode("US");

            // The following is internal to Merchello and not exposed in the public API
            var shipCountry = merchelloServices.ShipCountryService.GetShipCountryByCountryCode(
                    catalog.Key,
                    "US");

            // Add the ship country
            if (shipCountry == null || shipCountry.CountryCode == "ELSE")
            {
                shipCountry = new ShipCountry(catalog.Key, country);
                merchelloServices.ShipCountryService.Save(shipCountry);
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
            LogHelper.Info<FastTrackDataInstaller>("Adding example product collections");

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



            // Add the detached content type
            LogHelper.Info<FastTrackDataInstaller>("Getting information for detached content");

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


            LogHelper.Info<FastTrackDataInstaller>("Adding an example product - Despite Shirt");

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



            LogHelper.Info<FastTrackDataInstaller>("Adding an example product  - Element Meh Shirt");
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


            LogHelper.Info<FastTrackDataInstaller>("Adding an example product  - Evolution Shirt");
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


            LogHelper.Info<FastTrackDataInstaller>("Adding an example product  - Flea Shirt");
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



            LogHelper.Info<FastTrackDataInstaller>("Adding an example product  - Paranormal Shirt");
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


            LogHelper.Info<FastTrackDataInstaller>("Adding an example product  - Plan Ahead Shirt");
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