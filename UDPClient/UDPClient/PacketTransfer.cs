using DynamicServer;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPClient {
	class PacketTransfer {
		private static IPAddress server = Dns.GetHostEntry("jereman.com").AddressList[0];
		private const int port = 10069;

		private static UdpClient client;
		public delegate void stringpass(string s);
		stringpass ac;
		private static string uid;

		public static Thread listener;

		public PacketTransfer(stringpass a) {
			//server = IPAddress.Parse("10.137.189.38");
			ac = a;
			client = new UdpClient(port);
			listener = new Thread(startListener);
			listener.IsBackground = true;
			listener.Start();
			sendIP();
		}

		private void startListener() {
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);

			while(true) {
				byte[] data = client.Receive(ref groupEP);
				UDPFrame rec = BytestoUDP(data);
				Console.WriteLine(rec.command);
				checkCommands(rec);
			}
		}

		private void sendIP() {
			byte[] sendbuffer = UDPtoBytes(new UDPFrame {command = "~add"});
			IPEndPoint ep = new IPEndPoint(server, port);
			client.Send(sendbuffer, sendbuffer.Length, ep);
		}

		public static void pullIP() {
			byte[] sendbuffer = UDPtoBytes(new UDPFrame {command = "~remove", data = new string[] { uid } });
			IPEndPoint ep = new IPEndPoint(server, port);
			client.Send(sendbuffer, sendbuffer.Length, ep);
			
		}

		public static void sendPacket(string comm, string[] dat) {
			byte[] sendbuffer = UDPtoBytes(new UDPFrame { command = comm, data = dat});
			IPEndPoint ep = new IPEndPoint(server, port);
			client.Send(sendbuffer, sendbuffer.Length, ep);
		}
		
		private void checkCommands(UDPFrame recieved) {
			switch(recieved.command) {
				case "~":
					uid = recieved.data[0];
					ac("Connection established with the Server");
					break;
				case "display":
					ac(recieved.data[0]);
					break;
				case "commandResponse":
					ac(recieved.data[0]);
					break;
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
