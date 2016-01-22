namespace Merchello.Bazaar.Install.PackageActions
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;

    using umbraco.cms.businesslogic.packager.standardPackageActions;
    using umbraco.interfaces;

    /// <summary>
    /// Adds a key to the web.config app settings
    /// </summary>
    /// <remarks>
    /// 
    /// Modified verion of https://packageactioncontrib.codeplex.com/SourceControl/latest#PackageActionsContrib/AddAppConfigKey.cs
    /// 
    /// Original contribution from Paul Sterling
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class AddSslAppConfigKey : AddAppSettingBase, IPackageAction
    {
        #region IPackageAction Members

        /// <summary>
        /// The key.
        /// </summary>
        private const string Key = "Bazaar:RequireSsl";

        /// <summary>
        /// The alias.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Alias()
        {
            return "MerchelloBazaar_AddSslAppConfigKey";
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
                CreateAppSettingsKey(Key, false.ToString());

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
            const string Sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"MerchelloBazaar_AddSslAppConfigKey\"></Action>";

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
