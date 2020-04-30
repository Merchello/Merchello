namespace Merchello.Core
{
    using System;

    using Umbraco.Core;

    /// <summary>
    /// Wraps Mandate with some common parameter checks
    /// </summary>
    internal static class MandateWrapperExtensions
    {
        /// <summary>
        /// Mandates a GUID is not empty
        /// </summary>
        /// <param name="value">
        /// The GUID value.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        public static void ParameterNotEmptyGuid(this Guid value, string parameterName)
        {
            Ensure.ParameterCondition(!Guid.Empty.Equals(value), parameterName);
        }
    }
}