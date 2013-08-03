using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// Base Abstract Entity
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("Key: {Key}")]
    public abstract class KeyEntity : Entity, IKeyEntity
    {
        /// <summary>
        /// Locks down the Id value
        /// </summary>
        [IgnoreDataMember]
        public override int Id {
            get { return default(int); }
            set {  } }


        public override bool SameIdentityAs(IEntity other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() == ((Entity)other).GetRealType() && HasIdentity && other.HasIdentity)
                return other.Key.Equals(Key);

            return false;
        }
    }
}
