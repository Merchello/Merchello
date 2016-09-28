namespace Merchello.Core.Persistence.Mappers
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents the result of a model mapped to a DTO (database POCO)
    /// </summary>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v8/src/Umbraco.Core/Persistence/Mappers/DtoMapModel.cs"/>
    internal class DtoMapModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DtoMapModel"/> class.
        /// </summary>
        /// <param name="type">
        /// The type of the model.
        /// </param>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <param name="sourcePropertyName">
        /// The source property name.
        /// </param>
        public DtoMapModel(Type type, PropertyInfo propertyInfo, string sourcePropertyName)
        {
            Type = type;
            PropertyInfo = propertyInfo;
            SourcePropertyName = sourcePropertyName;
        }

        /// <summary>
        /// Gets or sets the source property name.
        /// </summary>
        public string SourcePropertyName { get; private set; }
		
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets or sets the property info.
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }
    }
}