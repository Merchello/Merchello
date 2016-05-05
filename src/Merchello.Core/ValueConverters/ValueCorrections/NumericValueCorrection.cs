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
