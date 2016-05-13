namespace Merchello.FastTrack.Models
{
    using System.ComponentModel.DataAnnotations;

    using Merchello.Core.Localization;

    using Umbraco.Core;

    /// <summary>
    /// The checkout billing address model.
    /// </summary>
    public class FastTrackBillingAddressModel : FastTrackCheckoutAddressModel, IFastTrackCheckoutAddressModel
    {
        /// <summary>
        /// The split names.
        /// </summary>
        private string[] _names;

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
        /// Gets or sets the email address.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelEmailAddress")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredEmailAddress")]
        [EmailAddress(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "InvalidEmailAddress")]
        public new string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use billing address for the shipping address.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelUseForShipping")]
        public bool UseForShipping { get; set; }

        public override string Name
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }

            set
            {
                // stored but never used
                if (_names == null && !value.IsNullOrWhiteSpace())
                {
                    _names = value.Split(' ');
                    if (_names.Length == 2)
                    {
                        FirstName = _names[0];
                        LastName = _names[1];
                    } 
                }
            }
        }

        /// <summary>
        /// Gets or sets the success URL to redirect to the  ship rate quote stage.
        /// </summary>
        /// <remarks>
        /// Used if customer opts to use the billing address for the shipping address 
        /// </remarks>
        public string SuccessUrlShipRateQuote { get; set; }
    }
}