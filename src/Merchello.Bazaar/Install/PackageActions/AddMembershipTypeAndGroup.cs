namespace Merchello.Bazaar.Install.PackageActions
{
    using System.Xml;

    using umbraco.cms.businesslogic.packager.standardPackageActions;
    using umbraco.interfaces;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// A package installation package action to install a MemberType and MemberGroup.
    /// </summary>
    public class AddMembershipTypeAndGroup : IPackageAction
    {
        /// <summary>
        /// The member type name.
        /// </summary>
        private const string MemberTypeName = "MerchelloCustomer";

        /// <summary>
        /// The member group name.
        /// </summary>
        private const string MemberGroupName = "MemberGroupName";

        /// <summary>
        /// The Umbraco <see cref="ServiceContext"/>
        /// </summary>
        private readonly ServiceContext _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddMembershipTypeAndGroup"/> class.
        /// </summary>
        public AddMembershipTypeAndGroup()
        {
            _services = ApplicationContext.Current.Services;
        }

        /// <summary>
        /// The alias.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Alias()
        {
            return "MerchelloBazaar_AddMembershipTypeAndGroup";
        }

        /// <summary>
        /// Performs the package action during package installation
        /// </summary>
        /// <param name="packageName">
        /// The package name.
        /// </param>
        /// <param name="xmlData">
        /// The xml data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> indicating success of failure.
        /// </returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            var dtd = _services.DataTypeService.GetDataTypeDefinitionById(-88);

            // Create the MerchelloCustomer MemberType
            var mt = new MemberType(-1)
            {
                Alias = MemberTypeName,
                Name = MemberTypeName,
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
            var mg = new MemberGroup() { Name = MemberGroupName };

            _services.MemberGroupService.Save(mg);

            return true;
        }

        /// <summary>
        /// The undo - removes the MemberType and MemberGroup during uninstall
        /// </summary>
        /// <param name="packageName">
        /// The package name.
        /// </param>
        /// <param name="xmlData">
        /// The xml data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Undo(string packageName, XmlNode xmlData)
        {
            //// http://issues.merchello.com/youtrack/issue/M-968
            return true;
            //// remove the MemberType
            //var mt = _services.MemberTypeService.Get(MemberTypeName);
            //if (mt != null) _services.MemberTypeService.Delete(mt);

            //// remove the MemberGroup
            //var mg = _services.MemberGroupService.GetByName(MemberGroupName);
            //if (mg != null) _services.MemberGroupService.Delete(mg);

            //return true;
        }

        /// <summary>
        /// The sample xml.
        /// </summary>
        /// <returns>
        /// The <see cref="XmlNode"/>.
        /// </returns>
        public XmlNode SampleXml()
        {
            const string Sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"MerchelloBazaar_AddMembershipTypeAndGroup\"></Action>";

            return helper.parseStringToXmlNode(Sample);
        }

    }
}