using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicServer;
using System.Diagnostics;
using System.Net;

namespace MinecraftServer {
	class Main : IModule{
		private int restartcount = 0;
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
							ClientManagement.sendPacket(ClientManagement.clients[ep].endp, "commandResponse" , new string[] { e.Data });
					}
			});
			cmd.Exited += Cmd_Exited;
			cmd.StartInfo.UseShellExecute = false;
			ModuleHelper.Terminal.addServerCommand("MCSERVER", serverCommands);
			ModuleHelper.Terminal.addClientCommand("MCSERVER", new Terminal.ClientCommand(new Terminal.consoleCall(serverCommands), 1));
		}

		private void Cmd_Exited(object sender, EventArgs e) {
			if(serverRunning) {
				stopServer();
				Console.WriteLine("Minecraft Server Crash Detected");
				if(restartcount < 5) {
					startServer();
					restartcount++;
				}
			}

			throw new NotImplementedException();
		}

		public void AddHooks() {
			ModuleHelper.Server.addReloadEvent(() => {
				if(serverRunning) {
					stopServer();
				}
			});
			ModuleHelper.Server.addExitEvent(() => {
				if(serverRunning) {
					stopServer();
				}
			});
			ModuleHelper.Server.registerPassThrough("MINECRAFTSERVER", recieveClient);
			
		}

		public string serverCommands(List<string> args) {
			if(args.Count > 0) {
				switch(args[0].ToUpper()) {
					case "START":
						startServer();
						return "Server Started";
					case "STOP":
						stopServer();
						return "Server Stopped";
					case "SHOWOUTPUT":
						if(args.Count > 1) {
							bool b;
							if (!bool.TryParse(args[1], out b))
								return "Invalid Boolean Value";
							showoutput = b;
							return "Output Set";
						}
						return "Usage: showOutput <true/false>";
					case "SENDCOMMAND":
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
							return "Usage: sendCommand <command args>";
						}
					case "HELP":
						return "start \n stop \n showOutput \n sendCommand \n";
					case "STATUS":
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
