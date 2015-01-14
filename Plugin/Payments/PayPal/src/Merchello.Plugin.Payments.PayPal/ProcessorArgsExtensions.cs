﻿using Merchello.Core.Gateways.Payment;

namespace Merchello.Plugin.Payments.PayPal
{
	/// <summary>
	/// Extended data utiltity extensions
	/// </summary>
	public static class ProcessorArgsExtensions
	{
		

		/// <summary>
		/// Returns the "ReturnUrl" item of "args" collection, or "elseValue" argument if the first does not exists.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="elseValue"></param>
		/// <returns></returns>
		public static string GetReturnUrl(this ProcessorArgumentCollection args, string elseValue = null) {
			return GetArgumentValue(args, Constants.ProcessorArgumentsKeys.ReturnUrl, elseValue);
		}
		
		/// <summary>
		/// Returns the "CancelUrl" item of "args" collection, or "elseValue" argument if the first does not exists.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="elseValue"></param>
		/// <returns></returns>
		public static string GetCancelUrl(this ProcessorArgumentCollection args, string elseValue = null) {
			return GetArgumentValue(args, Constants.ProcessorArgumentsKeys.CancelUrl, elseValue);
		}
		
		/// <summary>
		/// Returns the "ArticleBySkuPath" item of "args" collection, or "elseValue" argument if the first does not exists.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="elseValue"></param>
		/// <returns></returns>
		public static string GetArticleBySkuPath(this ProcessorArgumentCollection args, string elseValue = null) {
			return GetArgumentValue(args, Constants.ProcessorArgumentsKeys.ArticleBySkuPath, elseValue);
		}
		
		private static string GetArgumentValue(ProcessorArgumentCollection args, string argumentName, string elseValue = null) {
			string value;
			if (args != null && args.TryGetValue(argumentName, out value)) return value;
			return elseValue;
		}
	}
}
