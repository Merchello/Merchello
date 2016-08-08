namespace Merchello.FastTrack.Install
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

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
        /// The templates.
        /// </summary>
        private readonly IEnumerable<ITemplate> _templates;

        /// <summary>
        /// The collections.
        /// </summary>
        private readonly IDictionary<string, Guid> _collections = new Dictionary<string, Guid>();

        /// <summary>
        /// The example media.
        /// </summary>
        private readonly IDictionary<string, string> _media = new Dictionary<string, string>();

        /// <summary>
        /// The shared product options.
        /// </summary>
        private readonly IDictionary<string, IProductOption> _sharedOptions = new Dictionary<string, IProductOption>();

        /// <summary>
        /// The member type.
        /// </summary>
        private IMemberType _memberType;

        #endregion



        /// <summary>
        /// Initializes a new instance of the <see cref="FastTrackDataInstaller"/> class.
        /// </summary>
        public FastTrackDataInstaller()
        {
            _services = ApplicationContext.Current.Services;

            var templates = new[] { "ftProduct", "ftPayment", "ftPaymentMethod", "ftBillingAddress", "ftShipRateQuote", "ftShippingAddress" };

            _templates = ApplicationContext.Current.Services.FileService.GetTemplates(templates);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <returns>
        /// The <see cref="IContent"/>.
        /// </returns>
        public IContent Execute()
        {
            MultiLogHelper.Info<FastTrackDataInstaller>("Adding MemberType");
            _memberType = this.AddMemberType();

            MultiLogHelper.Info<FastTrackDataInstaller>("Adding example media");
            this.AddExampleMedia();

            MultiLogHelper.Info<FastTrackDataInstaller>("Adding shared options");
            this.CreateSharedOptions();

            // Adds the Merchello Data
            MultiLogHelper.Info<FastTrackDataInstaller>("Starting to add example FastTrack data");
            this.AddMerchelloData();

            // Adds the example Umbraco data
            MultiLogHelper.Info<FastTrackDataInstaller>("Starting to add example Merchello Umbraco data");
            return this.AddUmbracoData();
        }

        /// <summary>
        /// Adds the Merchello MemberType.
        /// </summary>
        /// <returns>
        /// The <see cref="IMemberType"/>.
        /// </returns>
        private IMemberType AddMemberType()
        {
            var dtd = _services.DataTypeService.GetDataTypeDefinitionById(-88);

            // Create the MerchelloCustomer MemberType
            var mt = new MemberType(-1)
            {
                Alias = "merchelloCustomer",
                Name = "Merchello Customer",
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


            // Add the MemberGroup
            var mg = new MemberGroup() { Name = "Customers" };

            _services.MemberGroupService.Save(mg);

            return mt;
        }

        /// <summary>
        /// The add example media.
        /// </summary>
        private void AddExampleMedia()
        {
            var folderType = Umbraco.Core.Constants.Conventions.MediaTypes.Folder;
           
            var exampleDir = HttpContext.Current.Server.MapPath("~/App_Plugins/FastTrack/Install/images");
            var dir = new DirectoryInfo(exampleDir);
            var files = dir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);

            if (files.Any())
            {
                var root = _services.MediaService.CreateMediaWithIdentity("Example", -1, folderType);
                AddMediaFile(files, root, "despite.jpg", "despite");
                AddMediaFile(files, root, "dog-fleas.jpg", "fleas");
                AddMediaFile(files, root, "element.jpg", "element");
                AddMediaFile(files, root, "evolution.jpg", "evolution");
                AddMediaFile(files, root, "paranormal.jpg", "paranormal");
                AddMediaFile(files, root, "planahead.jpg", "planahead");
            }
        }

        /// <summary>
        /// Adds the Umbraco content.
        /// </summary>
        /// <returns>
        /// The <see cref="IContent"/>.
        /// </returns>
        private IContent AddUmbracoData()
        {

            MultiLogHelper.Info<FastTrackDataInstaller>("Install MemberType");



            // Create the store root and add the initial data

            MultiLogHelper.Info<FastTrackDataInstaller>("Installing store root node");

            var storeRoot = _services.ContentService.CreateContent("Store", -1, "ftStore");

            storeRoot.SetValue("storeName", "FastTrack Store");
            storeRoot.SetValue("brief", @"<p>Example store which has been developed to help get you up and running quickly with Merchello. 
                                            It's designed to show you how to implement common features, and you can grab the source code from here, just fork/clone/download and open up Merchello.sln</p>");
            storeRoot.SetValue("featuredProducts", _collections["collectionFeaturedProducts"].ToString());

            _services.ContentService.SaveAndPublishWithStatus(storeRoot);


            // Add the example categories
            LogHelper.Info<FastTrackDataInstaller>("Adding example category page");

            // Create the root T-Shirt category
            var catalog = _services.ContentService.CreateContent("Catalog", storeRoot.Id, "ftCatalog");

            catalog.SetValue("categories", _collections[CollectionMainCategories].ToString());
            _services.ContentService.SaveAndPublishWithStatus(catalog);

            // Create the sun categories
            var funnyTShirts = _services.ContentService.CreateContent("Funny T-Shirts", catalog.Id, "ftCategory");
            funnyTShirts.SetValue("products", _collections[CollectionFunny].ToString());
            _services.ContentService.SaveAndPublishWithStatus(funnyTShirts);

            var geekyTShirts = _services.ContentService.CreateContent("Geeky T-Shirts", catalog.Id, "ftCategory");
            geekyTShirts.SetValue("products", _collections[CollectionGeeky].ToString());
            _services.ContentService.SaveAndPublishWithStatus(geekyTShirts);

            var sadTShirts = _services.ContentService.CreateContent("Sad T-Shirts", catalog.Id, "ftCategory");
            sadTShirts.SetValue("products", _collections[CollectionSad].ToString());
            _services.ContentService.SaveAndPublishWithStatus(sadTShirts);



            MultiLogHelper.Info<FastTrackDataInstaller>("Adding example eCommerce workflow pages");

            var basket = _services.ContentService.CreateContent("Basket", storeRoot.Id, "ftBasket");
            _services.ContentService.SaveAndPublishWithStatus(basket);

            var checkout = _services.ContentService.CreateContent("Checkout", storeRoot.Id, "ftCheckout");
            checkout.Template = _templates.FirstOrDefault(x => x.Alias == "BillingAddress");
            checkout.SetValue("checkoutStage", "BillingAddress");
            _services.ContentService.SaveAndPublishWithStatus(checkout);

            var checkoutShipping = _services.ContentService.CreateContent("Shipping Address", checkout.Id, "ftCheckout");
            checkoutShipping.Template = _templates.FirstOrDefault(x => x.Alias == "ShippingAddress");
            checkoutShipping.SetValue("checkoutStage", "ShippingAddress");
            _services.ContentService.SaveAndPublishWithStatus(checkoutShipping);

            var checkoutShipRateQuote = _services.ContentService.CreateContent("Ship Rate Quote", checkout.Id, "ftCheckout");
            checkoutShipRateQuote.Template = _templates.FirstOrDefault(x => x.Alias == "ShipRateQuote");
            checkoutShipRateQuote.SetValue("checkoutStage", "ShipRateQuote");
            _services.ContentService.SaveAndPublishWithStatus(checkoutShipRateQuote);

            var checkoutPaymentMethod = _services.ContentService.CreateContent("Payment Method", checkout.Id, "ftCheckout");
            checkoutPaymentMethod.Template = _templates.FirstOrDefault(x => x.Alias == "PaymentMethod");
            checkoutPaymentMethod.SetValue("checkoutStage", "PaymentMethod");
            _services.ContentService.SaveAndPublishWithStatus(checkoutPaymentMethod);

            var checkoutPayment = _services.ContentService.CreateContent("Payment", checkout.Id, "ftCheckout");
            checkoutPayment.Template = _templates.FirstOrDefault(x => x.Alias == "Payment");
            checkoutPayment.SetValue("checkoutStage", "Payment");
            _services.ContentService.SaveAndPublishWithStatus(checkoutPayment);

            var receipt = _services.ContentService.CreateContent("Receipt", storeRoot.Id, "ftReceipt");
            _services.ContentService.SaveAndPublishWithStatus(receipt);

            var login = _services.ContentService.CreateContent("Login", storeRoot.Id, "ftLogin");
            _services.ContentService.SaveAndPublishWithStatus(login);

            var account = _services.ContentService.CreateContent("Account", storeRoot.Id, "ftAccount");
            _services.ContentService.SaveAndPublishWithStatus(account);


            //// Protect the page
            var entry = new PublicAccessEntry(account, login, login, new List<PublicAccessRule>());
            ApplicationContext.Current.Services.PublicAccessService.Save(entry);

            //// Add the role to the document
            ApplicationContext.Current.Services.PublicAccessService.AddRule(account, Umbraco.Core.Constants.Conventions.PublicAccess.MemberRoleRuleType, "Customers");

            return storeRoot;
        }

        /// <summary>
        /// Creates shared options.
        /// </summary>
        private void CreateSharedOptions()
        {
            var productOptionService = MerchelloContext.Current.Services.ProductOptionService;

            var size = new ProductOption("Size") { Shared = true, UiOption = "select" };
            size.Choices.Add(new ProductAttribute("Small", "sm") { SortOrder = 1, IsDefaultChoice = false });
            size.Choices.Add(new ProductAttribute("Medium", "md") { SortOrder = 2, IsDefaultChoice = false });
            size.Choices.Add(new ProductAttribute("Large", "lg") { SortOrder = 3, IsDefaultChoice = true });
            size.Choices.Add(new ProductAttribute("X-Large", "xl") { SortOrder = 4, IsDefaultChoice = false });

            var colour = new ProductOption("Colour") { Shared = true, UiOption = "select" };
            colour.Choices.Add(new ProductAttribute("White", "white") { SortOrder = 1, IsDefaultChoice = false });
            colour.Choices.Add(new ProductAttribute("Black", "black") { SortOrder = 2, IsDefaultChoice = false });
            colour.Choices.Add(new ProductAttribute("Grey", "grey") { SortOrder = 3, IsDefaultChoice = true });
            colour.Choices.Add(new ProductAttribute("Blue", "blue") { SortOrder = 4, IsDefaultChoice = false });

            productOptionService.Save(size);
            productOptionService.Save(colour);

            _sharedOptions.Add("size", size);
            _sharedOptions.Add("colour", colour);
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


            MultiLogHelper.Info<FastTrackDataInstaller>("Adding example shipping data");
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


            // Create a main categories collection with Funny, Geeky and Sad under it

            var mainCategories =
                merchelloServices.EntityCollectionService.CreateEntityCollectionWithKey(
                    EntityType.Product,
                    Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                    "T-Shirts");

            var funny = merchelloServices.EntityCollectionService.CreateEntityCollection(
                EntityType.Product,
                Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                "Funny T-Shirts");
            funny.ParentKey = mainCategories.Key;

            var geeky = merchelloServices.EntityCollectionService.CreateEntityCollection(
                    EntityType.Product,
                    Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                    "Geeky T-Shirts");
            geeky.ParentKey = mainCategories.Key;

            var sad = merchelloServices.EntityCollectionService.CreateEntityCollection(
                        EntityType.Product,
                        Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey,
                        "Sad T-Shirts");
            sad.ParentKey = mainCategories.Key;

            // Save the collections
            merchelloServices.EntityCollectionService.Save(new[] { mainCategories, funny, geeky, sad });

            // Add the collections to the collections dictionary
            _collections.Add(CollectionFeaturedProducts, featuredProducts.Key);
            _collections.Add(CollectionMainCategories, mainCategories.Key);
            _collections.Add(CollectionFunny, funny.Key);
            _collections.Add(CollectionGeeky, geeky.Key);
            _collections.Add(CollectionSad, sad.Key);



            // Add the detached content type
            LogHelper.Info<FastTrackDataInstaller>("Getting information for detached content");

            var contentType = _services.ContentTypeService.GetContentType("ftProduct");
            var detachedContentTypeService =
                ((Core.Services.ServiceContext)merchelloServices).DetachedContentTypeService;
            var detachedContentType = detachedContentTypeService.CreateDetachedContentType(
                EntityType.Product,
                contentType.Key,
                "Product");

            // Detached Content
            detachedContentType.Description = "Default Product Content";
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

            var template = _templates.FirstOrDefault(x => x.Alias == "ftProduct");
            var templateId = template != null ? template.Id : 0;

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


            var despiteImg = _media.ContainsKey("despite") ? _media["despite"] : string.Empty;

            despiteShirt.DetachedContents.Add(
                new ProductVariantDetachedContent(
                    despiteShirt.ProductVariantKey,
                    detachedContentType,
                    "en-US",
                    new DetachedDataValuesCollection(
                        new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("brief", productOverview),
                                new KeyValuePair<string, string>("image", despiteImg)
                            }))
                {
                    CanBeRendered = true
                });

            SetTemplateAndSave(despiteShirt, templateId);



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

            var elementImg = _media.ContainsKey("element") ? _media["element"] : string.Empty;

            elementMehShirt.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   elementMehShirt.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("brief", productOverview),
                                new KeyValuePair<string, string>("image", elementImg)
                            }))
               {
                   CanBeRendered = true
               });

            SetTemplateAndSave(elementMehShirt, templateId);


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

            var evolutionImg = _media.ContainsKey("evolution") ? _media["evolution"] : string.Empty;

            // add to collections
            evolutionShirt.AddToCollection(funny);
            evolutionShirt.AddToCollection(geeky);
            evolutionShirt.AddToCollection(featuredProducts);
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
                                new KeyValuePair<string, string>("brief", productOverview),
                                new KeyValuePair<string, string>("image", evolutionImg)
                            }))
               {
                   CanBeRendered = true
               });

            SetTemplateAndSave(evolutionShirt, templateId);


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
            fleaShirt.AddToCollection(featuredProducts);

            var fleasImg = _media.ContainsKey("fleas") ? _media["fleas"] : string.Empty;

            fleaShirt.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   fleaShirt.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("brief", productOverview),
                                new KeyValuePair<string, string>("image", fleasImg)
                            }))
               {
                   CanBeRendered = true
               });

            SetTemplateAndSave(fleaShirt, templateId);



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
            paranormalShirt.AddToCollection(featuredProducts);

            var paraImg = _media.ContainsKey("paranormal") ? _media["paranormal"] : string.Empty;

            paranormalShirt.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   paranormalShirt.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("brief", productOverview),
                                new KeyValuePair<string, string>("image", paraImg)
                            }))
               {
                   CanBeRendered = true
               });

            SetTemplateAndSave(paranormalShirt, templateId);


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

            var planImg = _media.ContainsKey("planahead") ? _media["planahead"] : string.Empty;

            planAheadShirt.DetachedContents.Add(
               new ProductVariantDetachedContent(
                   planAheadShirt.ProductVariantKey,
                   detachedContentType,
                   "en-US",
                   new DetachedDataValuesCollection(
                       new[]
                            {
                                new KeyValuePair<string, string>("description", productDescription),
                                new KeyValuePair<string, string>("brief", productOverview),
                                new KeyValuePair<string, string>("image", planImg),
                                new KeyValuePair<string, string>("relatedProucts", string.Format("[ \"{0}\", \"{1}\"]", paranormalShirt.Key, elementMehShirt.Key))
                            }))
               {
                   CanBeRendered = true
               });

            SetTemplateAndSave(planAheadShirt, templateId);
        }

        /// <summary>
        /// Adds example options to product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        private void AddOptionsToProduct(IProduct product)
        {
            foreach (var option in _sharedOptions)
            {
                product.ProductOptions.Add(option.Value);
            }
        }

        /// <summary>
        /// Adds the example media files.
        /// </summary>
        /// <param name="files">
        /// The files.
        /// </param>
        /// <param name="root">
        /// The root (example folder).
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        private void AddMediaFile(FileInfo[] files, IMedia root, string fileName, string alias)
        {
            var fileType = Umbraco.Core.Constants.Conventions.MediaTypes.Image;
            var f = files.FirstOrDefault(x => x.Name == fileName);
            if (f != null)
            {
                var m = _services.MediaService.CreateMedia(fileName, root, fileType);
                m.SetValue("umbracoFile", fileName, f.Open(FileMode.Open));
                _services.MediaService.Save(m);
                _media.Add(alias, m.Id.ToString());
            }
        }

        private void SetTemplateAndSave(IProduct product, int templateId)
        {
            if (templateId > 0)
            {
                foreach (var dc in product.DetachedContents)
                {
                    dc.TemplateId = templateId;
                }
            }

            MerchelloContext.Current.Services.ProductService.Save(product, false);
        }
    }
}