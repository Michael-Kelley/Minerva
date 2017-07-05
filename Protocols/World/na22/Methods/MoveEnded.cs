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
		public static void ArrivedAtLocation(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var x = packet.ReadShort();
			var y = packet.ReadShort();

			var id = (uint)client.Metadata["cid"];

			builder.New(0xD3);
			{
				builder += (int)id;
				builder += x;
				builder += y;

				client.Metadata["x"] = (ushort)x;
				client.Metadata["y"] = (ushort)y;
				client.Metadata["dest_x"] = (ushort)x;
				client.Metadata["dest_y"] = (ushort)y;

				//b = builder.Data;
			}

			//var clients = client.Metadata["clients"] as Dictionary<ulong, ClientHandler>;
			var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

			foreach (var c in clients)
			{
				//var b2 = b;
				c.Send(builder, "MoveEnded");
			}
		}
	}
}