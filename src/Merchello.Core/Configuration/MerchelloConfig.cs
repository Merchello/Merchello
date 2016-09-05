namespace Merchello.Core.Configuration
{
    using System;

    /// <summary>
    /// Provides access to configurations in the Merchello.config file.
    /// </summary>
    public class MerchelloConfig
    {
        /// <summary>
        /// Configuration singleton.
        /// </summary>
        private static readonly Lazy<MerchelloConfig> config = new Lazy<MerchelloConfig>(() => new MerchelloConfig());

        /// <summary>
        /// Gets the <see cref="MerchelloConfig"/>.
        /// </summary>
        public static MerchelloConfig For
        {
            get
            {
                return config.Value;
            }
        }
    }
}