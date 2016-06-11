namespace Merchello.Web.Models.Ui
{
    /// <summary>
    /// Defines an Umbraco Membership Profile.
    /// </summary>
    public interface ICustomerProfile : IUiModel
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <remarks>
        /// This is the login / username
        /// </remarks>
        string Email { get; set; }

        /// <summary>
        /// Gets or sets the Umbraco member first name.
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Umbraco member last name.
        /// </summary>
        string LastName { get; set; }
    }
}