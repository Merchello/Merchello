// Decompiled with JetBrains decompiler
// Type: Merchello.FastTrack.Controllers.Membership.ResetPasswordController
// Assembly: Merchello.FastTrack, Version=2.7.6815.22450, Culture=neutral, PublicKeyToken=null
// MVID: F1497CF5-B8BA-418C-81F6-7D62CB0FE5C1
// Assembly location: Z:\Umbraco\hklving\www\bin\Merchello.FastTrack.dll

using Connect.Utils;
using ETC.B2B.WEB.Models;
using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;

namespace Merchello.FastTrack.Controllers.Membership
{
	[PluginController("FastTrack")]
	public class ResetPasswordController : SurfaceController
	{
		[ChildActionOnly]
		public ActionResult Render()
		{
			bool result;
			bool.TryParse(this.Request.QueryString["success"], out result);
			PasswordResetModel model = new PasswordResetModel()
			{
				Email = this.Request.QueryString["email"],
				Token = this.Request.QueryString["token"],
				Success = result
			};
			if (!result && ResetPasswordController.VerifyResetData(model) == null)
				model.Error = "Can't reset your password with the provided information";
			return (ActionResult)this.PartialView("ResetPassword", (object)model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPassword(PasswordResetModel model)
		{
			if (!this.ModelState.IsValid)
				return (ActionResult)this.CurrentUmbracoPage();
			IMember entity = ResetPasswordController.VerifyResetData(model);
			if (entity == null)
			{
				this.ModelState.AddModelError("", "Can't reset your password with the provided information");
				return (ActionResult)this.CurrentUmbracoPage();
			}
			NameValueCollection queryStringValues = new NameValueCollection();
			try
			{
				MembershipUser user = System.Web.Security.Membership.GetUser(model.Email);
				if (user != null)
				{
					IMemberService memberService = ApplicationContext.Current.Services.MemberService;
					entity.IsLockedOut = false;
					entity.FailedPasswordAttempts = 0;
					memberService.Save(entity, true);
					string oldPassword = user.ResetPassword();
					user.ChangePassword(oldPassword, model.NewPassword);
					try
					{
						entity.SetValue("passwordResetToken", (object)string.Empty);
						entity.SetValue("passwordResetTokenExpiryDate", (object)string.Empty);
					}
					catch (Exception ex)
					{
					}
					memberService.Save(entity, true);
					queryStringValues.Add("success", "true");
				}
				else
				{
					LogHelper.Warn<ResetPasswordController>(string.Format("ResetPassword - Can't find member in the MemberShip Provider {0}", (object)model.Email));
					this.ModelState.AddModelError("", "Can't reset your password with the provided information");
					return (ActionResult)this.CurrentUmbracoPage();
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error<ResetPasswordController>("Couldn't reset password", ex);
			}
			return (ActionResult)this.RedirectToCurrentUmbracoPage(queryStringValues);
		}

		private static IMember VerifyResetData(PasswordResetModel model)
		{
			try
			{
				IMember byEmail = ApplicationContext.Current.Services.MemberService.GetByEmail(model.Email);
				if (byEmail == null)
				{
					LogHelper.Warn<ResetPasswordController>(string.Format("VerifyResetData - Can't find member in the MemberService {0}", (object)model.Email));
					return (IMember)null;
				}
				string tokenExpiryDate = byEmail.GetValue<string>("passwordResetTokenExpiryDate");
				DateTime result;
				if (!DateTime.TryParse(tokenExpiryDate, out result))
				{
					LogHelper.Warn<ResetPasswordController>(string.Format("VerifyResetData - Could not parse date/time {0}", tokenExpiryDate));
					return (IMember)null;
				}
				if (result < DateTime.Now)
				{
					LogHelper.Warn<ResetPasswordController>(string.Format("VerifyResetData - Token expired at {0}, it is now {1}", result, DateTime.Now));
					return (IMember)null;
				}
				string resetToken = byEmail.GetValue<string>("passwordResetToken");

				bool isTokenValid = Encrypter.Encrypt(model.Token, Encrypter.EncryptEnum.SHA1) == resetToken;
				if (!isTokenValid)
					LogHelper.Warn<ResetPasswordController>(string.Format("VerifyResetData - VerifyToken failed, token value recieved: {0}, hashed token: {1}", model.Token, resetToken));

				return isTokenValid ? byEmail : null;
			}
			catch (Exception ex)
			{
				LogHelper.Error<ResetPasswordController>("Couldn't verify reset data", ex);
			}
			return (IMember)null;
		}
	}
}
