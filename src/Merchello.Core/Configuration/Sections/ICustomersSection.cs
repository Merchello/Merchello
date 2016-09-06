namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a configuration section for configurations related to Merchello "customers". 
    /// </summary>
    public interface ICustomersSection : IMerchelloConfigurationSection
    {
        /// <summary>
        /// Gets the number of days to persist Anonymous customers.
        /// <para>
        /// This value is only relevant if a scheduled task is setup to call the 'RemoveAnonymousCustomers' WebAPI method
        /// in the 'Merchello.Web.WebApi.ScheduledTasksApiController'.
        /// </para>
        /// </summary>
        int AnonymousCustomersMaxDays { get; }

        /// <summary>
        /// Gets the MemberTypes to be used when automatically creating or associating Merchello's customers with Membership users.
        /// </summary>
        IEnumerable<string> MemberTypes { get; } 
    }
}