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
		public static void EquipItem(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			/*
			 * ushort header
			 * ushort size
			 * uint padding
			 * ushort opcode
			 * ushort unk
			 * byte slot
			 */

			// ^^ is same for ItemPutting and ItemHolding

			Log.Message(packet[10].ToString("X2"), Log.DefaultFG);

			var equipslot = (EquipmentSlots)packet[12];

			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];
			var id = (uint)client.Metadata["id"];
			var itemslot = (byte)client.Metadata["item_slot"];

			database.EquipItem(server, (int)id, itemslot, equipslot.ToString().ToLower());

			var b = new byte[] { 0xE2, 0xB7, 0x07, 0x00, 0x96, 0x00, 0x01 };	// 1 = Good!

			/*var map = client.Metadata["map"] as IMap;
			map.RemoveClient(client);

			var p = new PacketBuilder(0x07);
			p += (ushort)0x96;	// Opcode of packet that caused error
			p += (ushort)0;		// ?
			p += (ushort)6;		// Error code
			p += (ushort)4066;	// ?

			b = p.Data;*/

			client.Send(ref b, "ItemEquipS0");			

			//client.Disconnect();	// Oops...
		}
	}
}