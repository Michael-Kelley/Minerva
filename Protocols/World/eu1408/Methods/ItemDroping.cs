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

using DItem = Minerva.Structures.Database.Item;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void DropItem(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];
			var id = (uint)client.Metadata["id"];
			var cid = (uint)client.Metadata["cid"];
			var slot = (byte)client.Metadata["item_slot"];
			var map = client.Metadata["map"] as IMap;
			var x = (ushort)client.Metadata["x"];
			var y = (ushort)client.Metadata["y"];

			var item = database.GetItem(server, (int)id, (int)slot);
			var ditem = (DItem)item.Item1.ToStructure<DItem>();
			
			map.DropItem(x, y, ditem.ID, cid, id);	// TODO: Add item attributes here!
			database.RemoveItem(server, (int)id, slot);

			var b = new byte[] { 0xE2, 0xB7, 0x07, 0x00, 0x9D, 0x00, 0x01 };	// 1 = Good!

			client.Send(ref b, "ItemDroping");
		}
	}
}