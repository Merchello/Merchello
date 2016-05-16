namespace Merchello.Web.Models.Ui
{
    using System;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    public class ProductCollection
    {
        private IEntityCollection _entityCollection;

        internal ProductCollection(IEntityCollection entityCollection)
        {
            Mandate.ParameterNotNull(entityCollection, "entityCollection");
            if (entityCollection.EntityTfKey != Core.Constants.TypeFieldKeys.Entity.ProductKey)
            {
                throw new Exception("Must be a product collection");
            }

            this._entityCollection = entityCollection;
        }

        public Guid? ParentKey
        {
            get
            {
                return this._entityCollection.ParentKey;
            }
        }

        public Guid CollectionKey
        {
            get
            {
                return this._entityCollection.Key;
            }
        }

        public string Name
        {
            get
            {
                return this._entityCollection.Name;
            }
        }
    }
}