namespace Merchello.Core.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a model with notes.
    /// </summary>
    public interface IHasNotes
    {
        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        [DataMember]
        IEnumerable<INote> Notes { get; set; }  
    }
}