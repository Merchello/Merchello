namespace Merchello.Core.Acquired.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// Provides a convenience methodology for implementing locked access to resources. 
    /// </summary>
    /// <remarks>
    /// Intended as an infrastructure class.
    /// UMBRACO Direct port of Umbraco internal interface to get rid of hard dependency
    /// </remarks>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/WriteLock.cs"/>
    public class WriteLock : IDisposable
    {
        /// <summary>
        /// The <see cref="ReaderWriterLock"/>.
        /// </summary>
        private readonly ReaderWriterLockSlim _rwLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteLock"/> class.
        /// </summary>
        /// <param name="rwLock">The <see cref="ReaderWriterLock"/>.</param>
        public WriteLock(ReaderWriterLockSlim rwLock)
        {
            this._rwLock = rwLock;
            this._rwLock.EnterWriteLock();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        void IDisposable.Dispose()
        {
            this._rwLock.ExitWriteLock();
        }
    }
}