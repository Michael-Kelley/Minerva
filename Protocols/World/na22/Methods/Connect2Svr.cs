/*
	Copyright © 2010, The Divinity Project
	All rights reserved.
	http://board.thedivinityproject.com
	http://www.ragezone.com


	This file is part of Minerva.

	Minerva is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	any later version.

	Minerva is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Minerva.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void Connect(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var server = packet.ReadByte();
			var channel = packet.ReadByte();

			uint key = 0x34BC821B;
			ushort step = 0x05D5;

			var timestamp = (uint)client.Metadata["timestamp"];
			var count = (ushort)client.Metadata["count"];

			builder.New(0x8C);
			{
				builder += (int)key;
				builder += (int)timestamp;
				builder += (short)count;
				builder += (short)step;

				//b = builder.Data;
			}

			client.Send(builder, "Connect2Svr");

			client.ChangeKey(key, step);
		}
	}
}