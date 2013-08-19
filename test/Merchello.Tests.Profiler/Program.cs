using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Persistence;

namespace Merchello.Tests.Profiler
{
    class Program
    {

        public class WorkItem
        {
            public string ClassName;
            public int Iterations;
        }

        public static WorkItem workItem;

        static void Main(string[] args)
        {
            // output introduction
            ShowIntro();

            workItem = new WorkItem();
            
            // parse parameters
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "/class=":
                        workItem.ClassName = arg.Replace("/class=", string.Empty);
                        break;
                    case "/iterations=":
                        workItem.Iterations = Convert.ToInt16(arg.Replace("/iterations=", string.Empty));
                        break;
                }
            }

            // test code
            workItem.ClassName = "Customer";
            workItem.Iterations = 10;
            
            // Build object Profile
            Assembly objAssembly = Assembly.Load("Merchello.Core.Models");
            var obj = objAssembly.GetType(workItem.ClassName);

            // Generate specified number of objects
            for (int x = 0; x <= workItem.Iterations; x++)
            {
                var objBack = ObjMaker.Generate(obj);

                //var newObj = Activator.CreateInstance(obj);
            }



            // finished, pause display
            // *** REMOVE BEFORE FLIGHT ***
            Console.ReadKey();
        }

        static void ShowIntro()
        {
            Assembly execAssembly = Assembly.GetCallingAssembly();

            AssemblyName name = execAssembly.GetName();

            Console.WriteLine(string.Format("{0}{1} {2:0}.{3:0} for .Net ({4}){0}",
                Environment.NewLine,
                name.Name,
                name.Version.Major.ToString(),
                name.Version.Minor.ToString(),
                execAssembly.ImageRuntimeVersion
                ));
        }
    }
}
