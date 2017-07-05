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
		public static void ChangeStyle(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			byte[] b;

			b = new byte[0x07] { 0xE2, 0xB7, 0x07, 0x00, 0x42, 0x01, 0x01 };

			client.Send(ref b, "ChangeStyle");

			var cid = (uint)client.Metadata["cid"];
			var style = (uint)client.Metadata["style"];

			builder.New(0x143);
			{
				builder += (int)cid;
				builder += (int)style;
				builder += 0;
				builder += 0;
				builder += (byte)0;

				//b = builder.Data;
			}

			client.Send(builder, "NewStyle?");
		}
	}
}