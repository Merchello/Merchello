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
        /// The name.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The two letter ISO code.
        /// </summary>
        private readonly string _code;

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
            _name = name;
            _code = code;            
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Code
        {
            get { return _code; }
        }
    }
}