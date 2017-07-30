namespace Merchello.Core.Registers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Merchello.Core.Data.Mappings;
    using Merchello.Core.Logging;

    /// <summary>
    /// A register for database mapping types.
    /// </summary>
    internal class DbMappingRegister : IDBMappingRegister
    {
        /// <summary>
        /// The <see cref="Type"/>s registered.
        /// </summary>
        private readonly HashSet<Type> _types = new HashSet<Type>();

        /// <summary>
        /// The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbMappingRegister"/> class.
        /// </summary>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        public DbMappingRegister(ILogger logger)
        {
            this._logger = logger;
            this.Initialize();
        }


        /// <summary>
        /// The instance types.
        /// </summary>
        public IEnumerable<Type> InstanceTypes => this._types;

        /// <summary>
        /// Gets the instantiated instances.
        /// </summary>
        /// <returns>
        /// The collection of instantiated builder configurations.
        /// </returns>
        public IEnumerable<dynamic> GetInstantiations()
        {
            return this.InstanceTypes.Select(Activator.CreateInstance);
        }

        /// <summary>
        /// Initializes the registry.
        /// </summary>
        public void Initialize()
        {
            this._logger.Info<DbMappingRegister>("Initializing");

            var typesToRegister =
                this.GetType()
                    .GetTypeInfo()
                    .Assembly.DefinedTypes.Where(ti => !string.IsNullOrEmpty(ti.Namespace))
                    .Where(
                        ti =>
                            ti.BaseType != null && ti.BaseType.IsConstructedGenericType
                            && ti.BaseType.GetGenericTypeDefinition() == typeof(DbEntityConfiguration<>))
                    .Select(ti => ti.AsType())
                    .ToArray();

            this._logger.Info<DbMappingRegister>($"Found {typesToRegister.Count()} types to register");

            foreach (var t in typesToRegister)
            {
                this._logger.Info<DbMappingRegister>($"Adding {t.FullName} to registry");
                this._types.Add(t);
            }

            this._logger.Info<DbMappingRegister>("Completed adding types to register");
        }
    }
}