namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Models.Interfaces;
    using Persistence.Querying;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// Defines an AuditLogService.
    /// </summary>
    public interface IAuditLogService : IPageCachedService<IAuditLog>
    {
        /// <summary>
        /// Creates an audit record and saves it to the database
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="isError">
        /// Designates whether or not this is an error log record
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        IAuditLog CreateAuditLogWithKey(string message, bool isError = false, bool raiseEvents = true);

        /// <summary>
        /// Creates an audit record and saves it to the database
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="isError">
        /// Designates whether or not this is an error log record
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        IAuditLog CreateAuditLogWithKey(string message, ExtendedDataCollection extendedData, bool isError = false, bool raiseEvents = true);

        /// <summary>
        /// Creates an audit record and saves it to the database
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
        /// <param name="isError">
        /// The is error.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        IAuditLog CreateAuditLogWithKey(Guid? entityKey, EntityType entityType, string message, bool isError = false, bool raiseEvents = true);

        /// <summary>
        /// Creates an audit record and saves it to the database
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
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="isError">
        /// The is error.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        IAuditLog CreateAuditLogWithKey(Guid? entityKey, EntityType entityType, string message, ExtendedDataCollection extendedData, bool isError = false, bool raiseEvents = true);

        ///// <summary>
        ///// Creates an audit record and saves it to the database
        ///// </summary>
        ///// <param name="entityKey">
        ///// The entity key.
        ///// </param>
        ///// <param name="entityTfKey">
        ///// The entity type field key.
        ///// </param>
        ///// <param name="message">
        ///// The message.
        ///// </param>
        ///// <param name="isError">
        ///// Designates whether or not this is an error log record
        ///// </param>
        ///// <param name="raiseEvents">
        ///// Optional boolean indicating whether or not to raise events
        ///// </param>
        ///// <returns>
        ///// The <see cref="IAuditLog"/>.
        ///// </returns>
        //IAuditLog CreateAuditLogWithKey(Guid? entityKey, Guid? entityTfKey, string message, bool isError = false, bool raiseEvents = true);

        /// <summary>
        /// Creates an audit record and saves it to the database
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
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="isError">
        /// Designates whether or not this is an error log record
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        IAuditLog CreateAuditLogWithKey(Guid? entityKey, Guid? entityTfKey, string message, ExtendedDataCollection extendedData, bool isError = false, bool raiseEvents = true);

        /// <summary>
        /// Saves an <see cref="IAuditLog"/>
        /// </summary>
        /// <param name="auditLog">
        /// The <see cref="IAuditLog"/> to save
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IAuditLog auditLog, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IAuditLog"/>
        /// </summary>
        /// <param name="auditLogs">
        /// The collection of <see cref="IAuditLog"/>s to be saved
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IEnumerable<IAuditLog> auditLogs, bool raiseEvents = true);

        /// <summary>
        /// Deletes a <see cref="IAuditLog"/>
        /// </summary>
        /// <param name="auditLog">
        /// The <see cref="IAuditLog"/> to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Delete(IAuditLog auditLog, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IAuditLog"/>
        /// </summary>
        /// <param name="auditLogs">
        /// The collection of <see cref="IAuditLog"/>s to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Delete(IEnumerable<IAuditLog> auditLogs, bool raiseEvents = true);

        /// <summary>
        /// Deletes all error logs
        /// </summary>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void DeleteErrorAuditLogs(bool raiseEvents = true);

        /// <summary>
        /// Gets a collection of <see cref="IAuditLog"/> for a particular entity
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IAuditLog}"/>.
        /// </returns>
        IEnumerable<IAuditLog> GetAuditLogsByEntityKey(Guid entityKey);

        /// <summary>
        /// Gets a collection of <see cref="IAuditLog"/> for an entity type
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
        /// The <see cref="Page{IAuditLog}"/>.
        /// </returns>
        Page<IAuditLog> GetAuditLogsByEntityTfKey(Guid entityTfKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets a collection of <see cref="IAuditLog"/> where IsError == true
        /// </summary>
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
        /// The <see cref="Page{IAuditLog}"/>.
        /// </returns>
        Page<IAuditLog> GetErrorAuditLogs(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);
    }
}