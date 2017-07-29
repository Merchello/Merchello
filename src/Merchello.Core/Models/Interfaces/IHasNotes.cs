namespace Merchello.Core.Models
{
    using System.Collections.Generic;
    

    /// <summary>
    /// Marker interface for classes that include a notes field.
    /// </summary>
    public interface IHasNotes
    {
        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        
        IEnumerable<INote> Notes { get; set; }  
    }
}