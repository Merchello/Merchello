using System;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Tests.Base.DataMakers
{
    public static class DataMakerMockSavedExtensions
    {

        #region IdEntity
        

        public static ICustomerAddress MockSavedWithId(this ICustomerAddress entity, int id)
        {
            entity.Id = id;
            ((IdEntity) entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;            
        }

        public static ICustomerItemCache MockSavedWithId(this ICustomerItemCache entity, int id)
        {
            entity.Id = id;
            ((IdEntity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IOrderLineItem MockSavedWithId(this IOrderLineItem entity, int id)
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

        public static IInvoiceLineItem MockSavedWithId(this IInvoiceLineItem entity, int id)
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

        public static IProductVariant MockSavedWithKey(this IProductVariant entity, Guid key)
        {
            ((KeyEntity)entity).UpdatingEntity();
            entity.Key = key;
            entity.ResetDirtyProperties();
            return entity;
        }


        #endregion

    }
}