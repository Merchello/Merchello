namespace Merchello.Core.ValueConverters.ValueCorrections
{
    using System;

    /// <summary>
    /// Overrides the stored detached value for the Decimal property editor so that it 
    /// renders properly in the value converter.
    /// </summary>
    [DetachedValueCorrection("Umbraco.Decimal")]
    public class DecimalValueCorrection : DetachedValueCorrectionBase
    {
        public override object ApplyCorrection(object value)
        {
            if (value != null)
            {
                return Convert.ToDecimal(value);
            }
            return 0;
        }
    }
}
