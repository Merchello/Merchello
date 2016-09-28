namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The note.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Note : Entity, INote
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// The entity key.
        /// </summary>
        private Guid _entityKey;

        /// <summary>
        /// The reference type.
        /// </summary>
        private Guid _entityTfKey;

        /// <summary>
        /// The author.
        /// </summary>
        private string _author;

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
            Ensure.ParameterCondition(entityTfKey != Guid.Empty, "entityTfKey");
            Message = string.Empty;
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            EntityKey = entityKey;
            EntityTfKey = entityTfKey;
            InternalOnly = false;
            Author = string.Empty;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid EntityKey 
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
        public Guid EntityTfKey
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
        public string Author
        {
            get
            {
                return _author;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _author, _ps.Value.AuthorSelector);
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
        public bool InternalOnly
        {
            get
            {
                return _internalOnly;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _internalOnly, _ps.Value.InternalOnlySelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The entity key selector.
            /// </summary>
            public readonly PropertyInfo EntityKeySelector = ExpressionHelper.GetPropertyInfo<Note, Guid?>(x => x.EntityKey);

            /// <summary>
            /// The author selector.
            /// </summary>
            public readonly PropertyInfo AuthorSelector = ExpressionHelper.GetPropertyInfo<Note, string>(x => x.Author);

            /// <summary>
            /// The message selector.
            /// </summary>
            public readonly PropertyInfo MessageSelector = ExpressionHelper.GetPropertyInfo<Note, string>(x => x.Message);

            /// <summary>
            /// The reference type selector.
            /// </summary>
            public readonly PropertyInfo EntityTfKeySelector = ExpressionHelper.GetPropertyInfo<Note, Guid?>(x => x.EntityTfKey);

            /// <summary>
            /// The internal only selector.
            /// </summary>
            public readonly PropertyInfo InternalOnlySelector = ExpressionHelper.GetPropertyInfo<Note, bool>(x => x.InternalOnly);
        }
    }
}