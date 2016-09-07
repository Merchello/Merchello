namespace Merchello.Core.Models.TypeFields
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    /// <inheritdoc/>
    public abstract class TypeFieldMapper<T> : TypeFieldMapperBase, ITypeFieldMapper<T>
    {
        /// <summary>
        /// The cached type fields.
        /// </summary>
        protected static readonly ConcurrentDictionary<T, ITypeField> CachedTypeFields = new ConcurrentDictionary<T, ITypeField>();


        /// <summary>
        /// Returns the respective enum value for a given <see cref="TypeField"/> TypeKey
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The type filed.
        /// </returns>
        public virtual T GetTypeField(Guid key)
        {
            return CachedTypeFields.Keys.FirstOrDefault(x => CachedTypeFields[x].TypeKey == key);
        }

        /// <summary>
        /// Returns a type field from the internal cache
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="ITypeField"/>.
        /// </returns>
        public ITypeField GetTypeField(T value)
        {
            ITypeField typeField;
            return CachedTypeFields.TryGetValue(value, out typeField) ?
                typeField : NotFound;
        }

        /// <summary>
        /// Builds the TypeField cache for the respective type
        /// </summary>
        internal abstract void BuildCache();

        /// <summary>
        /// Adds a type field to the internal cache dictionary
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="typeField">
        /// The type Field.
        /// </param>
        protected void AddUpdateCache(T key, ITypeField typeField)
        {
            CachedTypeFields.AddOrUpdate(key, typeField, (x, y) => typeField);
        }
    }
}
