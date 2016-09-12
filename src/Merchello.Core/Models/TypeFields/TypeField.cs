namespace Merchello.Core.Models.TypeFields
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Wrapper for the TypeFieldElement configuration class.
    /// </summary>
    public class TypeField : ITypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeField"/> class.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="typeKey">
        /// The type key.
        /// </param>
        [JsonConstructor]
        public TypeField(string alias, string name, Guid typeKey)
        {
            this.Alias = alias;
            this.Name = name;
            this.TypeKey = typeKey;
        }

        /// <inheritdoc/>
        public string Alias { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Guid TypeKey { get; }
    }
}