using System;
using System.Configuration;
using System.IO;
using log4net.Config;
using Umbraco.Core;
using Umbraco.Core.IO;

namespace Merchello.Tests.Base.Database
{
	/// <summary>
	/// Common helper properties and methods useful to testing
	/// </summary>
	public static class TestHelper
	{

		/// <summary>
		/// Gets the current assembly directory.
		/// </summary>
		/// <value>The assembly directory.</value>
		static public string CurrentAssemblyDirectory
		{
			get
			{
				var codeBase = typeof(TestHelper).Assembly.CodeBase;
				var uri = new Uri(codeBase);
				var path = uri.LocalPath;
				return Path.GetDirectoryName(path);
			}
		}

		/// <summary>
		/// Maps the given <paramref name="relativePath"/> making it rooted on <see cref="CurrentAssemblyDirectory"/>. <paramref name="relativePath"/> must start with <code>~/</code>
		/// </summary>
		/// <param name="relativePath">The relative path.</param>
		/// <returns></returns>
		public static string MapPathForTest(string relativePath)
		{
			if (!relativePath.StartsWith("~/"))
				throw new ArgumentException("relativePath must start with '~/'", "relativePath");

			return relativePath.Replace("~/", CurrentAssemblyDirectory + "/");
		}

		public static void SetupLog4NetForTests()
		{
		    var file = MapPathForTest("~/unit-test-log4net.config");
			XmlConfigurator.Configure(new FileInfo(file));
		}

      
	    public static void CreateDirectories(string[] directories)
        {
            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(IOHelper.MapPath(directory));
                if (directoryInfo.Exists == false)
                    Directory.CreateDirectory(IOHelper.MapPath(directory));
            }
        }

	    public static void CleanDirectories(string[] directories)
        {
            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(IOHelper.MapPath(directory));
                if (directoryInfo.Exists)
                    directoryInfo.GetFiles().ForEach(x => x.Delete());
            }
        }
	}
}