using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicServer;
using System.Diagnostics;
using System.Net;

namespace MinecraftServer {
	class Main {

		Process cmd = new Process();
		private bool showoutput = true;
		private bool serverRunning = false;
		private List<string> clients = new List<string>();
		public Main() {
			cmd.StartInfo.FileName = "cmd.exe";
			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
				if (showoutput)
					Console.WriteLine(e.Data);
				if(clients.Count > 0)
					foreach(string ep in clients) {
						if (ClientManagement.clients.ContainsKey(ep))
							ClientManagement.sendPacket(ClientManagement.clients[ep], "commandResponse" , new string[] { e.Data });
					}
			});
			cmd.StartInfo.UseShellExecute = false;
			Terminal.AddCommand("MCSERVER", serverCommands);
			Terminal.AddClientCommand("MCSERVER", new Terminal.consoleCall(serverCommands));
		}

		public void AddHooks() {
			Program.reloadEvent += () => {
				if(serverRunning) {
					stopServer();
				}
			};
			Program.clientPassthrough.Add("MinecraftServer", recieveClient);
			
		}

		public string serverCommands(List<string> args) {
			if(args.Count > 0) {
				switch(args[0]) {
					case "start":
						startServer();
						return "Server Started";
					case "stop":
						stopServer();
						return "Server Stopped";
					case "showOutput":
						if(args.Count > 1) {
							if(args[1].Equals("true"))
								showoutput = true;
							else if(args[1].Equals("false"))
								showoutput = false;
							else
								return "Invalid Arguments";
							return "Output Set";
						}
						return "Invalid Arguments";
					case "sendCommand":
						if(args.Count > 1) {
							string output = "";
							args.RemoveAt(0);
							foreach(string s in args) {
								output += (s + " ");
							}
							cmd.StandardInput.WriteLine(output);
							cmd.StandardInput.Flush();
							return "";
						} else {
							return "Invalid Arguments";
						}
					case "help":
						return "start \n stop \n showOutput [true/false] \n sendCommand [args] \n";
					case "status":
						return (serverRunning) ? ("Server Running") : ("Server Offline");
					default:
						return "No Command Found";
				}
			} else {
				return "Invalid Arguments";
			}
		}

		private void startServer() {
			if(serverRunning)
				return;
			cmd.Start();
			cmd.StandardInput.WriteLine("cd MCServer && LaunchServer.bat");
			cmd.StandardInput.Flush();
			cmd.BeginOutputReadLine();
			serverRunning = true;
		}

		private void stopServer() {
			if(!serverRunning)
				return;
			cmd.StandardInput.WriteLine("stop");
			cmd.StandardInput.Flush();
			serverRunning = false;
			cmd.CancelOutputRead();
			cmd.Kill();
		}

		private string recieveClient(string client, bool t) {
			if(t) {
				if(!clients.Contains(client)) {
					clients.Add(client);
					return "Client Added";
				}
			} else {
				if(clients.Contains(client)) {
					clients.Remove(client);
					return "Client Removed";
				}
			}
			return "Error";
		}
	}
}
