using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServer {
	public static class ClientManagement {

		private const int port = 10069;
		private static UdpClient serv;
		public static Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
		static Encryption encrypt;

		static ClientManagement() {
			serv = new UdpClient(port);
			encrypt = new Encryption();
		}

		public static void newClient(IPEndPoint groupEP) {
			//Console.WriteLine(groupEP.Address.ToString());
			string uid = Guid.NewGuid().ToString();
			clients.Add(uid, groupEP);
			sendPacket(groupEP, "~", new string[] { uid });
		}

		public static void sendPacket(IPEndPoint ep, string s, string[] dat) {
			byte[] sendbuffer = UDPtoBytes(new UDPFrame { command = s, data = dat });
			serv.Send(sendbuffer, sendbuffer.Length, ep);
		}

		public static void sendUDP(string client, UDPFrame frame) {
			byte[] sendBuffer = UDPtoBytes(frame);
			serv.Send(sendBuffer, sendBuffer.Length, clients[client]);
		}

		public static void sendAll(string s, string[] data) {
			foreach(KeyValuePair<string, IPEndPoint> o in clients) {
				sendPacket(o.Value, s, data);
			}
		}

		public static void retrievalLoop() {
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
			while(Program.runServer) {
				byte[] data = serv.Receive(ref groupEP);
				UDPFrame output = BytestoUDP(data);
				Console.WriteLine(output.command);
				if(output.command.Equals("~add")) {
					newClient(groupEP);
					Console.WriteLine("New Client: " + groupEP.Port.ToString());
				} else {
					checkClient(groupEP, output);
				}
				checkCommands(groupEP, output);
			}
		}

		private static void checkCommands(IPEndPoint groupEP, UDPFrame output) {
			switch(output.command) {
				case "heartbeat":
					break;
				case "~remove":
					if(clients.ContainsKey(output.clientID))
						clients.Remove(output.clientID);
					if (Program.clientPassthrough != null)
						foreach(Program.sendClient c in Program.clientPassthrough.Values) {
							c.Invoke(output.clientID, false);
						}
					break;
				case "console":
					string o = "";
					foreach(string s in output.data) {
						o += (s + " ");
					}
					sendPacket(groupEP, "commandResponse", new string[] { Terminal.ExecuteClientCommand(output.clientID, o) });
					break;
				default:
					Program.PacketCall(output);
					break;
			}
		}

		private static void checkClient(IPEndPoint p, UDPFrame output) {
			if(!clients.ContainsKey(output.clientID))
				return;
			if(clients[output.clientID] == p)
				return;
			clients[output.clientID] = p;
		}

		private static byte[] UDPtoBytes(UDPFrame frame) {
			using(MemoryStream ms = new MemoryStream()) {
				Serializer.Serialize(ms, frame);
				return encrypt.Encrypt(ms.ToArray());
			}
		}
		private static UDPFrame BytestoUDP(byte[] data) {
			var dat = encrypt.Decrypt(data);
			using(MemoryStream ms = new MemoryStream(dat)) {
				return Serializer.Deserialize<UDPFrame>(ms);
			}
		}

	}
}
