namespace Merchello.Core.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    /// <summary>
    /// Represents extra logger data that can be passed to remote loggers.
    /// </summary>
    public class ExtendedLoggerData : IExtendedLoggerData
    {
        /// <summary>
        /// The categories.
        /// </summary>
        private readonly List<string> _categories = new List<string>();

        /// <summary>
        /// An internal collection to store extra log data.
        /// </summary>
        private readonly IDictionary<string, string> _logData = new Dictionary<string, string>();

        /// <summary>
        /// A reference to the Umbraco logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedLoggerData"/> class.
        /// </summary>
        public ExtendedLoggerData()
            : this(Logger.CreateWithDefaultLog4NetConfiguration())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedLoggerData"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ExtendedLoggerData(ILogger logger)
        {
            Ensure.ParameterNotNull(logger, "logger");
            _logger = logger;
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        public IEnumerable<string> Categories
        {
            get
            {
                return _categories;
            }
        }

        /// <summary>
        /// The add category.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        public void AddCategory(string category)
        {
            if (category.IsNullOrWhiteSpace()) return;
            if (_categories.Contains(category)) return;
            _categories.Add(category);
        }

        /// <summary>
        /// The add category range.
        /// </summary>
        /// <param name="categories">
        /// The categories.
        /// </param>
        public void AddCategoryRange(IEnumerable<string> categories)
        {
            foreach (var c in categories.ToArray())
            {
                AddCategory(c);
            }
        }

        /// <summary>
        /// Returns a value indicating whether the dictionary contains a value for the alias.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the dictionary contains a value.
        /// </returns>
        public bool HasValue(string alias)
        {
            return !GetValue(alias).IsNullOrWhiteSpace();
        }

        /// <summary>
        /// Gets a value from an internal collection.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetValue(string alias)
        {
            return _logData.ContainsKey(alias) ? _logData[alias] : string.Empty;
        }

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
        public T GetValue<T>(string alias) where T : class, new()
        {
            if (!_logData.ContainsKey(alias)) return default(T);

            try
            {
                return JsonConvert.DeserializeObject<T>(_logData[alias]);
            }
            catch (Exception ex)
            {
                // Log the warning with exception and allow to continue
                _logger.WarnWithException<ExtendedLoggerData>(string.Format("Failed to deserialize value for alias: {0}", alias), ex);
            }

            return null;
        }

        /// <summary>
        /// Sets a string value.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetValue(string alias, string value)
        {
            if (value.IsNullOrWhiteSpace()) return;
            if (_logData.ContainsKey(alias))
            {
                _logData.Remove(alias);
            }

            _logData.Add(alias, value);
        }

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
        public void SetValue<T>(string alias, T value) where T : class, new()
        {
            if (value == null) return;

            try
            {
                var serialized = JsonConvert.SerializeObject(value);
                SetValue(alias, serialized);
            }
            catch (Exception ex)
            {
                // Log the warning with exception and allow to continue
                _logger.WarnWithException<ExtendedLoggerData>(string.Format("Failed to serialize value for alias: {0}", alias), ex);
            }
        }
    }
}