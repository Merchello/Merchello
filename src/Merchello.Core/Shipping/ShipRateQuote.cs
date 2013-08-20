using System;

namespace Merchello.Core.Shipping
{
        using System.Collections.Generic;
        using System.ComponentModel;

        public class ShipRateQuote
        {
            private Guid _shipmethodid;
            private decimal _rate;
            private decimal _surcharge;
            private List<string> _warnings;
            private ShipMethod _shipmethod;

            public IList<string> Warnings
            {
                get
                {
                    return this._warnings;
                }
            }

            public string Name
            {
                get { return this.ShipMethod.Name; }
            }

            public decimal Rate
            {
                get { return this._rate; }
                set { this._rate = value; }
            }

            public decimal Surcharge
            {
                get { return this._surcharge; }
                set { this._surcharge = value; }
            }

            public decimal TotalRate
            {
                get { return this.Rate + this.Surcharge; }
            }

            public int ShipMethodId
            {
                get
                {
                    return this._shipmethodid;
                }
                set
                {
                    this._shipmethodid = value;
                    if (this._shipmethod != null && this._shipmethod.Id != value)
                        this._shipmethod = null;
                }
            }

            public ShipMethod ShipMethod
            {
                get
                {
                    if (this._shipmethod == null)
                    {
                        IShipMethodRepository shipmethodRepo = AbleContext.Resolve<IShipMethodRepository>();

                        this._shipmethod = shipmethodRepo.Load(this._shipmethodid);
                    }
                    return this._shipmethod;
                }
                set
                {
                    if (value != null) this.ShipMethodId = value.Id;
                    else this.ShipMethodId = 0;
                    this._shipmethod = value;
                }
            }

            [EditorBrowsable(EditorBrowsableState.Advanced)]
            internal bool ShipMethodLoaded
            {
                get
                {
                    return this._shipmethod != null;
                }
            }

            public void AddWarning(string message)
            {
                if (string.IsNullOrEmpty(message)) return;

                if (this._warnings == null)
                {
                    this._warnings = new List<string>();
                }
                if (!this._warnings.Contains(message))
                {
                    this._warnings.Add(message);
                }
            }

            public void AddWarnings(IList<string> messages)
            {
                if (messages == null || messages.Count == 0) return;

                if (this._warnings == null)
                {
                    _warnings = new List<string>();
                    _warnings.AddRange(messages);
                }
                else
                {
                    foreach (string message in messages)
                    {
                        if (!this._warnings.Contains(message))
                        {
                            this._warnings.Add(message);
                        }
                    }
                }
            }

            public ShipRateQuote Clone()
            {
                ShipRateQuote clonedQuote = new ShipRateQuote();
                clonedQuote.ShipMethodId = this.ShipMethodId;
                clonedQuote.Rate = this.Rate;
                clonedQuote.Surcharge = this.Surcharge;
                if (_warnings != null && _warnings.Count > 0) clonedQuote.AddWarnings(_warnings);
                if (this.ShipMethodLoaded) clonedQuote.ShipMethod = this.ShipMethod;
                return clonedQuote;
            }
        }
    }
}