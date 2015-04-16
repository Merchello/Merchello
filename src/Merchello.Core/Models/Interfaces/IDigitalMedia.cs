namespace Merchello.Core.Models.Interfaces
{
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    ///// TODO SR - This is just a scaffold for example.  Do whatever you need to do =)

    /// <summary>
    /// Defines DigitalMedia.
    /// </summary>
    public interface IDigitalMedia : IEntity
    {
        // TODO define digital media fields

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        string Name { get; set; }
    }
}