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

using CItem = Minerva.Structures.Client.Item;
using DItem = Minerva.Structures.Database.Item;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void LootItem(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var uid = packet.ReadInt();
			var unk = packet.ReadShort();
			var itemID = packet.ReadShort();
			var slot = packet.ReadShort();

			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];
			var id = (uint)client.Metadata["id"];
			var map = client.Metadata["map"] as IMap;

			var item = map.LootItem(client, (uint)uid);
			var ditem = (DItem)item.ToStructure<DItem>();
			CItem citem = ditem.ToClient(slot);
			database.AddItem(server, (int)id, (int)slot, item, 0);

			builder.New(0x99);
			{
				builder += (byte)96;	// ?
				builder += citem.ToByteArray();

				//b = builder.Data;
			}

			client.Send(builder, "ItemLooting");
		}
	}
}