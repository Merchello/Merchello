namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;

    /// <summary>
    /// The registration model.
    /// </summary>
    public partial class RegistrationModel : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public RegistrationModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the registration.
        /// </summary>
        public CombinedRegisterLoginModel RegistrationLogin { get; set; }
    }
}