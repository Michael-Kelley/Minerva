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
		public static void Connect(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			byte[] b;

			uint key = 0x34BC821B;
			ushort step = 0x1116;

			builder.New(0x65);
			{
				builder += (int)key;
				builder += (byte)0x0F;
				builder += (byte)0xFF;
				builder += 0x00321254;
				builder += (short)step;

				b = builder.Data;
			}

			client.ChangeKey(key, step);

			client.Send(builder, "Connect2Svr");
		}
	}
}