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
		public static PacketBuilder PC_ItemDropped(object[] args)
		{
			var x = (ushort)args[0];
			var y = (ushort)args[1];
			var id = (ushort)args[2];
			var uid = (uint)args[3];
			var entity = (uint)args[4];

			var p = new PacketBuilder();

			p.New(0xCC);
			{
				p += (byte)1;		// Item count?  Doubtful...
				p += (int)uid;		// Unique item ID.  Used when client requests to loot the item.  Contains map ID on official.
				p += 0;
				p += (int)entity;	// The entity ID.  Not quite sure how this works for mobs, yet.
				p += (short)id;
				p += (short)x;
				p += (short)y;
				p += (short)0x713B;
				p += (byte)2;

				return p;
			}
		}
	}
}