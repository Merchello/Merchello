using System;
using System.Linq;
using System.Xml;
using Merchello.Core.Configuration;
using Merchello.Core.Models.Rdbms;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.DataLayer;
using umbraco.interfaces;
using Umbraco.Core.Logging;

namespace Merchello.Web.PackageActions
{
	/// <summary>
	/// This package action will grant a user access to an Umbraco application.
	/// </summary>
	/// <remarks>
	/// This package action has been customized from the PackageActionsContrib Project.
	/// http://packageactioncontrib.codeplex.com
	/// </remarks>
	public class GrantPermissionForApp : IPackageAction
	{
		private readonly Database _database;
		

		public GrantPermissionForApp()
		{
			_database = ApplicationContext.Current.DatabaseContext.Database;
		}


		private UserDto GetUserDto(XmlNode xmlData)
		{
			var appName = this.GetAttributeValue(xmlData, "appName");
			var userLogin = this.GetAttributeValue(xmlData, "userLogin");

			if (string.Equals(userLogin, "$currentUser", StringComparison.OrdinalIgnoreCase))
				userLogin = UmbracoContext.Current.UmbracoUser.LoginName;

			var sql = new Sql();
			sql.Select("*")
				.From<UserDto>()
				.Where<UserDto>(x => x.Login == userLogin);

			return _database.Fetch<UserDto>(sql).FirstOrDefault();
		}

		public string Alias()
		{
			return string.Concat(MerchelloConfiguration.ApplicationName, "_GrantPermissionForApp");
		}

		public bool Execute(string packageName, XmlNode xmlData)
		{
			// execute revoke first to clear any existing permissions app/user relationships

			var user = GetUserDto(xmlData);

			Revoke(packageName, xmlData, user);
			return Grant(packageName, xmlData, user);
		}

		public bool Undo(string packageName, XmlNode xmlData)
		{
			var user = GetUserDto(xmlData);
			return Revoke(packageName, xmlData, user);
		}

		public XmlNode SampleXml()
		{
			var sample = string.Concat("<Action runat=\"install\" undo=\"false\" alias=\"", Alias(), "\" userLogin=\"$currentUser\" appName=\"", MerchelloConfiguration.ApplicationName.ToLowerInvariant(), "\"/>");
			return helper.parseStringToXmlNode(sample);
		}

		private bool Grant(string packageName, XmlNode xmlData, UserDto user)
		{
			var dto = new User2AppDto()
			{
				UserId = user.Id,
				AppAlias = packageName.ToLowerInvariant()
			};

			LogHelper.Info<GrantPermissionForApp>("Granting permission to " + packageName.ToLowerInvariant() + " for user Id " + user.Id);

			_database.Insert(dto);

			return true;
		}

		private bool Revoke(string packageName, XmlNode xmlData, UserDto user)
		{
			
			try
			{
                _database.Execute("DELETE FROM umbracoUser2app WHERE app = @package", new { package = packageName.ToLowerInvariant()});

				return true;
			}
			catch (SqlHelperException ex)
			{
				LogHelper.Error<GrantPermissionForApp>(string.Format("Error in Grant User Permission for App action for package {0}.", packageName), ex);
			}

			return false;		    
		}


		private string GetAttributeValue(XmlNode node, string attributeName)
		{
			if (node.Attributes[attributeName] != null)
			{
				var result = node.Attributes[attributeName].InnerText;
				if (!string.IsNullOrEmpty(result))
					return result;
			}

			return string.Empty;
		}
	}
}