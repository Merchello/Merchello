namespace Merchello.Core.Models.TypeFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration;

    /// <inheritdoc/>
    public abstract class ExtendedTypeFieldMapper<T> : TypeFieldMapper<T>, IExtendedTypeFieldMapper<T>
    {
        /// <summary>
        /// Associates various enum types with the <see cref="CustomTypeFieldType"/>.
        /// </summary>
        private static readonly IDictionary<Type, CustomTypeFieldType> _associatedCFT = new Dictionary<Type, CustomTypeFieldType>
            {
                { typeof(AddressType), CustomTypeFieldType.Address },
                { typeof(ItemCacheType), CustomTypeFieldType.ItemCache },
                { typeof(LineItemType), CustomTypeFieldType.LineItem },
                { typeof(PaymentMethodType), CustomTypeFieldType.PaymentMethod },
                { typeof(ProductType), CustomTypeFieldType.Product }
            };

        /// <summary>
        /// Lazy for the configuration look up.
        /// </summary>
        private readonly Lazy<IEnumerable<ITypeField>> _fields = new Lazy<IEnumerable<ITypeField>>(GetCustomFields);


        /// <inheritdoc/>
        public IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return _fields.Value;
            }
        }

        /// <inheritdoc/>
        public override T GetTypeField(Guid key)
        {
            return this.CustomTypeFields.Any(x => x.TypeKey == key) ?
                CachedTypeFields.Keys.FirstOrDefault(x => CachedTypeFields[x].TypeKey == Guid.Empty) :
                CachedTypeFields.Keys.FirstOrDefault(x => CachedTypeFields[x].TypeKey == key);
        }

        /// <inheritdoc/>
        public ITypeField Custom(string alias)
        {
            return _fields.Value.FirstOrDefault(x => x.Alias == alias) ?? NotFound;
        }

        /// <summary>
        /// Gets the collection of custom type fields from the configuration file
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ITypeField}"/>.
        /// </returns>
        private static IEnumerable<ITypeField> GetCustomFields()
        {
            
            if (_associatedCFT.ContainsKey(typeof(T)))
            {
                var cft = _associatedCFT[typeof(T)];
                return MerchelloConfig.For.MerchelloExtensibility().TypeFields.CustomTypeFields[cft];
            }

            var nullRef = new NullReferenceException("The associated type of enum was not found or is not supported as a custom type field");
            // REFACTOR-todo Log this
            throw nullRef;
        }
    }
}