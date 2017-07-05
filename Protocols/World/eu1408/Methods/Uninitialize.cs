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
		public static void LeaveWorld(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var unk = packet.ReadByte();
			packet.Skip(1);
			var mapID = packet.ReadByte();

			var clients = client.Metadata["clients"] as Dictionary<ulong, ClientHandler>;
			var id = (uint)client.Metadata["cid"];
			var map = client.Metadata["map"] as IMap;
			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];
			var slot = (int)((uint)client.Metadata["cid"] % 8);
			var x = (ushort)client.Metadata["x"];
			var y = (ushort)client.Metadata["y"];
			database.UpdateCharacterPosition(server, client.AccountID, slot, (byte)map.ID, (byte)x, (byte)y);

			builder.New(0xC9);
			{
				builder += (int)id;
				builder += (byte)12;

				//b = builder.Data;
			}

			client.Metadata["cid"] = (uint)0;

			foreach (var c in clients.Values)
			{
				//var b2 = b;
				c.Send(builder, "Uninitialize");
			}

			map.RemoveClient(client);
		}
	}
}