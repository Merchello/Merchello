using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    public interface IProvince
    {
        /// <summary>
        /// The name of the province
        /// </summary>
        [DataMember]
        string Name { get; }

        /// <summary>
        /// The two letter province code
        /// </summary>
        [DataMember]
        string Code { get; }

    }
}