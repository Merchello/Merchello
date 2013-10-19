using System.Collections.Generic;

namespace Merchello.Examine.DataServices
{
    public interface IIndexDataService
    {
        /// <summary>
        /// Returns a list of all property names in the Merchello set being indexed
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetIndexFieldNames();
    }
}