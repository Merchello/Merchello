namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <inheritdoc/>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Province : IProvince
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Province"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public Province(string code, string name)
        {
            this.Name = name;
            this.Code = code;            
        }

        /// <summary>
        /// Gets the name of the province
        /// </summary>
        [DataMember]
        public string Name { get; }

        /// <summary>
        /// Gets the two letter province code
        /// </summary>
        [DataMember]
        public string Code { get; }
    }
}