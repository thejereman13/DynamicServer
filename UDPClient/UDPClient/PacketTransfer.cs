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
using System.Timers;
using System.Windows.Forms;

namespace UDPClient {
	class PacketTransfer {
		private static IPAddress server = Dns.GetHostEntry("jereman.com").AddressList[0];
		private const int port = 10069;
		private static bool periodicPacket = false;
		private static bool connected = false;
		private static UdpClient client;
		public delegate void stringpass(string s);
		stringpass ac;
		private static string uid;

		public static Thread listener;

		public PacketTransfer(stringpass a) {
			ac = a;

			System.Timers.Timer t = new System.Timers.Timer(30000);
			t.Elapsed += new ElapsedEventHandler(TimerEvent);
			t.Enabled = true;

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
			connected = true;
		}

		public static void pullIP() {
			byte[] sendbuffer = UDPtoBytes(new UDPFrame {command = "~remove", clientID = uid });
			IPEndPoint ep = new IPEndPoint(server, port);
			client.Send(sendbuffer, sendbuffer.Length, ep);
			connected = false;
			
		}

		public static void sendPacket(string comm, string[] dat) {
			byte[] sendbuffer = UDPtoBytes(new UDPFrame { command = comm, data = dat, clientID = uid});
			IPEndPoint ep = new IPEndPoint(server, port);
			client.Send(sendbuffer, sendbuffer.Length, ep);
			periodicPacket = true;
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

		private static void TimerEvent(object source, ElapsedEventArgs e) {
			if(!connected)
				return;
			if(periodicPacket) {
				periodicPacket = false;
				return;
			} else {
				sendPacket("heartbeat", null);
			}
		}

	}
}
