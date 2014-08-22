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
        #region Fields

        /// <summary>
        /// The extended data changed selector.
        /// </summary>
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<AuditLog, ExtendedDataCollection>(x => x.ExtendedData);

        /// <summary>
        /// The entity key selector.
        /// </summary>
        private static readonly PropertyInfo EntityKeySelector = ExpressionHelper.GetPropertyInfo<AuditLog, Guid?>(x => x.EntityKey);

        /// <summary>
        /// The message selector.
        /// </summary>
        private static readonly PropertyInfo MessageSelector = ExpressionHelper.GetPropertyInfo<AuditLog, string>(x => x.Message);

        /// <summary>
        /// The reference type selector.
        /// </summary>
        private static readonly PropertyInfo EntityTfKeySelector = ExpressionHelper.GetPropertyInfo<AuditLog, Guid?>(x => x.EntityTfKey);

        /// <summary>
        /// The verbosity selector.
        /// </summary>
        private static readonly PropertyInfo VerbositySelector = ExpressionHelper.GetPropertyInfo<AuditLog, int>(x => x.Verbosity);

        /// <summary>
        /// The is error selector.
        /// </summary>
        private static readonly PropertyInfo IsErrorSelector = ExpressionHelper.GetPropertyInfo<AuditLog, bool>(x => x.IsError);

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

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        [DataMember]
        public Guid? EntityKey 
        { 
            get
            {
                return _entityKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _entityKey = value;
                        return _entityKey;
                    }, 
                    _entityKey, 
                    EntityKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        [DataMember]
        public Guid? EntityTfKey
        {
            get
            {
                return _entityTfKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _entityTfKey = value;
                        return _entityTfKey;
                    },
                    _entityTfKey,
                    EntityTfKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember]
        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _message = value;
                        return _message;
                    },
                    _message,
                    MessageSelector);
            }
        }

        /// <summary>
        /// Gets or sets the verbosity.
        /// </summary>
        /// <remarks>
        /// Currently not used
        /// </remarks>
        [DataMember]
        public int Verbosity
        {
            get
            {
                return _verbosity;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _verbosity = value;
                        return _verbosity;
                    },
                    _verbosity,
                    VerbositySelector);
            }
        }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
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
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _isError = value;
                        return _isError;
                    },
                    _isError,
                    IsErrorSelector);
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
            OnPropertyChanged(ExtendedDataChangedSelector);
        }
    }
}