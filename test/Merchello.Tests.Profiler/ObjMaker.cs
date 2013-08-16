using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Tests.Profiler
{
    class ObjMaker
    {
        /// <summary>
        /// Generates the specified number of POCO objects
        /// </summary>
        /// <param name="T">Specified POCO class type</param>
        public static object Generate(Type T)
        {
            // identify constructor parameters for the type
            ConstructorInfo[] cInfo = T.GetConstructors();

            // get parameters from the first constructor
            Type[] newParams;

            ParameterInfo[] cParams = cInfo[0].GetParameters();
            foreach (ParameterInfo cParam in cParams)
            {
                switch (cParam.ParameterType.ToString())
                {
                    case "System.Decimal":
                        break;
                    case "System.DateTime":
                        break;
                    case "System.String":
                        break;


                }
                if (cParam.ParameterType == typeof (string))
                {
                }
            }


            return null;
        }
    }
}
