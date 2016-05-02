namespace Merchello.Core.ValueConverters.ValueOverrides
{
    using System;
    using System.Linq;

    /// <summary>
    /// Overrides the stored detached value for the multiple text string property editor so that it 
    /// renders properly in the value converter.
    /// </summary>
    [DetachedValueOverrider("Umbraco.MultipleTextstring")]
    internal class RepeatableTextstringValueOverrider : DetachedValueOverriderBase
    {
        /// <summary>
        /// Overrides the stored detached value to correct serialization issues with the required Environment NewLine.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <remarks>
        /// We need to watch 
        /// </remarks>
        public override object Override(object value)
        {
            // split the value into multiple lines
            var lines = value.ToString().Split(new[] { "\r\n" }, StringSplitOptions.None);

            // join the lines as XML values
            return string.Join(
                string.Empty,
                lines.Select(x => string.Format("<value>{0}</value>", x)));
        }
    }
}