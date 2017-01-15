using ProtoBuf;
using System;

namespace DynamicServer {
	[ProtoContract]
	public class UDPFrame {
		[ProtoMember(1)]
		public string command;
		[ProtoMember(2)]
		public string[] data;
	}
}
