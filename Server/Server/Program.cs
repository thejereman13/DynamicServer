using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Server
{
    class Program
    {
        static bool executeLogic = true;
        public static string modulePath = "../../../Modules/";
        static Thread loop;
        delegate void voidCall();
        static event voidCall LoopEvent;
        public static List<object> classes = new List<object>();

        static void Main(string[] args){
            LoadModules();
            loop = new Thread(Loop);
            loop.Start();
            while (executeLogic) {
                string s = Console.ReadLine();
                switch (s) { 
                    case "exit":
                    case "Exit":
                        executeLogic = false;
                        break;
                    case "reload":
                    case "Reload":
                        LoadModules();
                        break;
                }

            }
        }

        static void LoadModules() {
            if (LoopEvent != null)
                foreach (Delegate v in LoopEvent.GetInvocationList())
                {
                    LoopEvent -= (voidCall)v;
                }
            string[] modules = Directory.GetFiles(modulePath);
            foreach (string s in modules)
            {
                try
                {
                    if (!s.Substring(s.Length - 4).Equals(".dll"))
                        throw new FileLoadException();
                    string name = s.Replace(modulePath, "").Replace(".dll", "");
                    Assembly a = Assembly.LoadFrom(s);
                    Type t = a.GetType(a.GetName().Name + "." + "Main");
                    object c = Activator.CreateInstance(t);
                    classes.Add(c);
                    try
                    {
                        if (t.GetMethod("LogicHook") != null)
                        {
                            LoopEvent += (voidCall)Delegate.CreateDelegate(typeof(voidCall), t.GetMethod("LogicHook"));
                        }
                    }
                    catch (Exception g)
                    {
                        Console.WriteLine(g.StackTrace);
                    }
                    Console.WriteLine(s.Replace(modulePath, "") + " loaded");
                }
                catch (BadImageFormatException e)
                {
                    Console.WriteLine(s.Replace(modulePath, "") + " failed to load");
                }
                catch (Exception p) { }
            }
        }


        static void Loop() {
            long updateTime = (long)(1000 / 2.0);
            Stopwatch s = Stopwatch.StartNew();
            while (executeLogic) {
                s.Reset();
                if (LoopEvent != null)
                    LoopEvent.Invoke();
                Thread.Sleep((int)(updateTime - s.ElapsedMilliseconds));
            }
        }
    }
}
