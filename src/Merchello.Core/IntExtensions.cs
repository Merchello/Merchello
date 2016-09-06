namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Extension methods for integer types.
    /// </summary>
    /// UMBRACO_SRC
    internal static class IntExtensions
	{
		/// <summary>
		/// Does something 'x' amount of times
		/// </summary>
		/// <param name="n">
		/// The number of times to iterate.
		/// </param>
		/// <param name="action">
		/// The 'something' to do.
		/// </param>
		public static void Times(this int n, Action<int> action)
		{
			for (int i = 0; i < n; i++)
			{
				action(i);
			}
		}

        /// <summary>
        /// Creates a GUID based on an integer value
        /// </summary>
        /// <param name="value">
        /// The integer value to be represented as a GUID.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static Guid ToGuid(this int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
	}
}