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
using System.Net.Sockets;
using System.Net;

namespace Server
{
    class Program
    {
        public static bool executeLogic = false;
		public static bool runServer = true;
        public static string modulePath = "../../../Modules/";
        static Thread loop;
		static Thread packet;
        static event Action LoopEvent;
		delegate void packetCall(UDPFrame message);
		static event packetCall packetEvent;

        public static List<object> classes = new List<object>();

        static void Main(string[] args){

			LoadModules();
            loop = new Thread(Loop);
			loop.IsBackground = true;
            loop.Start();
			packet = new Thread(ClientManagement.retrievalLoop);
			packet.IsBackground = true;
			packet.Start();
            while (runServer) {
				string s = Console.ReadLine();
				Console.WriteLine(Terminal.ExecuteCommand(s));

            }
			executeLogic = false;
        }

        static void LoadModules() {
            if (LoopEvent != null)
                foreach (Delegate v in LoopEvent.GetInvocationList())
                {
                    LoopEvent -= (Action)v;
                }
			if(packetEvent != null)
				foreach(Delegate v in packetEvent.GetInvocationList()) {
					packetEvent -= (packetCall)v;
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
                            LoopEvent += (Action)Delegate.CreateDelegate(typeof(Action), t.GetMethod("LogicHook"));
                        }
						if(t.GetMethod("PacketHook") != null) {
							packetEvent += (packetCall)Delegate.CreateDelegate(typeof(packetCall), t.GetMethod("PacketHook"));
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


        public static void Loop() {
            long updateTime = (long)(1000 / 2.0);
            Stopwatch s = Stopwatch.StartNew();
            while (executeLogic) {
                s.Reset();
                if (LoopEvent != null)
                    LoopEvent.Invoke();
                Thread.Sleep((int)(updateTime - s.ElapsedMilliseconds));
            }
        }

		public static void PacketCall(UDPFrame data) {
			packetEvent.Invoke(data);
		}
    }
}
