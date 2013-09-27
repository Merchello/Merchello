using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Events;
using Umbraco.Core;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Transaction Service 
    /// </summary>
    public class AppliedPaymentService : IAppliedPaymentService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public AppliedPaymentService()
            : this(new RepositoryFactory())
        { }

        public AppliedPaymentService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public AppliedPaymentService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region ITransactionService Members


        /// <summary>
        /// Creates an <see cref="IAppliedPayment"/> object
        /// </summary>
        public IAppliedPayment CreateAppliedPayment(IPayment payment, IInvoice invoice, AppliedPaymentType appliedPaymentType, decimal amount)
        {           
            var typeField = EnumTypeFieldConverter.AppliedPayment().GetTypeField(appliedPaymentType);
            return CreateAppliedPayment(payment, invoice, typeField.TypeKey, amount);
        }

        public IAppliedPayment CreateAppliedPayment(IPayment payment, IInvoice invoice, Guid appliedPaymentTypeFieldKey, decimal amount)
        {
            Mandate.ParameterNotNull(payment, "payment");
            Mandate.ParameterNotNull(invoice, "invoice");

            var transaction = new AppliedPayment(payment, invoice, appliedPaymentTypeFieldKey)
                {                     
                    AppliedPaymentTfKey = appliedPaymentTypeFieldKey,
                    Description = string.Empty, 
                    Amount = amount, 
                    Exported = false
                };
                
            Created.RaiseEvent(new NewEventArgs<IAppliedPayment>(transaction), this);

            return transaction;
        }

        /// <summary>
        /// Saves a single <see cref="IAppliedPayment"/> object
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IAppliedPayment appliedPayment, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IAppliedPayment>(appliedPayment), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAppliedPaymentRepository(uow))
                {
                    repository.AddOrUpdate(appliedPayment);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IAppliedPayment>(appliedPayment), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IAppliedPayment"/> objects.
        /// </summary>
        /// <param name="appliedPaymentList">Collection of <see cref="AppliedPayment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IAppliedPayment> appliedPaymentList, bool raiseEvents = true)
        {
            var transactionArray = appliedPaymentList as IAppliedPayment[] ?? appliedPaymentList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IAppliedPayment>(transactionArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAppliedPaymentRepository(uow))
                {
                    foreach (var transaction in transactionArray)
                    {
                        repository.AddOrUpdate(transaction);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IAppliedPayment>(transactionArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IAppliedPayment"/> object
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IAppliedPayment appliedPayment, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IAppliedPayment>( appliedPayment), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAppliedPaymentRepository(uow))
                {
                    repository.Delete( appliedPayment);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IAppliedPayment>( appliedPayment), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IAppliedPayment"/> objects
        /// </summary>
        /// <param name="appliedPaymentList">Collection of <see cref="IAppliedPayment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IAppliedPayment> appliedPaymentList, bool raiseEvents = true)
        {
            var transactionArray = appliedPaymentList as IAppliedPayment[] ?? appliedPaymentList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IAppliedPayment>(transactionArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAppliedPaymentRepository(uow))
                {
                    foreach (var transaction in transactionArray)
                    {
                        repository.Delete(transaction);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IAppliedPayment>(transactionArray), this);
        }

        /// <summary>
        /// Gets a Transaction by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Transaction</param>
        /// <returns><see cref="IAppliedPayment"/></returns>
        public IAppliedPayment GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateAppliedPaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of Transaction give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IAppliedPayment> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateAppliedPaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        #endregion

        internal IEnumerable<IAppliedPayment> GetAll()
        {
            using (var repository = _repositoryFactory.CreateAppliedPaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IAppliedPaymentService, DeleteEventArgs<IAppliedPayment>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IAppliedPaymentService, DeleteEventArgs<IAppliedPayment>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IAppliedPaymentService, SaveEventArgs<IAppliedPayment>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IAppliedPaymentService, SaveEventArgs<IAppliedPayment>> Saved;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IAppliedPaymentService, NewEventArgs<IAppliedPayment>> Created;

        #endregion


     
    }
}