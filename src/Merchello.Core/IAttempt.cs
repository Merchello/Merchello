namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Represents the result of an operation attempt.
    /// </summary>
    /// <typeparam name="T">The type of the attempted operation result.</typeparam>
    internal interface IAttempt<T>
    {
        /// <summary>
        /// Gets the exception associated with an unsuccessful attempt.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Gets the attempt result.
        /// </summary>
        T Result { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IAttempt{T}"/> was successful.
        /// </summary>
        bool Success { get; }
    }
}