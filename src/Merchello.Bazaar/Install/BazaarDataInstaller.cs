namespace Merchello.Bazaar.Install
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Web.Security;
    using System.Web.UI;

    using Merchello.Core;
    using Merchello.Core.Models;

    using umbraco;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Security;

    /// <summary>
    /// The bazaar data installer.
    /// </summary>
    public class BazaarDataInstaller
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
            root.SetValue("storeTitle", "Merchello Bazaar");
            root.SetValue("tagLine", "Get Shopping");


            _services.ContentService.SaveAndPublishWithStatus(root);

            LogHelper.Info<BazaarDataInstaller>("Adding Example ProductGroup and Products");
            var pg = _services.ContentService.CreateContent("Soap", root.Id, "BazaarProductGroup");
            pg.SetValue("image", @"{  'focalPoint': { 'left': 0.5, 'top': 0.5 }, 'src': '/media/1005/greengoggles1.jpg', 'crops': [] }'");
            pg.SetValue("brief", "Green goggles are not really soap - so this is a placeholder.");
            _services.ContentService.SaveAndPublishWithStatus(pg);

            var prod = _services.ContentService.CreateContent("Bar of Soap", pg.Id, "BazaarProduct");
            prod.SetValue("merchelloProduct", product.Key.ToString());
            prod.SetValue("description", "<p>Vice Truffaut normcore, Portland narwhal High Life direct trade DIY swag viral 90's health goth gluten-free. Austin drinking vinegar Truffaut small batch selfies bicycle rights. Blog forage taxidermy, chia Truffaut pug selfies deep v post-ironic. Scenester Schlitz church-key taxidermy Shoreditch biodiesel. Pug raw denim bitters, health goth DIY Carles meggings pop-up chia ugh. Aesthetic Portland mustache vegan you probably haven't heard of them retro fap hoodie before they sold out cliche tote bag next level. Hoodie raw denim selvage farm-to-table, Thundercats chia mumblecore distillery.</p><p>Tilde letterpress brunch gluten-free lumbersexual sartorial. Intelligentsia lomo lumbersexual hoodie, craft beer XOXO Godard tote bag. Meh seitan photo booth, gentrify normcore hella sartorial salvia letterpress bespoke yr viral freegan. Neutra cardigan vegan, butcher twee raw denim plaid. Post-ironic locavore next level, mustache meggings polaroid fashion axe. Odd Future disrupt hoodie, Williamsburg cornhole Intelligentsia banjo McSweeney's leggings mlkshk. Salvia gluten-free cold-pressed narwhal Kickstarter kitsch, mlkshk photo booth cronut paleo.</p>");
            prod.SetValue("brief", "This is bar soap.");
            prod.SetValue("image", "{ 'focalPoint': { 'left': 0.5, 'top': 0.5 }, 'src': '/media/1006/grpcpbar09.jpg', 'crops': [] }");
            _services.ContentService.SaveAndPublishWithStatus(prod);


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

            LogHelper.Info<BazaarDataInstaller>("Adding an example product");
            var product = merchelloServices.ProductService.CreateProduct("Bar of Soap", "soapbar", 5M);
            product.Shippable = true;
            product.Taxable = true;
            product.TrackInventory = true;
            product.Available = true;
            merchelloServices.ProductService.Save(product);

            return product;
        }

    }
}