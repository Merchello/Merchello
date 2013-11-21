using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Province : IProvince
    {
        private readonly string _name;
        private readonly string _code;

        public Province(string code, string name)
        {
            _name = name;
            _code = code;            
        }

        /// <summary>
        /// The name of the province
        /// </summary>
        [DataMember]
        public string Name {
            get { return _name; }
        }

        /// <summary>
        /// The two letter province code
        /// </summary>
        [DataMember]
        public string Code {
            get { return _code; }
        }

    }
}