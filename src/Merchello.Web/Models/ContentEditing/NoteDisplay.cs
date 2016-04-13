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
    public class NoteDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        public Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether internal only.
        /// </summary>
        public bool InternalOnly { get; set; }

        /// <summary>
        /// Gets or sets the note type field.
        /// </summary>
        public TypeField NoteTypeField { get; set; }

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
            return AutoMapper.Mapper.Map<INote, NoteDisplay>(note);
        }

        /// <summary>
        /// Maps <see cref="NoteDisplay"/> to <see cref="INote"/>.
        /// </summary>
        /// <param name="noteDisplay">
        /// The note display.
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        public static INote ToNote(this NoteDisplay noteDisplay)
        {

            var note = new Note(noteDisplay.EntityKey, noteDisplay.EntityTfKey)
            {
                Author = noteDisplay.Author,
                Message = noteDisplay.Message,
                InternalOnly = noteDisplay.InternalOnly,
                CreateDate = noteDisplay.RecordDate == DateTime.MinValue ? DateTime.Now : noteDisplay.RecordDate
            };

            if (!noteDisplay.Key.Equals(Guid.Empty)) note.Key = noteDisplay.Key;

            return note;
        }
    }
}