namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Models.Interfaces;
    using Persistence.Querying;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// Defines a NoteService.
    /// </summary>
    public interface INoteService : IPageCachedService<INote>
    {
        /// <summary>
        /// Creates a note.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        INote CreateNote(Guid entityKey, EntityType entityType, string message, bool raiseEvents = true);

        /// <summary>
        /// Creates a note without saving it to the database.
        /// </summary>
        /// <param name="entityKey">
        /// The entity Key.
        /// </param>
        /// <param name="entityTfKey">
        /// The entity Type field Key.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        INote CreateNote(Guid entityKey, Guid entityTfKey, string message, bool raiseEvents = true);

        ///// <summary>
        ///// Creates a note and saves it to the database
        ///// </summary>
        ///// <param name="message">
        ///// The message.
        ///// </param>
        ///// <param name="raiseEvents">
        ///// Optional boolean indicating whether or not to raise events
        ///// </param>
        ///// <returns>
        ///// The <see cref="INote"/>.
        ///// </returns>
        //INote CreateNoteWithKey(string message, bool raiseEvents = true);


        /// <summary>
        /// Creates a note and saves it to the database
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        INote CreateNoteWithKey(Guid entityKey, EntityType entityType, string message, bool raiseEvents = true);

        /// <summary>
        /// Creates an note and saves it to the database
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        INote CreateNoteWithKey(Guid entityKey, Guid entityTfKey, string message, bool raiseEvents = true);

        /// <summary>
        /// Saves an <see cref="INote"/>
        /// </summary>
        /// <param name="note">
        /// The <see cref="INote"/> to save
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(INote note, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="INote"/>
        /// </summary>
        /// <param name="notes">
        /// The collection of <see cref="INote"/>s to be saved
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IEnumerable<INote> notes, bool raiseEvents = true);

        /// <summary>
        /// Deletes a <see cref="INote"/>
        /// </summary>
        /// <param name="note">
        /// The <see cref="INote"/> to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Delete(INote note, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="INote"/>
        /// </summary>
        /// <param name="notes">
        /// The collection of <see cref="INote"/>s to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Delete(IEnumerable<INote> notes, bool raiseEvents = true);

     
        /// <summary>
        /// Gets a collection of <see cref="INote"/> for a particular entity
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{INote}"/>.
        /// </returns>
        IEnumerable<INote> GetNotesByEntityKey(Guid entityKey);

        /// <summary>
        /// Gets a collection of <see cref="INote"/> for an entity type
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity tf key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{INote}"/>.
        /// </returns>
        Page<INote> GetNotesByEntityTfKey(Guid entityTfKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

    }
}