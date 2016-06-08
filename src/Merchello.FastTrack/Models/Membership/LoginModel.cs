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
    public class LoginModel : ISuccessRedirectUrl
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredUsername")]
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelUsername")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredPassword")]
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelPassword"), DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember me.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelRememberMe")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the success redirect url.
        /// </summary>
        public string SuccessRedirectUrl { get; set; }
    }
}