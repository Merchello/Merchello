namespace Merchello.Core.Configuration.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.Sections;
    using Merchello.Core.Models.TypeFields;

    /// <inheritdoc/>
    internal class TypeFieldsElement : RawXmlConfigurationElement, ITypeFieldsSection
    {
        /*
            public enum CustomTypeFieldType
            {
                Address,
                ItemCache,
                LineItem,
                PaymentMethod,
                Product
            }
         */

        /// <inheritdoc/>
        public IDictionary<CustomTypeFieldType, IEnumerable<ITypeField>> CustomTypeFields
        {
            get
            {
                var dictionary = new Dictionary<CustomTypeFieldType, IEnumerable<ITypeField>>();

                if (RawXml == null) return dictionary;

                dictionary.Add(CustomTypeFieldType.Address, Build("address/type"));
                dictionary.Add(CustomTypeFieldType.ItemCache, Build("itemCache/type"));
                dictionary.Add(CustomTypeFieldType.LineItem, Build("lineItem/type"));
                dictionary.Add(CustomTypeFieldType.PaymentMethod, Build("paymentMethod/type"));
                dictionary.Add(CustomTypeFieldType.Product, Build("product/type"));

                return dictionary;
            }
        }

        /// <summary>
        /// Constructs a collection of <see cref="ITypeField"/> from configuration elements.
        /// </summary>
        /// <param name="xpath">
        /// The xpath to the list of configured type fields.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ITypeField}"/>.
        /// </returns>
        private IEnumerable<ITypeField> Build(string xpath)
        {
            return
                RawXml.Elements(xpath)
                    .Select(
                        x =>
                        new TypeField(
                            x.Attribute("alias").Value,
                            x.Attribute("name").Value,
                            new Guid(x.Attribute("key").Value)));
        }
    }
}