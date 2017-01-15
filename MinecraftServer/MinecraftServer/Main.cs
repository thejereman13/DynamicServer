using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicServer;
using System.Diagnostics;

namespace MinecraftServer {
	class Main {

		Process cmd = new Process();
		private bool showoutput = true;
		private bool serverRunning = false;
		public Main() {
			cmd.StartInfo.FileName = "cmd.exe";
			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
				if (showoutput)
					Console.WriteLine(e.Data);
			});
			cmd.StartInfo.UseShellExecute = false;
			Terminal.AddCommand("MCSERVER", serverCommands);
		}

		public void AddHooks() {
			Program.reloadEvent += () => {
				if(serverRunning) {
					stopServer();
				}
			};
		}

		public string serverCommands(List<string> args) {
			if(args.Capacity > 1) {
				switch(args[0]) {
					case "start":
						startServer();
						return "Server Started";
					case "stop":
						stopServer();
						return "Server Stopped";
					case "showOutput":
						if(args.Capacity > 2) {
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
						if(args.Capacity > 2) {
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
					default:
						return "No Command Found";
				}
			} else {
				return "Invalid Arguments";
			}
		}

		private void startServer() {
			cmd.Start();
			cmd.StandardInput.WriteLine("cd MCServer && LaunchServer.bat");
			cmd.StandardInput.Flush();
			cmd.BeginOutputReadLine();
			serverRunning = true;
		}
		private void stopServer() {
			cmd.StandardInput.WriteLine("stop");
			cmd.StandardInput.Flush();
			serverRunning = false;
			cmd.CancelOutputRead();
			cmd.Kill();
		}
	}
}
