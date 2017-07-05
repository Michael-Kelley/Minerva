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
		public static void ChargeInfo(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			/*  Service types
			* 0x00 = Normal
			* 0x01 = Platinum
			* 0x02 = Acadamy Kit
			* 0x03 = Craftsman Kit
			* 0x04 = Adventurer Kit
			*/

			var b = new byte[0x10] {0xE2, 0xB7, 0x10, 0x00, 0x44, 0x01, 0x00, 0x00,
									0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01};

			client.Send(ref b, "ChargeInfo");
		}
	}
}