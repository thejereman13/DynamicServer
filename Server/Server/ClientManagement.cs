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
		public class clientStruct{
			public IPEndPoint endp { get; set; }
			public int authLevel { get; set; } = 0;
			public string userName { get; set; }
		}
		private const int port = 10069;
		private static UdpClient serv;
		/// <summary>
		/// Stores the Client information based on UID assigned to the client program when it connects
		/// </summary>
		public static Dictionary<string, clientStruct> clients = new Dictionary<string, clientStruct>();
		static Encryption encrypt;

		static ClientManagement() {
			serv = new UdpClient(port);
			encrypt = new Encryption();
		}
		/// <summary>
		/// Registers a new client with the client dictionary and sends a confirmation packet to the client with their uid
		/// </summary>
		/// <param name="groupEP"></param>
		public static void newClient(IPEndPoint groupEP) {
			string uid = Guid.NewGuid().ToString();
			clients.Add(uid, new clientStruct { endp = groupEP });
			sendPacket(groupEP, "~", new string[] { uid });
		}
		/// <summary>
		/// Constructs a UDPFrame from the command string and data array and sends it to the IPEndPoint passed
		/// </summary>
		/// <param name="ep"></param>
		/// <param name="s"></param>
		/// <param name="dat"></param>
		public static void sendPacket(IPEndPoint ep, string s, string[] dat) {
			byte[] sendbuffer = UDPtoBytes(new UDPFrame { command = s, data = dat });
			serv.Send(sendbuffer, sendbuffer.Length, ep);
		}
		/// <summary>
		/// Sends a UDPFrame object to the client matching the string
		/// </summary>
		/// <param name="client"></param>
		/// <param name="frame"></param>
		public static void sendUDP(string client, UDPFrame frame) {
			byte[] sendBuffer = UDPtoBytes(frame);
			if (clients.ContainsKey(client))
				serv.Send(sendBuffer, sendBuffer.Length, clients[client].endp);
		}
		/// <summary>
		/// Sends a packet with the passed data to all clients currently connected
		/// </summary>
		/// <param name="s"></param>
		/// <param name="data"></param>
		public static void sendAll(string s, string[] data) {
			foreach(KeyValuePair<string, clientStruct> o in clients) {
				sendPacket(o.Value.endp, s, data);
			}
		}

		//Loops to retrieve packets and run the necessary commands on them
		public static void retrievalLoop() {
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
			while(Program.runServer) {
				byte[] data = serv.Receive(ref groupEP);
				UDPFrame output = BytestoUDP(data);
				if(output.command.Equals("~add")) {
					newClient(groupEP);
					Console.WriteLine("New Client: " + groupEP.Port.ToString());
				} else {
					checkClient(groupEP, output);
				}
				checkCommands(groupEP, output);
			}
		}
		/// <summary>
		/// Switches on the command from the UDPFrame and executes or passes off the commands associated
		/// </summary>
		/// <param name="groupEP"></param>
		/// <param name="output"></param>
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
		//Updates the client registery's value for a client's endpoint if it gets updated while the client is connected
		private static void checkClient(IPEndPoint p, UDPFrame output) {
			if(!clients.ContainsKey(output.clientID))
				return;
			if(clients[output.clientID].endp == p)
				return;
			clients[output.clientID].endp = p;
		}
		/// <summary>
		/// Returns a byte array containing the serialized UDPFrame object
		/// </summary>
		/// <param name="frame"></param>
		/// <returns></returns>
		private static byte[] UDPtoBytes(UDPFrame frame) {
			using(MemoryStream ms = new MemoryStream()) {
				Serializer.Serialize(ms, frame);
				return encrypt.Encrypt(ms.ToArray());
			}
		}
		/// <summary>
		/// Deserializes a byte array into a UDPFrame
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static UDPFrame BytestoUDP(byte[] data) {
			var dat = encrypt.Decrypt(data);
			using(MemoryStream ms = new MemoryStream(dat)) {
				return Serializer.Deserialize<UDPFrame>(ms);
			}
		}

	}
}
