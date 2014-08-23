namespace Merchello.Web.Models.SaleHistory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.Models.Interfaces;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The audit log display.
    /// </summary>
    public class AuditLogDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        public Guid? EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        public Guid? EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the verbosity.
        /// </summary>
        public int Verbosity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is error.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Gets or sets the record date.
        /// </summary>
        public DateTime RecordDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }
    }

    /// <summary>
    /// The audit log display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class AuditLogDisplayExtensions
    {
        /// <summary>
        /// The to audit log display.
        /// </summary>
        /// <param name="auditLog">
        /// The audit log.
        /// </param>
        /// <returns>
        /// The <see cref="AuditLogDisplay"/>.
        /// </returns>
        public static AuditLogDisplay ToAuditLogDisplay(this IAuditLog auditLog)
        {
            return AutoMapper.Mapper.Map<AuditLogDisplay>(auditLog);
        }
    }
}
