namespace Merchello.Plugin.Payments.Braintree
{
    /// <summary>
    /// Assists in the mapping of the Braintree environment.
    /// </summary>
    public enum EnvironmentType
    {
        /// <summary>
        /// The development environment.
        /// </summary>
        Development,

        /// <summary>
        /// The QA environment.
        /// </summary>
        Qa,

        /// <summary>
        /// The sandbox environment.
        /// </summary>
        Sandbox,

        /// <summary>
        /// The production environment.
        /// </summary>
        Production
    }
}