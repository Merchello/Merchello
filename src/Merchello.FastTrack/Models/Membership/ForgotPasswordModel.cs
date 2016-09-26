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
	public class ForgotPasswordModel
	{
		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		[Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredUsername")]
		[Display(ResourceType = typeof(StoreFormsResource), Name = "LabelUsername")]
		public string Username { get; set; }

	}
}