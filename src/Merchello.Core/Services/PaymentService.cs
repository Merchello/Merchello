using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Strategies.Payment;
using Umbraco.Core;
using Umbraco.Core.Events;
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
        private readonly PaymentApplicationStrategyBase _defaultPaymentApplicationStrategy;


        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public PaymentService()
            : this(new RepositoryFactory(), new PaymentApplicationStrategy())
        { }

        public PaymentService(RepositoryFactory repositoryFactory, PaymentApplicationStrategyBase defaultPaymentApplicationStrategy)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, defaultPaymentApplicationStrategy)
        { }

        public PaymentService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, PaymentApplicationStrategyBase defaultPaymentApplicationStrategy)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(defaultPaymentApplicationStrategy, "defaultApplyPaymentStrategy");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _defaultPaymentApplicationStrategy = defaultPaymentApplicationStrategy;
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
                
            Created.RaiseEvent(new Events.NewEventArgs<IPayment>(payment), this);

            return payment;
        }


        /// <summary>
        /// Saves a single <see cref="IPayment"/> object
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IPayment payment, bool raiseEvents = true)
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
        /// Saves a single <see cref="IPayment"/> object and applies the payment to an <see cref="IInvoice"/> by creating a <see cref="IAppliedPayment"/> 
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <param name="invoice">The <see cref="IInvoice"/> to be paid</param>        
        /// <param name="transactionDescription">An optional description for the transaction</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void SaveAndApplyPayment(IPayment payment, IInvoice invoice, string transactionDescription = "", bool raiseEvents = true)
        {
            SaveAndApplyPayment(payment, invoice, payment.Amount, transactionDescription, raiseEvents);
        }

        /// <summary>
        /// Saves a single <see cref="IPayment"/> object and applies the payment to an <see cref="IInvoice"/> by creating a <see cref="IAppliedPayment"/> 
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <param name="invoice">The <see cref="IInvoice"/> to be paid</param>
        /// <param name="amountToApply">The amount of the payment to apply.  
        /// This in conjuction with other transaction amounts associated with the payment cannot 
        /// exceed the the total payment amount.
        /// </param>
        /// <param name="transactionDescription">An optional description for the transaction</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void SaveAndApplyPayment(IPayment payment, IInvoice invoice, decimal amountToApply, string transactionDescription = "", bool raiseEvents = true)
        {
            SaveAndApplyPayment(_defaultPaymentApplicationStrategy, payment, invoice, amountToApply, transactionDescription, raiseEvents);
        }

        /// <summary>
        /// Saves a single <see cref="IPayment"/> object and applies the payment to an <see cref="IInvoice"/> by creating a <see cref="IAppliedPayment"/> 
        /// </summary>
        /// <param name="paymentApplicationStrategy">The <see cref="PaymentApplicationStrategyBase"/> to use in applying the payment</param>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <param name="invoice">The <see cref="IInvoice"/> to be paid</param>
        /// <param name="amountToApply">The amount of the payment to apply.  
        /// This in conjuction with other transaction amounts associated with the payment cannot 
        /// exceed the the total payment amount.
        /// </param>
        /// <param name="transactionDescription">An optional description for the transaction</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void SaveAndApplyPayment(PaymentApplicationStrategyBase paymentApplicationStrategy, IPayment payment, IInvoice invoice, decimal amountToApply, string transactionDescription = "", bool raiseEvents = true)
        {
            // save the payment
            Save(payment);

            // TODO : TransactionType 
            paymentApplicationStrategy.ApplyPayment(payment, invoice, amountToApply, AppliedPaymentType.Credit, transactionDescription, raiseEvents);
        }

        /// <summary>
        /// Voids the <see cref="IPayment"/> and all assoicated transactions
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to be voided</param>
        /// <param name="transactionDescription">An optional description to be applied to each of the <see cref="IAppliedPayment"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void SaveAndVoidPayment(IPayment payment, string transactionDescription = "", bool raiseEvents = true)
        {
            throw new NotImplementedException();
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
        public static event TypedEventHandler<IPaymentService, Events.NewEventArgs<IPayment>> Created;


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