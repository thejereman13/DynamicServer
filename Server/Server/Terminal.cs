using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServer {
	public class Terminal {
		public delegate String consoleCall(List<string> args);
		private static Dictionary<string, consoleCall> commands = new Dictionary<string, consoleCall>();
		public delegate String clientCall(string client, List<string> args);
		private static Dictionary<string, Delegate> clientCommands = new Dictionary<string, Delegate>();
		static Terminal() {
			commands.Add("BROADCASTPACKET", BroadcastPacket);
			commands.Add("ECHO", Echo);
			commands.Add("EXIT", Exit);
			commands.Add("HELP", Help);
			commands.Add("RELOADMODULES", ReloadModules);
			commands.Add("SENDPACKET", SendPacket);
			commands.Add("SETLOGIC", SetLogic);
			commands.Add("LISTMODULES", ListModules);

			clientCommands.Add("ECHO", new consoleCall(Echo));
			clientCommands.Add("GETUID", new clientCall(getUID));
			clientCommands.Add("HELP", new consoleCall(ClientHelp));
			clientCommands.Add("LISTMODULES", new consoleCall(ListModules));
			clientCommands.Add("PASSTHROUGH", new clientCall(PassThrough));
			clientCommands.Add("SENDPACKET", new consoleCall(SendPacket));
		}

		public static void AddCommand(string name, consoleCall com) {
			commands.Add(name, com);
		}
		public static void AddClientCommand(string name, Delegate com) {
			clientCommands.Add(name, com);
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

		public static string ExecuteClientCommand(string client, string paths) {
			List<string> args = paths.Split(' ').ToList();
			Delegate ret;
			if(!clientCommands.TryGetValue(args[0].ToUpper(), out ret)) {
				return "Command not found";
			} else {
				args.RemoveAt(0);
				if(ret is clientCall)
					return ((clientCall)ret)(client, args);
				else if(ret is consoleCall)
					return ((consoleCall)ret)(args);
				return "Command Error";
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
			Console.WriteLine(args.Capacity);
			if(args.Count > 0 && Boolean.TryParse(args[0], out Program.executeLogic)) {
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
			string o = "Available Commands: \n";
			foreach(string c in commands.Keys) {
				o += (c + "\n");
			}
			return o;
		}
		private static string ClientHelp(List<string> args) {
			string o = "Available Commands: \n";
			foreach(string c in clientCommands.Keys) {
				o += (c + "\n");
			}
			return o;
		}

		private static string SendPacket(List<string> args) {
			if(args.Count > 1) {
				string uid = args[0];
				args.RemoveAt(0);
				if(!ClientManagement.clients.ContainsKey(uid))
					return "UID not found";
				ClientManagement.sendPacket(ClientManagement.clients[uid], "display", args.ToArray());
				return "UDP sent";
			} else {
				return "Usage: sendPacket <UID> <text>";
			}
		}

		private static string BroadcastPacket(List<string> args) {
			ClientManagement.sendAll("display", args.ToArray());
			return "UDP sent to all clients";
		}

		private static string ReloadModules(List<string> args) {
			Program.LoadModules();
			return "Reloaded";
		}

		private static string ListModules(List<string> args) {
			string o = "Modules: \n";
			foreach(object g in Program.classes) {
				o += g.ToString().Replace(".Main", "") + "\n";
			}
			return o;
		}

		private static string PassThrough(string client, List<string> args) {
			bool check;
			if(args.Count > 1 && Boolean.TryParse(args[1], out check) && Program.clientPassthrough.ContainsKey(args[0]))
				return Program.clientPassthrough[args[0]].Invoke(client, check);
			else
				return "Unable to parse command";
		}

		private static string getUID(string client, List<string> args) {
			return client;
		}

	}
}
