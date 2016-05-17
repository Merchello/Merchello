namespace Merchello.Web.Models.Ui.Rendering
{
    using System;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// The product collection.
    /// </summary>
    public class ProductCollection
    {
        /// <summary>
        /// The _entity collection.
        /// </summary>
        private readonly IEntityCollection _entityCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCollection"/> class.
        /// </summary>
        /// <param name="entityCollection">
        /// The <see cref="IEntityCollection"/>.
        /// </param>
        /// <exception cref="Exception">
        /// Throws an exception if the <see cref="IEntityCollection"/> is not a product collection
        /// </exception>
        internal ProductCollection(IEntityCollection entityCollection)
        {
            Mandate.ParameterNotNull(entityCollection, "entityCollection");
            if (entityCollection.EntityTfKey != Core.Constants.TypeFieldKeys.Entity.ProductKey)
            {
                throw new Exception("Must be a product collection");
            }

            this._entityCollection = entityCollection;
        }

        /// <summary>
        /// Gets the parent key.
        /// </summary>
        public Guid? ParentKey
        {
            get
            {
                return this._entityCollection.ParentKey;
            }
        }

        /// <summary>
        /// Gets the collection key.
        /// </summary>
        public Guid CollectionKey
        {
            get
            {
                return this._entityCollection.Key;
            }
        }

        /// <summary>
        /// Gets the collection name.
        /// </summary>
        public string Name
        {
            get
            {
                return this._entityCollection.Name;
            }
        }
    }
}