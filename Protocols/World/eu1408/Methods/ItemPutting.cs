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
		public static void PlaceItem(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			var b = new byte[] { 0xE2, 0xB7, 0x07, 0x00, 0x9F, 0x00, 0x01 };	// 1 = Good!

			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];
			var id = (uint)client.Metadata["id"];
			var oldslot = (byte)client.Metadata["item_slot"];
			var newslot = packet[12];

			database.MoveItem(server, (int)id, oldslot, newslot);

			client.Send(ref b, "ItemPutting");
		}
	}
}