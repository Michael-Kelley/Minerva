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
		public static PacketBuilder PC_UpdateStats(object[] args)
		{
			var client = (ClientHandler)args[0];
			var type = (byte)args[1];
			var diff = (ushort)args[2];

			var hp = (ushort)client.Metadata["hp"];
			var mp = (ushort)client.Metadata["mp"];

			var p = new PacketBuilder();

			p.New(0x11F);
			{
				p += type;	// Sub Opcode (3 = hp, 4 = mp)

				if (type == 3)
				{
					p += (short)diff;	// Damaged HP
					p += (short)hp;		// Current HP
				}
				else if (type == 4)
					p += (int)mp;	// Current MP

				p += (short)0;

				return p;
			}
		}
	}
}