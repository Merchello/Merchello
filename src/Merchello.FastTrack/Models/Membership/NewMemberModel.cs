namespace Merchello.FastTrack.Models.Membership
{
    using System.ComponentModel.DataAnnotations;

    using Merchello.Core.Localization;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A model for creating a new Umbraco Membership account that has all properties to populate a Merchello Customer.
    /// </summary>
    public class NewMemberModel : IMerchelloCustomerMembershipProfile
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredEmailAddress")]
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelEmailAddress")]
        [EmailAddress(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "InvalidEmailAddress")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelFirstName")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredFirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelLastName")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredLastName")]
        public string LastName { get; set; }

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

        /// <summary>
        /// Gets or sets the member type name.
        /// </summary>
        /// <remarks>
        /// Not displayed but required for membership creation
        /// </remarks>
        [Required]
        public string MemberTypeName { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to persist login after registration.
        /// </summary>
        public bool PersistLogin { get; set; }
    }
}