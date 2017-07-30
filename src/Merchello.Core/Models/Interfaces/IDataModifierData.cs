namespace Merchello.Core.Models
{
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents modifiable data.
    /// </summary>
    public interface IDataModifierData
    {
        /// <summary>
        /// Gets or sets the modified data logs.
        /// </summary>
        IEnumerable<IDataModifierLog> ModifiedDataLogs { get; set; }
    }
}