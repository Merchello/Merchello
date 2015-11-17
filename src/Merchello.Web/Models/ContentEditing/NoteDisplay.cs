namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The note.
    /// </summary>
    [Serializable]
    public class NoteDisplay
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
        /// Gets or sets the record date.
        /// </summary>
        public DateTime RecordDate { get; set; }


    }


    /// <summary>
    /// The audit log display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class NoteDisplayExtensions
    {
        /// <summary>
        /// The to audit log display.
        /// </summary>
        /// <param name="note">
        /// The audit log.
        /// </param>
        /// <returns>
        /// The <see cref="NoteDisplay"/>.
        /// </returns>
        public static NoteDisplay ToNoteDisplay(this INote note)
        {
            return new NoteDisplay()
            {
                Message = note.Message,
                Key = note.Key,
                EntityKey = note.EntityKey,
                EntityTfKey = note.EntityTfKey,
                EntityType = EnumTypeFieldConverter.EntityType.GetTypeField(note.EntityTfKey ?? new Guid()),
                RecordDate = note.CreateDate
            };
        }
    }
}