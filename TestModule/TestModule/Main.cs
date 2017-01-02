using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModule
{
    class Main
    {
        public Main()
        {
            Console.WriteLine("Added TestModule");
        }

        public static void LogicHook()
        {
            Console.WriteLine("Invoked");
        }
    }
}
