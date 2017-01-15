using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
	class Terminal {
		private delegate String consoleCall(List<string> args);
		private static Dictionary<string, consoleCall> commands = new Dictionary<string, consoleCall>();

		static Terminal() {
			commands.Add("ECHO", Echo);
			commands.Add("SETLOGIC", SetLogic);
			commands.Add("EXIT", Exit);
			commands.Add("SENDPACKET", SendPacket);
			commands.Add("BROADCASTPACKET", BroadcastPacket);
			commands.Add("HELP", Help);
		}

		public static string ExecuteCommand(string paths) {
			List<string> args = paths.Split(' ').ToList();
			consoleCall ret;
			if(!commands.TryGetValue(args[0].ToUpper(), out ret)) {
				return "Command not found";
			}else {
				args.RemoveAt(0);
				return ret(args);
			}
		}

		private static string Echo(List<string> args) {
			string output = "";
			foreach (string s in args){
				output += (s + " ");
			}
			return output;
		}

		private static string SetLogic(List<string> args) {
			if(args.Capacity > 0 && Boolean.TryParse(args[0], out Program.executeLogic)) {
				if (Program.executeLogic)
					Program.Loop();
				return "Logic set to " + Program.executeLogic;
			} else {
				return "Unable to parse command";
			}
		}

		private static string Exit(List<string> args) {
			Program.runServer = false;
			return "";
		}
		private static string Help(List<string> args) {
			Console.WriteLine("Available Commands: \n");
			foreach(KeyValuePair<string, consoleCall> c in commands) {
				Console.WriteLine(c.Key);
			}
			return "";
		}

		private static string SendPacket(List<string> args) {
			ClientManagement.sendPacket(ClientManagement.clients.Last().Value, "display", args.ToArray());
			return "UDP sent";
		}

		private static string BroadcastPacket(List<string> args) {
			ClientManagement.sendAll("display", args.ToArray());
			return "UDP sent to all clients";
		}

	}
}
