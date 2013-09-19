using System;
using System.Linq;

namespace Merchello.Tests.Base.DataMakers
{
    public abstract class MockDataMakerBase
    {
        protected static Random NoWhammyStop = new Random();

        protected static string SelectRandomString(string[] values)
        {

            var index = NoWhammyStop.Next(values.Count());
            return values.ToArray()[index];
        }

    }
}
