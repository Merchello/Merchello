namespace Merchello.Core.Models.Counting
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Intended to get the use count of an entity.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class EntityUseCount
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [DataMember]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        [DataMember]
        public long UseCount { get; set; } 
    }
}