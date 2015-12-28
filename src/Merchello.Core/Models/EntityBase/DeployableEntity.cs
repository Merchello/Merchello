namespace Merchello.Core.Models.EntityBase
{
    using System;

    /// <summary>
    /// Represents a deployable entity.
    /// </summary>
    public class DeployableEntity : Entity
    {
        /// <summary>
        /// Allows a key to be set when creating an entity given that GUIDs should match between environments
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <remarks>
        /// You need to be careful that this is only called when you are certain that an entity with the given key
        /// does not already exist in the target environment or there will be a constraint violation.
        /// </remarks>
        internal void SetKeyForDeploymentCreate(Guid key)
        {
            Key = key;
            HasIdentity = false;
        }
    }
}