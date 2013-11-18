using System;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;


namespace Merchello.Tests.Base.DataMakers
{
    public static class DataMakerMockSavedExtensions
    {
        

        public static ICustomerAddress MockSavedWithKey(this ICustomerAddress entity, Guid key)
        {
            entity.Key = key;
            ((Entity) entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;            
        }

        public static IItemCache MockSavedWithKey(this IItemCache entity, Guid key)
        {
            entity.Key = key;
            ((Entity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IOrderLineItem MockSavedWithKey(this IOrderLineItem entity, Guid key)
        {
            entity.Key = key;
            ((Entity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        //public static IInvoice MockSavedWithId(this IInvoice entity, Guid key)
        //{
        //    entity.Key = key;
        //    ((Entity)entity).AddingEntity();
        //    entity.ResetDirtyProperties();
        //    return entity;
        //}

        public static IInvoiceLineItem MockSavedWithKey(this IInvoiceLineItem entity, Guid key)
        {
            entity.Key = key;
            ((Entity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IInvoiceStatus MockSavedWithKey(this IInvoiceStatus entity, Guid key)
        {
            entity.Key = key;
            ((Entity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IPayment MockSavedWithKey(this IPayment entity, Guid key)
        {
            entity.Key = key;
            ((Entity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IShipment MockSavedWithKey(this IShipment entity, Guid key)
        {
            entity.Key = key;
            ((Entity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IShipMethod MockSavedWithKey(this IShipMethod entity, Guid key)
        {
            entity.Key = key;
            ((Entity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IAppliedPayment MockSavedWithKey(this IAppliedPayment entity, Guid key)
        {
            entity.Key = key;
            ((Entity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IWarehouse MockSavedWithKey(this IWarehouse entity, Guid key)
        {
            entity.Key = key;
            ((Entity)entity).AddingEntity();
            entity.ResetDirtyProperties();
            return entity;
        }



        public static ICustomer MockSavedWithKey(this ICustomer entity, Guid key)
        {
            ((Entity)entity).UpdatingEntity();
            entity.Key = Guid.NewGuid();
            entity.Key = key;
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IAnonymousCustomer MockSavedWithKey(this IAnonymousCustomer entity, Guid key)
        {
            ((Entity)entity).UpdatingEntity();
            entity.Key = key;
            entity.ResetDirtyProperties();
            return entity;
        }

        public static IProductVariant MockSavedWithKey(this IProductVariant entity, Guid key)
        {
            ((Entity)entity).UpdatingEntity();
            entity.Key = key;
            entity.ResetDirtyProperties();
            return entity;
        }

    }
}