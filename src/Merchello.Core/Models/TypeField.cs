using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Wrapper for the TypeFieldElement configuration class.
    /// </summary>
    public class TypeField : ITypeField
    {
        private readonly string _alias;
        private readonly string _name;
        private readonly Guid _typeKey;

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

        public string Alias { get { return _alias;  } }
        public string Name  { get { return _name; } }
        public Guid TypeKey { get { return _typeKey; } }
    }
}