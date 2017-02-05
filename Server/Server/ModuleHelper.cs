using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServer {
	public static class ModuleHelper {


		public static class Terminal{
			public static void addServerCommand(string s, DynamicServer.Terminal.consoleCall c) {
				DynamicServer.Terminal.AddCommand(s, c);
			}
			public static void addClientCommand(string s, Delegate d) {
				DynamicServer.Terminal.AddClientCommand(s, d);
			}
			public static void addAdminCommand(string s, Delegate d) {
				DynamicServer.Terminal.AddAdminCommand(s, d);
			}
		}

		public static class Server {
			public static void addReloadEvent(Action a) {
				Program.reloadEvent += a;
			}
			public static void registerPassThrough(string s, Program.sendClient c) {
				Program.clientPassthrough.Add(s, c);
			}
			public static void addPacketEvent(Program.packetCall p) {
				Program.packetEvent += p;
			}
			public static void addExitEvent(Action a) {
				Program.exitEvent += a;
			}
		}

	}
}
