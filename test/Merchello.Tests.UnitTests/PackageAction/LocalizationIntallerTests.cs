using System;
using System.IO;
using System.Linq;
using System.Xml;
using Merchello.Web.PackageActions;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Tests.UnitTests.PackageAction
{
    [TestFixture]
    public class LocalizationIntallerTests
    {
        [Test]
        public void Debug()
        {
            var merchelloFiles = AddLocalizationAreas.GetMerchelloLanguageFiles();
            var merchFileArray = merchelloFiles as FileInfo[] ?? merchelloFiles.ToArray();

            var existingLangs = AddLocalizationAreas.GetUmbracoLanguageFilesToInsertLocalizationData();

            foreach (var lang in existingLangs)
            {
                var merch = new XmlDocument(){ PreserveWhitespace = true };
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
                            var import = umb.ImportNode((XmlNode) area, true);
                            umb.DocumentElement.AppendChild(import);
                        }
                 
                    }
                    
                 
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    LogHelper.Error<AddLocalizationAreas>("Failed to add Merchello localization values to language file", ex);
                }

            }
        }


        [Test]
        public void Can_Get_A_List_Of_All_Umbraco_Lang_Files()
        {
            //// Arrange
            
            //// Act
            var files = AddLocalizationAreas.GetUmbracoLanguageFiles();


            //// Assert
            Assert.IsTrue(files.Any());
            Console.WriteLine(files.Count());
            foreach (var f in files) Console.WriteLine(f.Name);
        }


        [Test]
        public void Can_Get_A_List_Of_All_Merchello_Lang_Files()
        {
            //// Arrange

            //// Act
            var files = AddLocalizationAreas.GetMerchelloLanguageFiles();


            //// Assert
            Assert.IsTrue(files.Any());
            Console.WriteLine(files.Count());

            foreach(var f in files) Console.WriteLine(f.Name);
        }

        [Test]
        public void Can_Get_A_List_Of_All_Language_Files_In_Common_Between_Umbraco_And_Merchello()
        {
            //// Act
            var files = AddLocalizationAreas.GetUmbracoLanguageFilesToInsertLocalizationData();

            //// Assert
           Assert.IsTrue(files.Any());
            Assert.AreEqual(5, files.Count());
            foreach (var f in files) Console.WriteLine(f.Name);
        }
    }
}