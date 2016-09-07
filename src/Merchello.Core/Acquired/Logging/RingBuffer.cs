namespace Merchello.Core.Acquired.Logging
{
    using System;

    /// <summary>
    /// Borrowed from https://github.com/cjbhaines/Log4Net.Async - will reference Nuget packages directly in v8 REFACTOR remove when V8 Released
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// UMBRACO_SRC
    internal sealed class RingBuffer<T> : IQueue<T>
    {
        private readonly object lockObject = new object();
        private readonly T[] buffer;
        private readonly int size;
        private int readIndex = 0;
        private int writeIndex = 0;
        private bool bufferFull = false;

        public int Size { get { return this.size; } }

        public event Action<object, EventArgs> BufferOverflow;

        public RingBuffer(int size)
        {
            this.size = size;
            this.buffer = new T[size];
        }

        public void Enqueue(T item)
        {
            var bufferWasFull = false;
            lock (this.lockObject)
            {
                this.buffer[this.writeIndex] = item;
                this.writeIndex = (++this.writeIndex) % this.size;
                if (this.bufferFull)
                {
                    bufferWasFull = true;
                    this.readIndex = this.writeIndex;
                }
                else if (this.writeIndex == this.readIndex)
                {
                    this.bufferFull = true;
                }
            }

            if (bufferWasFull)
            {
                if (this.BufferOverflow != null)
                {
                    this.BufferOverflow(this, EventArgs.Empty);
                }
            }
        }

        public bool TryDequeue(out T ret)
        {
            if (this.readIndex == this.writeIndex && !this.bufferFull)
            {
                ret = default(T);
                return false;
            }
            lock (this.lockObject)
            {
                if (this.readIndex == this.writeIndex && !this.bufferFull)
                {
                    ret = default(T);
                    return false;
                }

                ret = this.buffer[this.readIndex];
                this.buffer[this.readIndex] = default(T);
                this.readIndex = (++this.readIndex) % this.size;
                this.bufferFull = false;
                return true;
            }
        }
    }
}