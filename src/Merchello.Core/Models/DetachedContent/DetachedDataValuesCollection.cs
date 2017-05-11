﻿namespace Merchello.Core.Models.DetachedContent
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    using Umbraco.Core;

    /// <summary>
    /// The detached data values collection.
    /// </summary>
    public class DetachedDataValuesCollection : ConcurrentDictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedDataValuesCollection"/> class.
        /// </summary>
        public DetachedDataValuesCollection()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedDataValuesCollection"/> class.
        /// </summary>
        /// <param name="keyValues">
        /// The key values.
        /// </param>
        public DetachedDataValuesCollection(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            this.Load(keyValues);
        }

        /// <summary>
        /// The collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Sets a detached data value.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetValue(string key, string value)
        {
            AddOrUpdate(key, value, (x, y) => value);
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
            string obj;
            TryRemove(key, out obj);
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
            return ContainsKey(key) ? this[key] : string.Empty;
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
        /// The load.
        /// </summary>
        /// <param name="keyValues">
        /// The key values.
        /// </param>
        private void Load(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            // TODO the keyvalue pairs should never be empty but we need a hack fix to account for 
            // a WebAPi deserialization when creating a new detached content item.
            foreach (var pair in keyValues.Where(x => !x.Key.IsNullOrWhiteSpace() && !x.Value.IsNullOrWhiteSpace()))
            {
                this.SetValue(pair.Key, pair.Value);
            }
        }
    }
}