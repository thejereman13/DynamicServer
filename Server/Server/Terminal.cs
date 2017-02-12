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
		private static Dictionary<string, ClientCommand> clientCommands = new Dictionary<string, ClientCommand>();

		public class ClientCommand {
			public Delegate command;
			public int authLevel;
			/// <summary>
			/// New Client Command with the action delegate d and authorization level a
			/// </summary>
			/// <param name="d"></param>
			/// <param name="a"></param>
			public ClientCommand(Delegate d, int a) {
				command = d;
				authLevel = a;
			}
			/// <summary>
			/// New Client Command with the action delegate d
			/// </summary>
			/// <param name="d"></param>
			public ClientCommand(Delegate d) {
				command = d;
				authLevel = 0;
			}
		}
		static Terminal() {
			resetCommands();
		}

		public static void resetCommands() {
			commands.Clear();
			clientCommands.Clear();

			commands.Add("BROADCASTPACKET", BroadcastPacket);
			commands.Add("ECHO", Echo);
			commands.Add("EXIT", Exit);
			commands.Add("HELP", Help);
			commands.Add("RELOADMODULES", ReloadModules);
			commands.Add("SETLOGIC", SetLogic);
			commands.Add("LISTMODULES", ListModules);
			commands.Add("ADDUSER", addUser);
			commands.Add("REMOVEUSER", removeUser);
			commands.Add("SETPASSWORD", setPassword);
			commands.Add("LISTUSERS", listUsers);
			commands.Add("GETAUTHLEVEL", getAuthLevel);
			commands.Add("SETAUTHLEVEL", setAuthLevel);

			clientCommands.Add("ECHO", new ClientCommand(new consoleCall(Echo)));
			clientCommands.Add("GETUID", new ClientCommand(new clientCall(getUID)));
			clientCommands.Add("HELP", new ClientCommand(new clientCall(ClientHelp)));
			clientCommands.Add("LISTMODULES", new ClientCommand(new consoleCall(ListModules), 1));
			clientCommands.Add("PASSTHROUGH", new ClientCommand(new clientCall(PassThrough), 1));
			clientCommands.Add("SETLOGIC", new ClientCommand(new consoleCall(SetLogic), 3));
			clientCommands.Add("LOGIN", new ClientCommand(new clientCall(login)));
			clientCommands.Add("ADDUSER", new ClientCommand(new clientCall(addUserClient), 2));
			clientCommands.Add("LISTUSERS", new ClientCommand(new consoleCall(listUsers), 2));
			clientCommands.Add("SETPASSWORD", new ClientCommand(new clientCall(setPasswordClient)));
			clientCommands.Add("RELOADMODULES", new ClientCommand(new consoleCall(ReloadModules), 2));
			clientCommands.Add("REMOVEUSER", new ClientCommand(new consoleCall(removeUser), 2));
			clientCommands.Add("GETAUTHLEVEL", new ClientCommand(new clientCall(getAuthLevelClient)));
			clientCommands.Add("SETAUTHLEVEL", new ClientCommand(new consoleCall(setAuthLevel), 3));

		}

		public static void AddCommand(string name, consoleCall com) {
			commands.Add(name, com);
		}
		public static void AddClientCommand(string name, ClientCommand com) {
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
			ClientCommand ret;
			if(!clientCommands.TryGetValue(args[0].ToUpper(), out ret)) {
				return "Command not found";
			} else {
				if(ClientManagement.clients.ContainsKey(client) && ret.authLevel > ClientManagement.clients[client].authLevel)
					return "Invalid Authentication Level";
				args.RemoveAt(0);
				if(ret.command is clientCall)
					return ((clientCall)ret.command)(client, args);
				else if(ret.command is consoleCall)
					return ((consoleCall)ret.command)(args);
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
				return "Usage: setLogic <true/false>";
			}
		}

		private static string Exit(List<string> args) {
			Program.Exit();
			return "";
		}
		private static string Help(List<string> args) {
			string o = "Available Commands: \n";
			List<string> lis = commands.Keys.ToList();
			lis.Sort();
			foreach(string c in lis) {
				o += (c + "\n");
			}
			return o;
		}
		private static string ClientHelp(string user, List<string> args) {
			string o = "Available Commands: \n";
			List<string> lis = clientCommands.Keys.ToList();
			lis.Sort();
			foreach(string c in lis) {
				if (clientCommands[c].authLevel <= ClientManagement.clients[user].authLevel)
					o += (c + "\n");
			}
			return o;
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
				return Program.clientPassthrough[args[0].ToUpper()].Invoke(client, check);
			else
				return "Usage: passThrough <Module Name> <true/false>";
		}

		private static string getUID(string client, List<string> args) {
			return client;
		}

		private static string addUser(List<string> args) {
			if(args.Count > 2) {
				if(Authentication.userExists(args[0]))
					return "User Already Exists";
				int a;
				if(!int.TryParse(args[2], out a))
					return "Invalid Authorization Level";
				Authentication.addUser(args[0], args[1], a);
				return "User Added";
			}
			return "Usage: addUser <UserName> <Password> <AuthLevel>";
		}

		private static string addUserClient(string client, List<string> args) {
			if(args.Count > 2) {
				if(Authentication.userExists(args[0]))
					return "User Already Exists";
				int a;
				if(!int.TryParse(args[2], out a))
					return "Invalid Authorization Level";
				if(a > ClientManagement.clients[client].authLevel)
					return "You are not authorized to add such an authentication level";
				Authentication.addUser(args[0], args[1], a);
				return "User Added";
			}
			return "Usage: addUser <UserName> <Password> <AuthLevel>";
		}

		private static string login(string client, List<string> args) {
			if(args.Count > 1) {
				if(!Authentication.userExists(args[0]))
					return "User Not Found";
				return Authentication.login(client, args[0], args[1]) ? "Login Successful" : "Invalid Password";
			}
			return "Usage: login <UserName> <Password>";
		}

		private static string setPassword(List<string> args) {
			if(args.Count > 1) {
				if(!Authentication.userExists(args[0]))
					return "User Not Found";
				return Authentication.setPass(args[0], args[1]) ? "Password Set" : "Password set failed";
			}
			return "Usage: login <UserName> <Password>";
		}

		private static string setPasswordClient(string client, List<string> args) {
			if(args.Count > 1) {
				if(!Authentication.userExists(args[0]))
					return "User Not Found";
				Authentication.User u = Authentication.getUser(args[0]);
				if(ClientManagement.clients[client].authLevel < 2 || (u.AuthLvl >= ClientManagement.clients[client].authLevel && !u.UserName.Equals(ClientManagement.clients[client].userName)))
					return "You are not authorized to change that account's password";
				return Authentication.setPass(args[0], args[1]) ? "Password Set" : "Password set failed";
			}
			return "Usage: login <UserName> <Password>";
		}

		private static string removeUser(List<string> args) {
			if(args.Count > 0) {
				return Authentication.removeUser(args[0]) ? "User Removed" : "User Does Not Exist";
			}
			return "Usage: removeUser <UserName>";
		}

		private static string listUsers(List<string> args) {
			var us = Authentication.listUsers();
			string s = "Users: \n";
			foreach (string d in us){
				s += d + "\n";
			}
			return s;
		}

		private static string getAuthLevelClient(string client, List<string> args) {
			return Authentication.getUser(ClientManagement.clients[client].userName).AuthLvl.ToString();
		}

		private static string getAuthLevel(List<string> args) {
			if(args.Count > 0)
				if(ClientManagement.clients.ContainsKey(args[0]))
					return Authentication.getUser(ClientManagement.clients[args[0]].userName).AuthLvl.ToString();
				else
					return "User not found";
			return "Usage: getAuth <UserName>";
		}

		private static string setAuthLevel(List<string> args) {
			if(args.Count > 1) {
				if(!Authentication.userExists(args[0]))
					return "User does not exist";
				Authentication.User u = Authentication.getUser(args[0]);
				int a;
				if(!int.TryParse(args[1], out a) && a < 4)
					return "Invalid Authentication Level";
				u.AuthLvl = a;
				Authentication.updateUser(u);
			}
			return "Usage: setAuthLevel <UserName> <Level>";
		}
	}
}
