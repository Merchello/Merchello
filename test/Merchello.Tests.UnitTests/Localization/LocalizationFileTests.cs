namespace Merchello.Tests.UnitTests.Localization
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Hosting;
    using System.Xml;

    using NUnit.Framework;

    using Rhino.Mocks;

    using Umbraco.Core.IO;

    [TestFixture]
    public class LocalizationFileTests
    {
        private const string LocalizationFileDirectory = "../../src/Merchello.Web.UI/App_Plugins/Merchello/Config/Lang";

        private const string LocalizationSourceDirectory = "../../src/Merchello.Web.UI.Client/src/config/lang";

        private DirectoryInfo _langFileDirectory;

        private DirectoryInfo _langSourceDirectory;

        [TestFixtureSetUp]
        public void Setup()
        {
            var path = IOHelper.MapPath(LocalizationFileDirectory);
            Assert.IsTrue(Directory.Exists(path));

            _langFileDirectory = new DirectoryInfo(path);

            path = IOHelper.MapPath(LocalizationSourceDirectory);
            Assert.IsTrue(Directory.Exists(path));

            _langSourceDirectory = new DirectoryInfo(path);
        }

        [Test]
        public void Can_Verify_That_Language_Files_Do_Not_Contain_Duplicate_Keys()
        {
            var allPassed = true;
            foreach (var f in _langFileDirectory.GetFiles())
            {
                Console.WriteLine("Testing " + f.Name);
                var success = this.TestUniqueKeys(f);
                Console.WriteLine(success ? "Succeeds" : "Fails");
                if (!success) allPassed = false;
                Console.WriteLine(string.Empty);
                Console.WriteLine("------------------------------");
                Console.WriteLine(string.Empty);
            }

            Assert.IsTrue(allPassed);
        }

        [Test]
        public void Can_Verify_All_Keys_Exist_Matching_In_EnXml_File()
        {
            var en = _langFileDirectory.GetFiles("en.xml").FirstOrDefault();
            Assert.NotNull(en);

            var others = _langFileDirectory.GetFiles().Where(x => !x.Name.Equals("en.xml"));

            var enDom = new XmlDocument();
            enDom.Load(en.FullName);

            var enAreas = enDom.DocumentElement.SelectNodes("area");
            foreach (var other in others)
            {
                var otherDom = new XmlDocument();
                otherDom.Load(other.FullName);

                Console.WriteLine("**********************************");
                Console.WriteLine("Comparing Areas to " + other.Name);
                Console.WriteLine("**********************************");
                Console.WriteLine(string.Empty);
                foreach (var a in enAreas)
                {
                    

                    // assert the area exists
                    if (
                        otherDom.DocumentElement.SelectNodes(
                            "area[@alias='" + ((XmlElement)a).GetAttribute("alias") + "']").Count == 0)
                    {
                        Console.WriteLine("Missing: " + ((XmlElement)a).GetAttribute("alias"));
                        this.AddAreaToFile(other, (XmlNode)a);
                    }
                    else
                    {
                        // verify that all keys match
                        var otherNode =
                            otherDom.DocumentElement.SelectNodes("area[@alias='" + ((XmlElement)a).GetAttribute("alias") + "']").Item(0);

                        Console.WriteLine("Verifying keys in " + ((XmlElement)a).GetAttribute("alias"));
                        foreach (var key in ((XmlNode)a).SelectNodes("key"))
                        {
                            var xpath = "area[@alias='" + ((XmlElement)a).GetAttribute("alias") + "']";
                            var match = otherNode.SelectSingleNode("key[@alias='" + ((XmlElement)key).GetAttribute("alias") + "']");
                            if (match == null)
                            {
                                if (!string.IsNullOrEmpty(((XmlElement)key).GetAttribute("alias"))) { 
                                Console.WriteLine("Missing Key: " +  ((XmlElement)key).GetAttribute("alias"));
                                this.AddKeyToArea(other, xpath, (XmlNode)key);
                                }
                            }
                        }
                    }
                    Console.WriteLine("---------------------");
                    Console.WriteLine(string.Empty);
                }
            }

        }

        private void AddKeyToArea(FileInfo f, string xpath, XmlNode keyNode)
        {
            var src = _langSourceDirectory.GetFiles(f.Name).FirstOrDefault();
            Assert.NotNull(src);

            var srcXml = new XmlDocument();
            srcXml.Load(src.FullName);

            var import = srcXml.ImportNode(keyNode, true);

            srcXml.DocumentElement.SelectNodes(xpath).Item(0).AppendChild(import);
            srcXml.Save(src.FullName);
        }


        private void AddAreaToFile(FileInfo f, XmlNode areaNode)
        {
            var src = _langSourceDirectory.GetFiles(f.Name).FirstOrDefault();
            Assert.NotNull(src);

            var srcXml = new XmlDocument();
            srcXml.Load(src.FullName);

            var import = srcXml.ImportNode(areaNode, true);

            srcXml.DocumentElement.AppendChild(import);

            srcXml.Save(src.FullName);
        }

        private bool TestUniqueKeys(FileInfo langFile)
        {
            var dom = new XmlDocument();
            dom.Load(langFile.FullName);
            var succeeds = true;
            var areas = dom.DocumentElement.SelectNodes("area");

            var areaList = new ArrayList();
            var badAreas = new ArrayList();
            
            foreach (var a in areas)
            {
                if (!areaList.Contains(((XmlElement)a).GetAttribute("alias")))
                {
                    areaList.Add(((XmlElement)a).GetAttribute("alias"));
                }
                else
                {
                    badAreas.Add(((XmlElement)a).GetAttribute("alias"));
                }

                var keysList = new ArrayList();
                var badKeys = new ArrayList();
                var keys = ((XmlElement)a).SelectNodes("key");
                foreach (var key in keys)
                {
                    if (!keysList.Contains(((XmlElement)key).GetAttribute("alias")))
                    {
                        keysList.Add(((XmlElement)key).GetAttribute("alias"));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(((XmlElement)key).GetAttribute("alias")))
                        badKeys.Add(((XmlElement)key).GetAttribute("alias"));
                    }
                }
                if (badKeys.Count > 0)
                {
                    succeeds = false;
                    Console.WriteLine("Areas (" + ((XmlElement)a).GetAttribute("alias") + ") Keys (Alias)");
                    foreach (var alias in badKeys) Console.WriteLine(alias);
                }
            }

            if (badAreas.Count > 0)
            {
                succeeds = false;
                Console.WriteLine("Areas Duplicated (Alias)");
                foreach(var alias in badAreas) Console.WriteLine(alias);
            }

            return succeeds;
        }
    }
}