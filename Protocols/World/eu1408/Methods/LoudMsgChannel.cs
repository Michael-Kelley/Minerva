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

#region Includes

using System.Collections.Generic;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void SayInShout(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var name = client.Metadata["name"] as string;

			int size = packet.ReadByte() - 5;
			packet.Skip(2);
			var type = packet.ReadByte();
			var x = packet.ReadByte();
			var y = packet.ReadByte();
			var msg = packet.ReadString(size);

			if (msg == ".quit")
			{
				client.Disconnect();
				return;
			}

			var id = (uint)client.Metadata["cid"];
			var clients = client.Metadata["clients"] as Dictionary<ulong, ClientHandler>;

			foreach (var c in clients.Values)
			{
				builder.New(0x18A);
				{
					builder += (int)id;
					builder += (byte)name.Length;
					builder += (byte)(msg.Length + 6);
					builder += name;
					builder += (byte)0xFE;
					builder += (byte)0xFE;
					builder += type;
					builder += x;
					builder += y;
					builder += msg;
					builder += (byte)0;

					//b = builder.Data;
				}

				c.Send(builder, "LoudMsgChannel");
			}

			events.SaidChannel("world.700.LoudMsgChannel", (int)id, name, msg);
		}
	}
}