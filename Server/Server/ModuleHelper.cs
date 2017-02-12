using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServer {
	public static class ModuleHelper {


		public static class Terminal{
			/// <summary>
			/// Adds a command to the server console with the name s and a consolecall delegate c
			/// </summary>
			/// <param name="s"></param>
			/// <param name="c"></param>
			public static void addServerCommand(string s, DynamicServer.Terminal.consoleCall c) {
				DynamicServer.Terminal.AddCommand(s, c);
			}
			/// <summary>
			/// Adds a client command with the name s and ClientCommand object containing the delegate for the method and, optionally, the authorization level for the command
			/// </summary>
			/// <param name="s"></param>
			/// <param name="d"></param>
			public static void addClientCommand(string s, DynamicServer.Terminal.ClientCommand d) {
				DynamicServer.Terminal.AddClientCommand(s, d);
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
