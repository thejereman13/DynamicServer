using ProtoBuf;
using System;

namespace Server {
	[ProtoContract]
	class UDPFrame {
		[ProtoMember(1)]
		public string command;
		[ProtoMember(2)]
		public string[] data;
	}
}
