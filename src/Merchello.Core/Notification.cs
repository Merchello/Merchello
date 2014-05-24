using System;
using System.Collections.Generic;

namespace Merchello.Core
{
    public static class Notification
    {
        public static void Trigger(this string alias, object model)
        {
            Trigger(alias, model, new string[]{});
        }

        public static void Trigger(string alias, object model, IEnumerable<string> contacts)
        {
            throw new NotImplementedException();
        }

        public static void Trigger(Guid key, object model)
        {
            Trigger(key, model, new String[]{});
        }

        public static void Trigger(Guid key, object model, IEnumerable<string> contacts)
        {
            throw new NotImplementedException();
        }        

    }

}