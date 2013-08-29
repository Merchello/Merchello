﻿using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class InvoiceRepository : MerchelloPetaPocoRepositoryBase<int, IInvoice>, IInvoiceRepository
    {

        public InvoiceRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public InvoiceRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IInvoice>


        protected override IInvoice PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<InvoiceDto, CustomerDto, InvoiceStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new InvoiceFactory();

            var invoice = factory.BuildEntity(dto);

            return invoice;
        }

        protected override IEnumerable<IInvoice> PerformGetAll(params int[] ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    yield return Get(id);
                }
            }
            else
            {
                var factory = new InvoiceFactory();
                var dtos = Database.Fetch<InvoiceDto, CustomerDto, InvoiceStatusDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IInvoice>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<InvoiceDto>()
               .InnerJoin<CustomerDto>()
               .On<InvoiceDto, CustomerDto>(left => left.CustomerKey, right => right.Key)
               .InnerJoin<InvoiceStatusDto>()
               .On<InvoiceDto, InvoiceStatusDto>(left => left.InvoiceStatusId, right => right.Id);

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchInvoice.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {                    
                    "DELETE FROM merchTransaction WHERE invoiceId = @Id",
                    "DELETE FROM merchShipment WHERE invoiceId = @Id",
                    "DELETE FROM merchInvoiceItem WHERE invoiceId = @Id",
                    "DELETE FROM merchInvoice WHERE id = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(IInvoice entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new InvoiceFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IInvoice entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new InvoiceFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IInvoice entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }

        protected override IEnumerable<IInvoice> PerformGetByQuery(IQuery<IInvoice> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IInvoice>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceDto, CustomerDto, InvoiceStatusDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
