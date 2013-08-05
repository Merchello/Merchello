using System;
using System.Runtime.Serialization;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// Marker interface for IKeyEntity
    /// </summary>
    public interface IKeyEntity : ITracksDirty, ISingularRoot
    {

    }
}
