namespace Merchello.FastTrack.Models.Membership
{
    using System.ComponentModel.DataAnnotations;

    using Merchello.Core.Localization;

    /// <summary>
    /// A model used for authentication.
    /// </summary>
    public class LoginModel
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
    }
}