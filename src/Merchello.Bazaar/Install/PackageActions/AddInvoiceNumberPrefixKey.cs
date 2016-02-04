namespace Merchello.Bazaar.Install.PackageActions
{
    using System.Xml;

    using umbraco.cms.businesslogic.packager.standardPackageActions;
    using umbraco.interfaces;

    /// <summary>
    /// A package action to add the InvoiceNumberPrefix app setting.
    /// </summary>
    public class AddInvoiceNumberPrefixKey : AddAppSettingBase, IPackageAction
    {
        #region IPackageAction Members

        /// <summary>
        /// The key.
        /// </summary>
        private const string Key = "Bazaar:InvoiceNumberPrefix";

        /// <summary>
        /// The alias.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Alias()
        {
            return "MerchelloBazaar_AddInvoiceNumberPrefixKey";
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="packageName">
        /// The package name.
        /// </param>
        /// <param name="xmlData">
        /// The xml data.
        /// </param>
        /// <returns>
        /// True or false inicating success.
        /// </returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                CreateAppSettingsKey(Key, "BZR");

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The sample xml.
        /// </summary>
        /// <returns>
        /// The <see cref="XmlNode"/>.
        /// </returns>
        public XmlNode SampleXml()
        {
            const string Sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"MerchelloBazaar_AddInvoiceNumberPrefixKey\"></Action>";

            return helper.parseStringToXmlNode(Sample);
        }

        /// <summary>
        /// The undo.
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
            try
            {
                RemoveAppSettingsKey(Key);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion 
    }
}