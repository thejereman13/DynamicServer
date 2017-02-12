using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicServer;

namespace ChatRoom {
	class Main : IModule{

		private Dictionary<string, List<string>> rooms = new Dictionary<string, List<string>>();

		public Main() {
			ModuleHelper.Terminal.addClientCommand("CHATROOM", new Terminal.ClientCommand(new Terminal.clientCall(serverCommands)));
			ModuleHelper.Terminal.addClientCommand("SAY", new Terminal.ClientCommand(new Terminal.clientCall(chat)));
		}

		public void AddHooks() {
			Program.clientPassthrough.Add("CHATROOM", recieveClient);
		}

		public string serverCommands(string uid, List<string> args) {
			if(args.Count > 0) {
				switch(args[0]) {
					case "createRoom":
						if(args.Count > 1) {
							if(!rooms.ContainsKey(args[1])) {
								rooms.Add(args[1], new List<string>());
								return "Room Created";
							}
							return "Room Already Exists";
						}
						return "Invalid Arguments";
					case "removeRoom":
						if(args.Count > 1) {
							if(rooms.ContainsKey(args[1])) {
								rooms.Remove(args[1]);
								return "Room Removed";
							}
							return "Room Does Not Exist";
						}
						return "Invalid Arguments";
					case "join":
						if(args.Count > 1) {
							if(rooms.ContainsKey(args[1])) {
								rooms[args[1]].Add(uid);
								return "Added to " + args[1];
							}
							return "Room Does Not Exist";
						}
						return "Invalid Arguments";
					case "leave":
						if(args.Count > 1) {
							if(rooms.ContainsKey(args[1])) {
								rooms[args[1]].Remove(uid);
								return "Removed From " + args[1];
							}
							return "Room Does Not Exist";
						}
						return "Invalid Arguments";
					case "listRooms":
						string output = "Rooms: \n";
						foreach(string s in rooms.Keys) {
							output += s + "\n";
						}
						return output;
					case "help":
						return "Available Commands: \n createRoom \n join \n leave \n listRooms \n removeRoom \n";

				}
			}
			return "Invalid Arguments";
		}

		public string chat(string uid, List<string> args) {
			if(args.Count > 0) {
				foreach(KeyValuePair<string, List<string>> d in rooms) {
					if(d.Value.Contains(uid)) {
						string output = d.Key + ": ";
						foreach(string s in args) {
							output += (s + " ");
						}
						foreach(string s in d.Value) {
							ClientManagement.sendPacket(ClientManagement.clients[s].endp, "commandResponse", new string[] { output });
						}
					}
				}
			}
			return null;
		}

		private string recieveClient(string client, bool t) {
			if(!t) {
				foreach(List<string> d in rooms.Values) {
					if(d.Contains(client))
						d.Remove(client);
				}
			}
			return "";
		}
	}
}
