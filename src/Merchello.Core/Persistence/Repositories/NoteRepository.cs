﻿namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;

    /// <summary>
    /// Represents the note repository.
    /// </summary>
    internal class NoteRepository : PagedRepositoryBase<INote, NoteDto>, INoteRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoteRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public NoteRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {
        }

        /// <summary>
        /// Searches the notes
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public override Page<Guid> SearchKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var terms = searchTerm.Split(' ');

            var sql = new Sql();
            sql.Select("*").From<NoteDto>();

            if (terms.Any())
            {
                var preparedTerms = string.Format("%{0}%", string.Join("%", terms));

                sql.Where("message LIKE @msg", new { @msg = preparedTerms });
            }

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets a <see cref="INote"/>
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        protected override INote PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
            .Where(GetBaseWhereClause(), new { Key = key });


            var dto = Database.Fetch<NoteDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new NoteFactory();

            var customer = factory.BuildEntity(dto);

            return customer;
        }

        /// <summary>
        /// Gets the collection of all <see cref="INote"/>
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{INote}"/>.
        /// </returns>
        protected override IEnumerable<INote> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var id in keys)
                {
                    yield return Get(id);
                }
            }
            else
            {
                var factory = new NoteFactory();
                var dtos = Database.Fetch<NoteDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="INote"/> by query
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{INote}"/>.
        /// </returns>
        protected override IEnumerable<INote> PerformGetByQuery(IQuery<INote> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<INote>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<NoteDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base query.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<NoteDto>();

            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The base where clause.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchNote.pk = @Key";
        }

        /// <summary>
        /// Gets a collection of delete clauses
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchNote WHERE merchNote.pk = @Key"
                };

            return list;
        }

        /// <summary>
        /// Persist new note.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(INote entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new NoteFactory();
            var dto = factory.BuildDto(entity);
            Database.Insert(dto);
            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates a note
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(INote entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new NoteFactory();
            var dto = factory.BuildDto(entity);
            Database.Update(dto);
            
            entity.ResetDirtyProperties();
        }

    }
}