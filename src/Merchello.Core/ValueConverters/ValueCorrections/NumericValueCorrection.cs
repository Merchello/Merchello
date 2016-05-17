namespace Merchello.Core.ValueConverters.ValueCorrections
{
    using System;

    /// <summary>
    /// Overrides the stored detached value for the numeric property editor so that it 
    /// renders properly in the value converter.
    /// </summary>
    [DetachedValueCorrection("Umbraco.Integer")]
    public class NumericValueCorrection : DetachedValueCorrectionBase
    {
        /// <summary>
        /// Overrides the stored detached value to correct JSON serialized object value.
        /// </summary>
        /// <param name="value">
        /// The value stored by Merchello.
        /// </param>
        /// <returns>
        /// The corrected value <see cref="object"/>.
        /// </returns>
        public override object ApplyCorrection(object value)
        {
            if (value != null)
            {
                return Convert.ToInt32(value);
            }
            return 0;
        }
    }
}
