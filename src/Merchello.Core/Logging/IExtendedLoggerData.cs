namespace Merchello.Core.Logging
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines additional data that can be passed to remote loggers.
    /// </summary>
    public interface IExtendedLoggerData
    {
        /// <summary>
        /// Gets the categories.
        /// </summary>
        IEnumerable<string> Categories { get; }

        /// <summary>
        /// Adds a category.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        void AddCategory(string category);

        /// <summary>
        /// Adds a range of categories.
        /// </summary>
        /// <param name="categories">
        /// The categories.
        /// </param>
        void AddCategoryRange(IEnumerable<string> categories);

        /// <summary>
        /// Gets a value from an internal collection.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetValue(string alias);

        /// <summary>
        /// Gets a value from an internal collection as a typed object.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <typeparam name="T">
        /// The type of object to return
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T GetValue<T>(string alias) where T : class, new();

        /// <summary>
        /// Sets a string value.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        void SetValue(string alias, string value);

        /// <summary>
        /// Sets an object value.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// The type of object to be stored
        /// </typeparam>
        void SetValue<T>(string alias, T value) where T : class, new();
    }
}