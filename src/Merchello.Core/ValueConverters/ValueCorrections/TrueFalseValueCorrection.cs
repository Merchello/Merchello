namespace Merchello.Core.ValueConverters.ValueCorrections
{
    using System;

    /// <summary>
    /// Overrides the stored detached value for the True/False property editor so that it 
    /// renders properly in the value converter.
    /// </summary>
    [DetachedValueCorrection("Umbraco.TrueFalse")]
    internal class TrueFalseValueCorrection : DetachedValueCorrectionBase
    {
        public override object ApplyCorrection(object value)
        {
            if (value != null)
            {
                if (Convert.ToInt32(value) == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
