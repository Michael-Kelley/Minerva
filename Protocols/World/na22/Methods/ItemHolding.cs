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
		public static void GrabItem(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			Log.Message(packet[10].ToString("X2"), Log.DefaultFG);

			var b = new byte[] { 0xE2, 0xB7, 0x07, 0x00, 0x9E, 0x00, 0x01 };	// 1 = Good!

			client.Metadata["item_slot"] = packet[12];

			client.Send(ref b, "ItemHolding");
		}
	}
}