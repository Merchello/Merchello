namespace Merchello.Core.Services.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Marker interface product service queries that need to be ported forward to V3 version.
    /// </summary>
    public interface IProductServicePortForward
    {
        /// <summary>
        /// Gets a list of currently listed Manufacturers.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> (manufacturer names).
        /// </returns>
        IEnumerable<string> GetAllManufacturers();
    }
}