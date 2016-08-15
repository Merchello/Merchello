using System;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Persistence;


namespace Merchello.Tests.Base.Respositories.UnitOfWork
{
    public class MockDatabaseUnitOfWork : DisposableObject, IDatabaseUnitOfWork
    {

        private readonly UmbracoDatabase _database;

        public MockDatabaseUnitOfWork()
        {

            CommitCalled = false;
            InsertCalled = false;
            DeleteCalled = false;
            _database = null;
        }

        public UmbracoDatabase Database
        {
            get { return _database; }
        }

        public bool CommitCalled { get; set; }
        public void Commit()
        {
            CommitCalled = true;
            Committed(this);
        }

        public void CommitBulk<TEntity>() where TEntity : IEntity
        {
            CommitCalled = true;
            Committed(this);
        }

        public object Key
        {
            get { return Guid.NewGuid(); }
        }

        public bool InsertCalled { get; set; }
        public void RegisterAdded(IEntity entity, IUnitOfWorkRepository repository)
        {
            InsertCalled = true;
        }

        public bool UpdateCalled { get; set; }
        public void RegisterChanged(IEntity entity, IUnitOfWorkRepository repository)
        {
            UpdateCalled = true;
        }

        public bool DeleteCalled { get; set; }
        public void RegisterRemoved(IEntity entity, IUnitOfWorkRepository repository)
        {
            DeleteCalled = true;
        }

        protected override void DisposeResources()
        {
            
        }

        public delegate void CommitEventHandler(object sender);
        public static event CommitEventHandler Committed;

    }
}
