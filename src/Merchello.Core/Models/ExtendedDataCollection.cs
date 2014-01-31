using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Merchello.Core.Models
{    
    /// <summary>
    /// Represents an ExtendedDataCollection
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    public class ExtendedDataCollection : ConcurrentDictionary<string, string>, INotifyCollectionChanged
    {

        public ExtendedDataCollection()
        { }

        internal ExtendedDataCollection(string persistedXml)
        {
            var doc = XDocument.Parse(persistedXml);
            var exData  = doc.Element("extendedData");
            if (exData == null) return;
            foreach (var el in exData.Elements())
            {
                SetValue(el.Name.LocalName, el.Value);                 
            }
        }

        internal string SerializeToXml()
        {
            string xml;
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("extendedData");

                    foreach (var key in Keys)
                    {
                        writer.WriteElementString(key, this[key]);
                    }

                    writer.WriteEndElement(); // ExtendedData
                    writer.WriteEndDocument();

                    xml = sw.ToString();
                }
            }

            return xml;
        }

        public void SetValue(string key, string value)
        {
            var validKey = AssertValidKey(key);
            AddOrUpdate(validKey, value, (x, y) => value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        }

        public void RemoveValue(string key)
        {
            var validKey = AssertValidKey(key);
            string obj;
            TryRemove(validKey, out obj);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj));
        }

        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public string GetValue(string key)
        {
            var validKey = AssertValidKey(key);
            return ContainsKey(validKey) ? this[validKey] : string.Empty;
        }

        private static string AssertValidKey(string key)
        {
            key = key.Replace(" ", string.Empty);
            return XmlConvert.EncodeLocalName(key);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }
        }
    }
}