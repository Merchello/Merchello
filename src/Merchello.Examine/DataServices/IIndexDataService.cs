namespace Merchello.Examine.DataServices
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an IndexDataService interface.
    /// </summary>
    public interface IIndexDataService
    {
        /// <summary>
        /// Returns a list of all property names in the Merchello set being indexed
        /// </summary>
        /// <returns>
        /// The collection of all index field names.
        /// </returns>
        IEnumerable<string> GetIndexFieldNames();
    }
}