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

namespace Server {
	class ClientManagement {

		private const int port = 10069;
		private static UdpClient serv;
		public static Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();


		static ClientManagement() {
			serv = new UdpClient(port);
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
				} else if(output.command.Equals("~remove")) {
					if (clients.ContainsKey(output.data[0]))
						clients.Remove(output.data[0]);
				} else {
				Program.PacketCall(output);
				}
			}
		}

		private static byte[] UDPtoBytes(UDPFrame frame) {
			using(MemoryStream ms = new MemoryStream()) {
				Serializer.Serialize(ms, frame);
				return ms.ToArray();
			}
		}
		private static UDPFrame BytestoUDP(byte[] data) {
			using(MemoryStream ms = new MemoryStream(data)) {
				return Serializer.Deserialize<UDPFrame>(ms);
			}
		}
	}
}
