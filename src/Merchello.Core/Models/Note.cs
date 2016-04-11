namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core;

    /// <summary>
    /// The note.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Note : Entity, INote
    {
        #region Fields

        /// <summary>
        /// The entity key selector.
        /// </summary>
        private static readonly PropertyInfo EntityKeySelector = ExpressionHelper.GetPropertyInfo<Note, Guid?>(x => x.EntityKey);

        /// <summary>
        /// The message selector.
        /// </summary>
        private static readonly PropertyInfo MessageSelector = ExpressionHelper.GetPropertyInfo<Note, string>(x => x.Message);

        /// <summary>
        /// The reference type selector.
        /// </summary>
        private static readonly PropertyInfo EntityTfKeySelector = ExpressionHelper.GetPropertyInfo<Note, Guid?>(x => x.EntityTfKey);

        /// <summary>
        /// The internal only selector.
        /// </summary>
        private static readonly PropertyInfo InternalOnlySelector = ExpressionHelper.GetPropertyInfo<Note, bool>(x => x.InternalOnly);

        /// <summary>
        /// The entity key.
        /// </summary>
        private Guid _entityKey;

        /// <summary>
        /// The reference type.
        /// </summary>
        private Guid _entityTfKey;

        /// <summary>
        /// The message.
        /// </summary>
        private string _message;

        /// <summary>
        /// The internal only.
        /// </summary>
        private bool _internalOnly;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class.
        /// </summary>
        /// <param name="entityKey">
        /// The entity Key.
        /// </param>
        /// <param name="entityTfKey">
        /// The entity type field Key.
        /// </param>
        public Note(Guid entityKey, Guid entityTfKey)
        {
            Mandate.ParameterCondition(entityTfKey != Guid.Empty, "entityTfKey");
            Message = string.Empty;
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            EntityKey = entityKey;
            EntityTfKey = entityTfKey;
            InternalOnly = false;
        }

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        [DataMember]
        public Guid EntityKey 
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
        public Guid EntityTfKey
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
        /// Gets or sets a value indicating whether the note should be used for internal use only.
        /// </summary>
        public bool InternalOnly
        {
            get
            {
                return _internalOnly;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _internalOnly = value;
                        return _internalOnly;
                    },
                    _internalOnly,
                    InternalOnlySelector);
            }
        }
    }
}