using System;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Tests.Base.DataMakers
{
    public static class DataMakerMockSavedExtensions
    {

        #region IdEntity
        

        public static IAddress MockSavedWithId(this IAddress entity, int id)
        {
            entity.Id = id;
            ((IdEntity) entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;            
        }

        public static ICustomerRegistry MockSavedWithId(this ICustomerRegistry entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IPurchaseLineItem MockSavedWithId(this IPurchaseLineItem entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IInvoice MockSavedWithId(this IInvoice entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IInvoiceItem MockSavedWithId(this IInvoiceItem entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IInvoiceStatus MockSavedWithId(this IInvoiceStatus entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IPayment MockSavedWithId(this IPayment entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IShipment MockSavedWithId(this IShipment entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IShipMethod MockSavedWithId(this IShipMethod entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IAppliedPayment MockSavedWithId(this IAppliedPayment entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IWarehouse MockSavedWithId(this IWarehouse entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        #endregion


        #region KeyEntity

        public static ICustomer MockSavedWithKey(this ICustomer entity, Guid key)
        {
            ((KeyEntity)entity).UpdatingEntity();
            entity.Key = key;
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IAnonymousCustomer MockSavedWithKey(this IAnonymousCustomer entity, Guid key)
        {
            ((KeyEntity)entity).UpdatingEntity();
            entity.Key = key;
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IProductActual MockSavedWithKey(this IProductActual entity, Guid key)
        {
            ((KeyEntity)entity).UpdatingEntity();
            entity.Key = key;
            entity.ResetDirtyProperties();
            return entity;
        }


        #endregion

    }
}