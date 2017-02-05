using DynamicServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModule
{
    class Main : IModule
    {
        public Main()
        {
            Console.WriteLine("Added TestModule");
        }

        public static void LogicHook()
        {
            Console.WriteLine("Invoked");
        }
		public void AddHooks() {

		}
    }
}
