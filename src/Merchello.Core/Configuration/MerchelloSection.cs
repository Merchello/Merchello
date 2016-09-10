namespace Merchello.Core.Configuration
{
    using System.Configuration;

    /// <summary>
    /// Represents a Merchello section within the configuration file.
    /// </summary>
    /// <remarks>
    /// <para>The requirement for these sections is to be read-only.</para>
    /// <para>However for unit tests purposes it is internally possible to override some values, and
    /// then calling <c>>ResetSection</c> should cancel these changes and bring the section back to
    /// what it was originally.</para>
    /// <para>The <c>MerchelloSettings.For{T}</c> method will return a section, either one that
    /// is in the configuration file, or a section that was created with default values.</para>
    /// </remarks>
    public interface IMerchelloSection
    {
    }

    /// <inheritdoc/>
    public abstract class MerchelloSection : ConfigurationSection, IMerchelloSection
    {
        /// <summary>
        /// Gets a value indicating whether the section actually is in the configuration file.
        /// </summary>
        protected bool IsPresent
        {
            get
            {
                return this.ElementInformation.IsPresent;
            }
        }
    }
}