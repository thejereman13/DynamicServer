using ProtoBuf;
using System;

namespace DynamicServer {
	[ProtoContract]
	public class UDPFrame {
		[ProtoMember(1)]
		public string command;
		[ProtoMember(2)]
		public string[] data;
		[ProtoMember(3)]
		public string clientID;
		[ProtoMember(4)]
		public int packetID; 
	}
}
