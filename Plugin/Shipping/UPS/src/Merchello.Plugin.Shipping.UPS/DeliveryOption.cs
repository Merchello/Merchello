using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Merchello.Plugin.Shipping.UPS
{
    [Serializable]
    public class DeliveryOptionCollection : List<DeliveryOption>
    {
        public void Load(IDataReader rdr)
        {
            while (rdr.Read())
            {
                var option = new DeliveryOption();
                option.Load(rdr);
                this.Add(option);
            }
        }
        public void Combine(DeliveryOptionCollection options)
        {
            foreach (var option in options)
            {
                this.Add(option);
            }
        }
    }

    [Serializable]
    public class DeliveryOption : IEnumerable
    {
        public decimal Rate { get; set; }

        public decimal AmountPerUnit { get; set; }

        public string Service { get; set; }
        
        public bool IsAirOnly { get; set; }

        public bool IsGroundOnly { get; set; }

        public bool IsDownloadOnly { get; set; }

        public void Load(IDataReader rdr)
        {
            try { this.Service = rdr["service"].ToString(); }
            catch { };
            try { this.Rate = (decimal)rdr["rate"]; }
            catch { };
            try { this.AmountPerUnit = (decimal)rdr["amountPerUnit"]; }
            catch { };
            try { this.IsAirOnly = (bool)rdr["isAirOnly"]; }
            catch { };
            try { this.IsGroundOnly = (bool)rdr["isGroundOnly"]; }
            catch { };
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
