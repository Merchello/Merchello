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
    public class TransactionService : ITransactionService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public TransactionService()
            : this(new RepositoryFactory())
        { }

        public TransactionService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public TransactionService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region ITransactionService Members


        /// <summary>
        /// Creates an <see cref="ITransaction"/> object
        /// </summary>
        public ITransaction CreateTransaction(IPayment payment, IInvoice invoice, TransactionType transactionType, decimal amount)
        {
            var typeField = EnumTypeFieldConverter.Transaction().GetTypeField(transactionType);
            return CreateTransaction(payment, invoice, typeField.TypeKey, amount);
        }

        public ITransaction CreateTransaction(IPayment payment, IInvoice invoice, Guid transactionTypeFieldKey, decimal amount)
        {
           
            var transaction = new Transaction(payment, invoice, transactionTypeFieldKey)
                {                     
                    TransactionTypeFieldKey = transactionTypeFieldKey,
                    Description = string.Empty, 
                    Amount = amount, 
                    Exported = false
                };
                
            Created.RaiseEvent(new NewEventArgs<ITransaction>(transaction), this);

            return transaction;
        }

        /// <summary>
        /// Saves a single <see cref="ITransaction"/> object
        /// </summary>
        /// <param name="transaction">The <see cref="ITransaction"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(ITransaction transaction, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ITransaction>(transaction), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateTransactionRepository(uow))
                {
                    repository.AddOrUpdate(transaction);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ITransaction>(transaction), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="ITransaction"/> objects.
        /// </summary>
        /// <param name="transactionList">Collection of <see cref="Transaction"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<ITransaction> transactionList, bool raiseEvents = true)
        {
            var transactionArray = transactionList as ITransaction[] ?? transactionList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ITransaction>(transactionArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateTransactionRepository(uow))
                {
                    foreach (var transaction in transactionArray)
                    {
                        repository.AddOrUpdate(transaction);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ITransaction>(transactionArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ITransaction"/> object
        /// </summary>
        /// <param name="transaction">The <see cref="ITransaction"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(ITransaction transaction, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ITransaction>( transaction), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateTransactionRepository(uow))
                {
                    repository.Delete( transaction);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ITransaction>( transaction), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="ITransaction"/> objects
        /// </summary>
        /// <param name="transactionList">Collection of <see cref="ITransaction"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<ITransaction> transactionList, bool raiseEvents = true)
        {
            var transactionArray = transactionList as ITransaction[] ?? transactionList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ITransaction>(transactionArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateTransactionRepository(uow))
                {
                    foreach (var transaction in transactionArray)
                    {
                        repository.Delete(transaction);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ITransaction>(transactionArray), this);
        }

        /// <summary>
        /// Gets a Transaction by its unique id - pk
        /// </summary>
        /// <param name="id">int Id for the Transaction</param>
        /// <returns><see cref="ITransaction"/></returns>
        public ITransaction GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateTransactionRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a list of Transaction give a list of unique keys
        /// </summary>
        /// <param name="ids">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<ITransaction> GetByIds(IEnumerable<int> ids)
        {
            using (var repository = _repositoryFactory.CreateTransactionRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids.ToArray());
            }
        }

        #endregion

        internal IEnumerable<ITransaction> GetAll()
        {
            using (var repository = _repositoryFactory.CreateTransactionRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ITransactionService, DeleteEventArgs<ITransaction>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ITransactionService, DeleteEventArgs<ITransaction>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ITransactionService, SaveEventArgs<ITransaction>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ITransactionService, SaveEventArgs<ITransaction>> Saved;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ITransactionService, NewEventArgs<ITransaction>> Created;

        #endregion


     
    }
}