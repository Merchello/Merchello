namespace Merchello.Umbraco.Adapters
{
    using Merchello.Core;

    internal static class UmbracoAttemptExtensions
    {
        public static global::Umbraco.Core.Attempt<T> FU<T>(this Attempt<T> merchAttempt)
        {
            return merchAttempt.ForUmbraco<T>();
        }

        public static global::Umbraco.Core.Attempt<T> ForUmbraco<T>(this Attempt<T> merchAttempt)
        {
            // TODO needs to be tested!!!
            return merchAttempt.Success
                       ? global::Umbraco.Core.Attempt<T>.Succeed(merchAttempt.Result)
                       : global::Umbraco.Core.Attempt<T>.Fail(merchAttempt.Result, merchAttempt.Exception);
        }
    }
}