namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Represents an ExtendedDataCollection
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    public class ExtendedDataCollection : ConcurrentDictionary<string, string>, INotifyCollectionChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedDataCollection"/> class.
        /// </summary>
        public ExtendedDataCollection()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedDataCollection"/> class.
        /// </summary>
        /// <param name="persistedXml">
        /// The persisted xml.
        /// </param>
        internal ExtendedDataCollection(string persistedXml)
        {
            var doc = XDocument.Parse(persistedXml);

            var extendedData  = doc.Element("extendedData") ?? doc.Descendants("extendedData").FirstOrDefault();

            if (extendedData == null) return;
 
            foreach (var el in extendedData.Elements())
            {
                SetValue(el.Name.LocalName, el.Value);                 
            }
        }

        /// <summary>
        /// The collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Sets an extended data value.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetValue(string key, string value)
        {
            var validKey = AssertValidKey(key);
            AddOrUpdate(validKey, value, (x, y) => value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        }

        /// <summary>
        /// Removes a value from extended data.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public void RemoveValue(string key)
        {
            var validKey = AssertValidKey(key);
            string obj;
            TryRemove(validKey, out obj);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj));
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Gets a value from the collection.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetValue(string key)
        {
            var validKey = AssertValidKey(key);
            return ContainsKey(validKey) ? this[validKey] : string.Empty;
        }

        /// <summary>
        /// Serializes the collection to an Xml representation.
        /// </summary>
        /// <returns>
        /// The Xml representation.
        /// </returns>
        internal string SerializeToXml()
        {
            string xml;
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = false,
                    ConformanceLevel = ConformanceLevel.Fragment
                };

                using (var writer = XmlWriter.Create(sw, settings))
                {
                    writer.WriteStartElement("extendedData");

                    foreach (var key in Keys)
                    {
                        writer.WriteElementString(key, this[key]);
                    }

                    writer.WriteEndElement(); // ExtendedData
                }

                xml = sw.ToString();
            }

            return xml;
        }

        /// <summary>
        /// The on collection changed.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }
        }

        /// <summary>
        /// Ensures that a key is value.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string AssertValidKey(string key)
        {
            key = key.Replace(" ", string.Empty);
            return XmlConvert.EncodeLocalName(key);
        }
    }
}