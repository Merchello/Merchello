namespace Merchello.Core.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    using log4net.Core;
    using log4net.Util;

    /// <summary>
    /// An asynchronous appender based on <see cref="BlockingCollection{T}"/>
    /// </summary>
    /// <remarks>
    /// Borrowed from https://github.com/cjbhaines/Log4Net.Async - will reference Nuget packages directly in v8 REFACTOR remove when V8 Released
    /// </remarks>
    /// UMBRACO_SRC
    public class ParallelForwardingAppender : AsyncForwardingAppenderBase, IDisposable
    {
        #region Private Members

        private const int DefaultBufferSize = 1000;
        private BlockingCollection<LoggingEventContext> _loggingEvents;
        private CancellationTokenSource _loggingCancelationTokenSource;
        private CancellationToken _loggingCancelationToken;
        private Task _loggingTask;
        private Double _shutdownFlushTimeout = 2;
        private TimeSpan _shutdownFlushTimespan = TimeSpan.FromSeconds(2);
        private static readonly Type ThisType = typeof(ParallelForwardingAppender);
        private volatile bool shutDownRequested;
        private int? bufferSize = DefaultBufferSize;

        #endregion Private Members

        #region Properties

        /// <summary>
        /// Gets or sets the number of LoggingEvents that will be buffered.  Set to null for unlimited.
        /// </summary>
        public override int? BufferSize
        {
            get { return this.bufferSize; }
            set { this.bufferSize = value; }
        }

        public int BufferEntryCount
        {
            get
            {
                if (this._loggingEvents == null) return 0;
                return this._loggingEvents.Count;
            }
        }

        /// <summary>
        /// Gets or sets the time period in which the system will wait for appenders to flush before canceling the background task.
        /// </summary>
        public Double ShutdownFlushTimeout
        {
            get
            {
                return this._shutdownFlushTimeout;
            }
            set
            {
                this._shutdownFlushTimeout = value;
            }
        }

        protected override string InternalLoggerName
        {
            get
            {
                {
                    return "ParallelForwardingAppender";
                }
            }
        }

        #endregion Properties

        #region Startup

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            this._shutdownFlushTimespan = TimeSpan.FromSeconds(this._shutdownFlushTimeout);
            this.StartForwarding();
        }

        private void StartForwarding()
        {
            if (this.shutDownRequested)
            {
                return;
            }
            //Create a collection which will block the thread and wait for new entries
            //if the collection is empty
            if (this.BufferSize.HasValue && this.BufferSize > 0)
            {
                this._loggingEvents = new BlockingCollection<LoggingEventContext>(this.BufferSize.Value);
            }
            else
            {
                //No limit on the number of events.
                this._loggingEvents = new BlockingCollection<LoggingEventContext>();
            }
            //The cancellation token is used to cancel a running task gracefully.
            this._loggingCancelationTokenSource = new CancellationTokenSource();
            this._loggingCancelationToken = this._loggingCancelationTokenSource.Token;
            this._loggingTask = new Task(this.SubscriberLoop, this._loggingCancelationToken);
            this._loggingTask.Start();
        }

        #endregion Startup

        #region Shutdown

        private void CompleteSubscriberTask()
        {
            this.shutDownRequested = true;
            if (this._loggingEvents == null || this._loggingEvents.IsAddingCompleted)
            {
                return;
            }
            //Don't allow more entries to be added.
            this._loggingEvents.CompleteAdding();
            //Allow some time to flush
            Thread.Sleep(this._shutdownFlushTimespan);
            if (!this._loggingTask.IsCompleted && !this._loggingCancelationToken.IsCancellationRequested)
            {
                this._loggingCancelationTokenSource.Cancel();
                //Wait here so that the error logging messages do not get into a random order.
                //Don't pass the cancellation token because we are not interested
                //in catching the OperationCanceledException that results.
                this._loggingTask.Wait();
            }
            if (!this._loggingEvents.IsCompleted)
            {
                this.ForwardInternalError("The buffer was not able to be flushed before timeout occurred.", null, ThisType);
            }
        }

        protected override void OnClose()
        {
            this.CompleteSubscriberTask();
            base.OnClose();
        }

        #endregion Shutdown

        #region Appending

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (this._loggingEvents == null || this._loggingEvents.IsAddingCompleted || loggingEvent == null)
            {
                return;
            }

            loggingEvent.Fix = this.Fix;
            //In the case where blocking on a full collection, and the task is subsequently completed, the cancellation token
            //will prevent the entry from attempting to add to the completed collection which would result in an exception.
            this._loggingEvents.Add(new LoggingEventContext(loggingEvent, this.HttpContext), this._loggingCancelationToken);
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            if (this._loggingEvents == null || this._loggingEvents.IsAddingCompleted || loggingEvents == null)
            {
                return;
            }

            foreach (var loggingEvent in loggingEvents)
            {
                this.Append(loggingEvent);
            }
        }

        #endregion Appending

        #region Forwarding

        /// <summary>
        /// Iterates over a BlockingCollection containing LoggingEvents.
        /// </summary>
        private void SubscriberLoop()
        {
            Thread.CurrentThread.Name = String.Format("{0} ParallelForwardingAppender Subscriber Task", this.Name);
            //The task will continue in a blocking loop until
            //the queue is marked as adding completed, or the task is canceled.
            try
            {
                //This call blocks until an item is available or until adding is completed
                foreach (var entry in this._loggingEvents.GetConsumingEnumerable(this._loggingCancelationToken))
                {
                    this.HttpContext = entry.HttpContext;
                    this.ForwardLoggingEvent(entry.LoggingEvent, ThisType);
                }
            }
            catch (OperationCanceledException ex)
            {
                //The thread was canceled before all entries could be forwarded and the collection completed.
                this.ForwardInternalError("Subscriber task was canceled before completion.", ex, ThisType);
                //Cancellation is called in the CompleteSubscriberTask so don't call that again.
            }
            catch (ThreadAbortException ex)
            {
                //Thread abort may occur on domain unload.
                this.ForwardInternalError("Subscriber task was aborted.", ex, ThisType);
                //Cannot recover from a thread abort so complete the task.
                this.CompleteSubscriberTask();
                //The exception is swallowed because we don't want the client application
                //to halt due to a logging issue.
            }
            catch (Exception ex)
            {
                //On exception, try to log the exception
                this.ForwardInternalError("Subscriber task error in forwarding loop.", ex, ThisType);
                //Any error in the loop is going to be some sort of extenuating circumstance from which we
                //probably cannot recover anyway.   Complete subscribing.
                this.CompleteSubscriberTask();
            }
        }

        #endregion Forwarding

        #region IDisposable Implementation

        private bool _disposed = false;

        //Implement IDisposable.
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (this._loggingTask != null)
                    {
                        if (!(this._loggingTask.IsCanceled || this._loggingTask.IsCompleted || this._loggingTask.IsFaulted))
                        {
                            try
                            {
                                this.CompleteSubscriberTask();
                            }
                            catch (Exception ex)
                            {
                                LogLog.Error(ThisType, "Exception Completing Subscriber Task in Dispose Method", ex);
                            }
                        }
                        try
                        {
                            this._loggingTask.Dispose();
                        }
                        catch (Exception ex)
                        {
                            LogLog.Error(ThisType, "Exception Disposing Logging Task", ex);
                        }
                        finally
                        {
                            this._loggingTask = null;
                        }
                    }
                    if (this._loggingEvents != null)
                    {
                        try
                        {
                            this._loggingEvents.Dispose();
                        }
                        catch (Exception ex)
                        {
                            LogLog.Error(ThisType, "Exception Disposing BlockingCollection", ex);
                        }
                        finally
                        {
                            this._loggingEvents = null;
                        }
                    }
                    if (this._loggingCancelationTokenSource != null)
                    {
                        try
                        {
                            this._loggingCancelationTokenSource.Dispose();
                        }
                        catch (Exception ex)
                        {
                            LogLog.Error(ThisType, "Exception Disposing CancellationTokenSource", ex);
                        }
                        finally
                        {
                            this._loggingCancelationTokenSource = null;
                        }
                    }
                }
                this._disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~ParallelForwardingAppender()
        {
            // Simply call Dispose(false).
            this.Dispose(false);
        }

        #endregion IDisposable Implementation
    }
}