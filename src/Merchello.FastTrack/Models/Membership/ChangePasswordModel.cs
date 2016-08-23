namespace Merchello.FastTrack.Models.Membership
{
	using System.ComponentModel.DataAnnotations;

	using Merchello.Web.Store.Localization;

	/// <summary>
	/// A model used for authentication.
	/// </summary>
	/// <remarks>
	/// Could probably just use Umbraco's LoginModel here 
	/// </remarks>
	public class ChangePasswordModel
	{
		/// <summary>
		/// Gets or sets the old password.
		/// </summary>
		[Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredOldPassword")]
		[Display(ResourceType = typeof(StoreFormsResource), Name = "LabelOldPassword"), DataType(DataType.Password)]
		public string OldPassword { get; set; }

		/// <summary>
		/// Gets or sets the new password.
		/// </summary>
		[Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredPassword")]
		[Display(ResourceType = typeof(StoreFormsResource), Name = "LabelPassword"), DataType(DataType.Password)]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the confirm password.
		/// </summary>
		[Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredConfirmPassword")]
		[Display(ResourceType = typeof(StoreFormsResource), Name = "LabelConfirmPassword"), DataType(DataType.Password)]
		[Compare("Password", ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "InvalidConfirmPassword")]
		public string ConfirmPassword { get; set; }
	}
}