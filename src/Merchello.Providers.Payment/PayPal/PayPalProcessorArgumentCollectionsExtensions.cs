namespace Merchello.Providers.Payment.PayPal
{
    using Merchello.Core.Gateways.Payment;

    using Constants = Merchello.Providers.Payment.Constants;

	/// <summary>
	/// PayPal ProcessorArgumentCollections extensions
	/// </summary>
	public static class PayPalProcessorArgumentCollectionsExtensions
    {
		/// <summary>
		/// Returns the "ReturnUrl" item of "args" collection, or "elseValue" argument if the first does not exists.
		/// </summary>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <param name="elseValue">
		/// The else Value.
		/// </param>
		/// <returns>
		/// The return URL.
		/// </returns>
		public static string GetPayPalReturnUrl(this ProcessorArgumentCollection args, string elseValue = null)
        {
			return GetArgumentValue(args, Constants.PayPal.ProcessorArgumentsKeys.ReturnUrl, elseValue);
		}
		
		/// <summary>
		/// Returns the "CancelUrl" item of "args" collection, or "elseValue" argument if the first does not exists.
		/// </summary>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <param name="elseValue">
		/// The else Value.
		/// </param>
		/// <returns>
		/// The cancel URL.
		/// </returns>
		public static string GetPayPalCancelUrl(this ProcessorArgumentCollection args, string elseValue = null)
        {
			return GetArgumentValue(args, Constants.PayPal.ProcessorArgumentsKeys.CancelUrl, elseValue);
		}
		
		/// <summary>
		/// Returns the "ProductContentSlug" item of "args" collection, or "elseValue" argument if the first does not exists.
		/// </summary>
		/// <param name="args">
		/// The args.
		/// </param>
		/// <param name="elseValue">
		/// The else Value.
		/// </param>
		/// <returns>
		/// The product content slug.
		/// </returns>
		public static string GetPayPalProductContentSlug(this ProcessorArgumentCollection args, string elseValue = null)
        {
			return GetArgumentValue(args, Constants.PayPal.ProcessorArgumentsKeys.ProductContentSlug, elseValue);
		}

	    /// <summary>
	    /// Safely gets a value.
	    /// </summary>
	    /// <param name="args">
	    /// The args.
	    /// </param>
	    /// <param name="argumentName">
	    /// The argument name.
	    /// </param>
	    /// <param name="elseValue">
	    /// The else value.
	    /// </param>
	    /// <returns>
	    /// Tries to get an argument value with an optional default
	    /// </returns>
	    /// <remarks>
	    /// This was in the original plugin.
	    /// </remarks>
	    private static string GetArgumentValue(ProcessorArgumentCollection args, string argumentName, string elseValue = null)
        {
			string value;
			if (args != null && args.TryGetValue(argumentName, out value)) return value;
			return elseValue;
		}
	}
}
