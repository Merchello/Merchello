namespace Merchello.Web.PackageActions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Core.Configuration;
    using umbraco.cms.businesslogic.packager.standardPackageActions;    
    using umbraco.interfaces;
    using Umbraco.Core.Logging;

    public class AddLocalizationAreas : IPackageAction
    {
        // Set the path of the language files directory
        private const string UmbracoLangPath = "~/umbraco/config/lang/";
        private const string MerchelloLangPath = "~/App_Plugins/Merchello/Config/Lang/";

        public string Alias()
        {
            return string.Concat(MerchelloConfiguration.ApplicationName, "_AddLocalizationAreas");
        }


        public bool Execute(string packageName, XmlNode xmlData)
        {
            var merchelloFiles = GetMerchelloLanguageFiles();
            LogHelper.Info<AddLocalizationAreas>(string.Format("Merchello Package Acction - {0} Merchello Plugin language files to be merged", merchelloFiles.Count()));

            var merchFileArray = merchelloFiles as FileInfo[] ?? merchelloFiles.ToArray();

            var existingLangs = GetUmbracoLanguageFilesToInsertLocalizationData();
            LogHelper.Info<AddLocalizationAreas>(string.Format("Merchello Package Acction - {0} Umbraco language files to that match", existingLangs.Count()));

            foreach (var lang in existingLangs)
            {
                var merch = new XmlDocument() { PreserveWhitespace = true };
                var umb = new XmlDocument() { PreserveWhitespace = true };
                try
                {

                    var match = merchFileArray.FirstOrDefault(x => x.Name == lang.Name);
                    if (match != null)
                    {
                        merch.LoadXml(File.ReadAllText(match.FullName));
                        umb.LoadXml(File.ReadAllText(lang.FullName));

                        // get all of the areas from merch
                        var areas = merch.DocumentElement.SelectNodes("//area");

                        foreach (var area in areas)
                        {
                            var import = umb.ImportNode((XmlNode)area, true);
                            umb.DocumentElement.AppendChild(import);
                        }
                        umb.Save(lang.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    LogHelper.Error<AddLocalizationAreas>("Failed to add Merchello localization values to language file", ex);
                    return false;
                }

            }
            return true;
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            var merchelloFiles = GetMerchelloLanguageFiles();
            var merchFileArray = merchelloFiles as FileInfo[] ?? merchelloFiles.ToArray();

            var existingLangs = GetUmbracoLanguageFilesToInsertLocalizationData();

            foreach (var lang in existingLangs)
            {
                var merch = new XmlDocument() { PreserveWhitespace = true };
                var umb = new XmlDocument() { PreserveWhitespace = true };
                try
                {

                    var match = merchFileArray.FirstOrDefault(x => x.Name == lang.Name);
                    if (match != null)
                    {
                        merch.LoadXml(File.ReadAllText(match.FullName));
                        umb.LoadXml(File.ReadAllText(lang.FullName));

                        // get all of the areas from merch
                        var areas = merch.DocumentElement.SelectNodes("//area");

                        foreach (var area in areas)
                        {
                            var import = umb.ImportNode((XmlNode)area, true);
                            var child =
                                umb.DocumentElement.SelectSingleNode(string.Format("//area[@alias='{0}']",
                                    ((XmlElement) area).GetAttribute("alias")));

                            if(child != null)
                            umb.DocumentElement.RemoveChild(child);
                        }
                        umb.Save(lang.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    LogHelper.Error<AddLocalizationAreas>("Failed to remove Merchello localization values to language file", ex);
                    return false;
                    
                }

            }
            return true;
        }

        public XmlNode SampleXml()
        {
            var xml = string.Concat("<Action runat=\"install\" undo=\"true\" alias=\"", Alias(), "\" />");
            return helper.parseStringToXmlNode(xml);
        }

        internal static IEnumerable<FileInfo> GetUmbracoLanguageFiles()
        {
            var di = new DirectoryInfo(string.Format("{0}{1}", RootPath(), UmbracoLangPath.Replace("~", string.Empty).Replace("/", "\\")));
            return di.GetFiles("*.xml");
        }

        internal static IEnumerable<FileInfo> GetMerchelloLanguageFiles()
        {
            var di = new DirectoryInfo(string.Format("{0}{1}", RootPath(), MerchelloLangPath.Replace("~", string.Empty).Replace("/", "\\")));
            return di.GetFiles("*.xml");
        }


        internal static IEnumerable<FileInfo> GetUmbracoLanguageFilesToInsertLocalizationData()
        {
            return GetUmbracoLanguageFiles().Where(x => GetMerchelloLanguageFiles().Any(y => y.Name == x.Name));
        }


        internal static string RootPath()
        {
            return MerchelloConfiguration.Current.GetRootDirectorySafe();
        }
    }
}