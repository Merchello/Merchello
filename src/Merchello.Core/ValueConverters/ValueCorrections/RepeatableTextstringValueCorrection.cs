namespace Merchello.Core.ValueConverters.ValueCorrections
{
    using System;
    using System.Linq;

    /// <summary>
    /// Overrides the stored detached value for the multiple text string property editor so that it 
    /// renders properly in the value converter.
    /// </summary>
    [DetachedValueCorrection("Umbraco.MultipleTextstring")]
    internal class RepeatableTextstringValueCorrection : DetachedValueCorrectionBase
    {
        /// <summary>
        /// Overrides the stored detached value to correct serialization issues with the required Environment NewLine.
        /// </summary>
        /// <param name="value">
        /// The value stored by Merchello.
        /// </param>
        /// <returns>
        /// The corrected value <see cref="object"/>.
        /// </returns>
        public override object ApplyCorrection(object value)
        {
            if (!EnsureValueNeedsCorrection(value)) return value;

            // split the value into multiple lines
            var lines = value.ToString().Split(new[] { "\r\n" }, StringSplitOptions.None);

            // join the lines as XML values
            return string.Join(
                string.Empty,
                lines.Select(x => string.Format("<value>{0}</value>", x)));
        }

        /// <summary>
        /// Ensure the value needs to be corrected.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool EnsureValueNeedsCorrection(object value)
        {
            var sourceString = value.ToString().Trim();
            return sourceString.IndexOf("<value>", StringComparison.Ordinal) != 0;
        }
    }
}