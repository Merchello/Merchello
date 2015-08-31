namespace Merchello.Core.Models.DetachedContent
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web.Http.Validation.Providers;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines DetachedContent.
    /// </summary>
    public interface IDetachedContent : IEntity 
    {
        /// <summary>
        /// Gets the detached content type.
        /// </summary>
        [DataMember]
        IDetachedContentType DetachedContentType { get; }

        /// <summary>
        /// Gets the culture name.
        /// </summary>
        [DataMember]
        string CultureName { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        [DataMember]
        DetachedDataValuesCollection DetachedDataValues { get; }
    }
}