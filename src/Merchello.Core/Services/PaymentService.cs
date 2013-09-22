using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.OrderFulfillment.Strategies.Payment;
using Merchello.Core.Persistence;
using Merchello.Core.Events;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Payment Service 
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public PaymentService()
            : this(new RepositoryFactory())
        { }

        public PaymentService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public PaymentService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region IPaymentService Members


        /// <summary>
        /// Creates an <see cref="IPayment"/> object
        /// </summary>
        public IPayment CreatePayment(ICustomer customer, Guid providerKey, PaymentMethodType paymentMethodType, string paymentMethodName, string referenceNumber, decimal amount)
        {
            var typeFieldKey = EnumTypeFieldConverter.PaymentMethod().GetTypeField(paymentMethodType).TypeKey;
            return CreatePayment(customer, providerKey, typeFieldKey, paymentMethodName, referenceNumber, amount);
        }

        internal IPayment CreatePayment(ICustomer customer, Guid providerKey, Guid paymentTypeFieldKey, string paymentMethodName, string referenceNumber, decimal amount)
        {
            var payment = new Payment(customer, paymentTypeFieldKey, amount)
                { 
                    ProviderKey = providerKey, 
                    PaymentTypeFieldKey = paymentTypeFieldKey, 
                    PaymentMethodName = paymentMethodName, 
                    ReferenceNumber = referenceNumber,
                    Exported = false
                };
                
            Created.RaiseEvent(new NewEventArgs<IPayment>(payment), this);

            return payment;
        }


        /// <summary>
        /// Saves a single <see cref="IPayment"/> object
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IPayment payment, bool raiseEvents = true)
        {
            SaveByPaymnetFulfillmentStrategy(new PaymentNotAppliedStrategy(payment, raiseEvents));
        }

        public void SaveAndApplyInFull(IPayment payment, IInvoice invoice, string description = "", bool raiseEvents = true)
        {
            SaveByPaymnetFulfillmentStrategy(new PaymentInFullStrategy(payment, invoice, description, raiseEvents));
        }

        public void SaveByPaymnetFulfillmentStrategy(PaymentFulfillmentStrategyBase paymentFulfillmentStrategy)
        {
            paymentFulfillmentStrategy.Process();
        }

        public void SaveNotApplied(IPayment payment, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IPayment>(payment), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreatePaymentRepository(uow))
                {
                    repository.AddOrUpdate(payment);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IPayment>(payment), this);
            }
        }

        

        /// <summary>
        /// Saves a collection of <see cref="IPayment"/> objects.
        /// </summary>
        /// <param name="paymentList">Collection of <see cref="Payment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IPayment> paymentList, bool raiseEvents = true)
        {
            var paymentArray = paymentList as IPayment[] ?? paymentList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IPayment>(paymentArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreatePaymentRepository(uow))
                {
                    foreach (var payment in paymentArray)
                    {
                        repository.AddOrUpdate(payment);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IPayment>(paymentArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IPayment"/> object
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IPayment payment, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IPayment>( payment), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreatePaymentRepository(uow))
                {
                    repository.Delete( payment);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IPayment>( payment), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IPayment"/> objects
        /// </summary>
        /// <param name="paymentList">Collection of <see cref="IPayment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IPayment> paymentList, bool raiseEvents = true)
        {
            var paymentArray = paymentList as IPayment[] ?? paymentList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IPayment>(paymentArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreatePaymentRepository(uow))
                {
                    foreach (var payment in paymentArray)
                    {
                        repository.Delete(payment);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IPayment>(paymentArray), this);
        }

        /// <summary>
        /// Gets a Payment by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Payment</param>
        /// <returns><see cref="IPayment"/></returns>
        public IPayment GetById(int id)
        {
            using (var repository = _repositoryFactory.CreatePaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of Payment give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IPayment> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreatePaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        /// <summary>
        /// Gets a list of <see cref="IPayment"/> for a customer
        /// </summary>
        /// <param name="customerKey">The key of for the customer</param>
        /// <returns>A collection of <see cref="IPayment"/></returns>
        public IEnumerable<IPayment> GetPaymentsByCustomer(Guid customerKey)
        {
            using (var repository = _repositoryFactory.CreatePaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IPayment>.Builder.Where(x => x.CustomerKey == customerKey);
                return repository.GetByQuery(query);
            }
        }

        #endregion

        internal IEnumerable<IPayment> GetAll()
        {
            using (var repository = _repositoryFactory.CreatePaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IPaymentService, NewEventArgs<IPayment>> Created;


        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IPaymentService, DeleteEventArgs<IPayment>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IPaymentService, DeleteEventArgs<IPayment>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IPaymentService, SaveEventArgs<IPayment>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IPaymentService, SaveEventArgs<IPayment>> Saved;

        
        #endregion


     
    }
}