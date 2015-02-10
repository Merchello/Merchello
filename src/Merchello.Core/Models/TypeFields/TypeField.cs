namespace Merchello.Core.Models.TypeFields
{
    using System;

    using Merchello.Core.Configuration.Outline;

    using Newtonsoft.Json;

    /// <summary>
    /// Wrapper for the TypeFieldElement configuration class.
    /// </summary>
    public class TypeField : ITypeField
    {
        /// <summary>
        /// The alias of the type file.
        /// </summary>
        private readonly string _alias;

        /// <summary>
        /// The name of the type field.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The GUID key representing the type - generally "TfKey".
        /// </summary>
        private readonly Guid _typeKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeField"/> class.
        /// </summary>
       

        [JsonConstructor]
        public TypeField(string alias, string name, Guid typeKey)
        {
            _alias = alias;
            _name = name;
            _typeKey = typeKey;
        }

        public TypeField(TypeFieldElement config)
        {
            _alias = config.Alias;
            _name = config.Name;
            _typeKey = config.TypeKey;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias
        {
            get
            {
                return _alias;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the type key.
        /// </summary>
        public Guid TypeKey
        {
            get
            {
                return _typeKey;
            }
        }
    }
}