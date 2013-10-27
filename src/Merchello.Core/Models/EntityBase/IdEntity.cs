using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Umbraco.Core;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// Base Abstract EntityEntity
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("Id: {Id}")]
    public abstract class IdEntity : Entity
    {        
        /// <summary>
        /// Locks down the Key value
        /// </summary>
        [IgnoreDataMember]
        public override Guid Key {
            get { return Id.ToGuid(); }
            set { }
        }

        public override bool SameIdentityAs(IEntity other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() == ((Entity)other).GetRealType() && HasIdentity && other.HasIdentity)
                return other.Id.Equals(Id);

            return false;
        }
    }
}
