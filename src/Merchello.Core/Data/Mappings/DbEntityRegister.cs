namespace Merchello.Core.Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Merchello.Core.Logging;

    /// <summary>
    /// A register for Entity Framework entities types.
    /// </summary>
    internal class DbEntityRegister : IDbEntityRegister
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
        /// Initializes a new instance of the <see cref="DbEntityRegister"/> class.
        /// </summary>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        public DbEntityRegister(ILogger logger)
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
        public IEnumerable<IEntityMap> GetInstantiations()
        {
            return this.InstanceTypes.Select(t => Activator.CreateInstance(t) as IEntityMap).Where(i => i != null);
        }

        /// <summary>
        /// Initializes the registry.
        /// </summary>
        public void Initialize()
        {
            this._logger.Info<DbEntityRegister>("Initializing");

            var typesToRegister =
                this.GetType()
                    .GetTypeInfo()
                    .Assembly.DefinedTypes.Where(ti => !string.IsNullOrEmpty(ti.Namespace))
                    .Where(ti =>
                        ti.IsClass &&
                        typeof(IEntityMap).IsAssignableFrom(ti.AsType())
                        )
                    //.Where(
                    //    ti =>
                    //        ti.BaseType != null && ti.BaseType.IsConstructedGenericType
                    //        && typeof(IEntityMap).IsAssignableFrom(ti.AsType()))
                    .Select(ti => ti.AsType())
                    .ToArray();

            this._logger.Info<DbEntityRegister>($"Found {typesToRegister.Count()} types to register");

            foreach (var t in typesToRegister)
            {
                this._logger.Info<DbEntityRegister>($"Adding {t.FullName} to registry");
                this._types.Add(t);
            }

            this._logger.Info<DbEntityRegister>("Completed adding types to register");
        }
    }
}