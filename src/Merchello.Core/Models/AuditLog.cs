namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The audit log.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class AuditLog : Entity, IAuditLog
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// The entity key.
        /// </summary>
        private Guid? _entityKey;

        /// <summary>
        /// The reference type.
        /// </summary>
        private Guid? _entityTfKey;

        /// <summary>
        /// The message.
        /// </summary>
        private string _message;

        /// <summary>
        /// The verbosity.
        /// </summary>
        private int _verbosity;

        /// <summary>
        /// The _extended data.
        /// </summary>
        private ExtendedDataCollection _extendedData;

        /// <summary>
        /// The is error.
        /// </summary>
        private bool _isError;

        #endregion

        /// <inheritdoc/>
        [DataMember]
        public Guid? EntityKey
        {
            get
            {
                return _entityKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _entityKey, _ps.Value.EntityKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? EntityTfKey
        {
            get
            {
                return _entityTfKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _entityTfKey, _ps.Value.EntityTfKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _message, _ps.Value.MessageSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int Verbosity
        {
            get
            {
                return _verbosity;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _verbosity, _ps.Value.VerbositySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get
            {
                return _extendedData;
            }

            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a record of an error.
        /// </summary>
        [DataMember]
        public bool IsError
        {
            get
            {
                return _isError;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _isError, _ps.Value.IsErrorSelector);
            }
        }

        /// <summary>
        /// The extended data changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.ExtendedDataChangedSelector);
        }

        /// <summary>
        /// Property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The extended data changed selector.
            /// </summary>
            public readonly PropertyInfo ExtendedDataChangedSelector =
                ExpressionHelper.GetPropertyInfo<AuditLog, ExtendedDataCollection>(x => x.ExtendedData);

            /// <summary>
            /// The entity key selector.
            /// </summary>
            public readonly PropertyInfo EntityKeySelector =
                ExpressionHelper.GetPropertyInfo<AuditLog, Guid?>(x => x.EntityKey);

            /// <summary>
            /// The message selector.
            /// </summary>
            public readonly PropertyInfo MessageSelector =
                ExpressionHelper.GetPropertyInfo<AuditLog, string>(x => x.Message);

            /// <summary>
            /// The reference type selector.
            /// </summary>
            public readonly PropertyInfo EntityTfKeySelector =
                ExpressionHelper.GetPropertyInfo<AuditLog, Guid?>(x => x.EntityTfKey);

            /// <summary>
            /// The verbosity selector.
            /// </summary>
            public readonly PropertyInfo VerbositySelector =
                ExpressionHelper.GetPropertyInfo<AuditLog, int>(x => x.Verbosity);

            /// <summary>
            /// The is error selector.
            /// </summary>
            public readonly PropertyInfo IsErrorSelector =
                ExpressionHelper.GetPropertyInfo<AuditLog, bool>(x => x.IsError);
        }
    }
}