namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

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

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class.
        /// </summary>
        public Note()
        {
            Message = null;
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            EntityKey = null;
            EntityTfKey = null;
        }

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
    }
}