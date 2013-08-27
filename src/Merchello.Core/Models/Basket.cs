using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public class Basket : IdEntity, IBasket
    {

        public Basket(BasketType basketType)
        {
            _basketTypeFieldKey = TypeFieldProvider.Basket().GetTypeField(basketType).TypeKey;
        }

        internal Basket(Guid basketTypeFieldKey)
        {
            _basketTypeFieldKey = basketTypeFieldKey;
        }

        private Guid _consumerKey;
        private Guid _basketTypeFieldKey;

        private static readonly PropertyInfo ConsumerKeySelector = ExpressionHelper.GetPropertyInfo<Basket, Guid>(x => x.ConsumerKey);
        private static readonly PropertyInfo BasketTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<Basket, Guid>(x => x.BasketTypeFieldKey);

        /// <summary>
        /// The key of the customer or anonymous customer associated with the Basket
        /// </summary>
        [DataMember]
        public Guid ConsumerKey
        {
            get { return _consumerKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _consumerKey = value;
                    return _consumerKey;
                }, _consumerKey, ConsumerKeySelector);
            }
        }

        /// <summary>
        /// The Basket Type (<see cref="ITypeField"/>) associated with the Basket
        /// </summary>
        [DataMember]
        public Guid BasketTypeFieldKey
        {
            get { return _basketTypeFieldKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _basketTypeFieldKey = value;
                    return _basketTypeFieldKey;
                }, _basketTypeFieldKey, BasketTypeFieldKeySelector);
            }
        }

        /// <summary>
        /// Gets/sets the BasketType
        /// </summary>
        /// <remarks>
        /// This property only allows internally defined BasketTypes to be set.  eg. no Custom types.  These will have
        /// to be set through the BasketTypeFieldKey property directly.
        /// </remarks>
        [DataMember]
        public BasketType BasketType
        {
            get { return TypeFieldProvider.Basket().GetTypeField(_basketTypeFieldKey); }
            set
            {
                var reference = TypeFieldProvider.Basket().GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    BasketTypeFieldKey = reference.TypeKey;
                }
            }
        }

    }

}