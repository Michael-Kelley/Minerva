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

using System;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void MoveToLocation(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var startX = packet.ReadShort();
			var startY = packet.ReadShort();
			var endX = packet.ReadShort();
			var endY = packet.ReadShort();
			var unkX = packet.ReadShort();
			var unkY = packet.ReadShort();
			var map = packet.ReadShort();

			var id = (uint)client.Metadata["cid"];
			var timestamp = (uint)client.Metadata["timestamp"];

			builder.New(0xD2);
			{
				builder += (int)id;
				builder += Environment.TickCount - (int)timestamp;
				builder += startX;
				builder += startY;
				builder += endX;
				builder += endY;

				//b = builder.Data;
			}

			client.Metadata["x"] = (ushort)startX;
			client.Metadata["y"] = (ushort)startY;
			client.Metadata["dest_x"] = (ushort)endX;
			client.Metadata["dest_y"] = (ushort)endY;

			var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

			foreach (var c in clients)
			{
				//var b2 = b;
				c.Send(builder, "MoveBegined");
			}
		}
	}
}