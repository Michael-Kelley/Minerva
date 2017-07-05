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
		public static void CreateCharacter(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			#region INCOMING
			/*
			short header;
			short size;
			int padding;
			short opcode;
			uint style;
			byte padding;
			byte slot;
			byte name_length;
			string name;
*/
			#endregion

			var style = packet.ReadInt();

			var _class = (byte)(style & 0xFF ^ 8);
			style >>= 8;
			var colour = (byte)((style & 0xFF) >> 5);
			var face = (byte)(style & 0x1F);
			style >>= 8;
			var hair = (byte)((style & 0xFF) >> 1);
			style >>= 8;
			var gender = (style != 0);

			packet.Skip(1);
			var slot = packet.ReadByte();
			var nameLength = packet.ReadByte();
			var name = packet.ReadString(nameLength);

			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];

			int[] result = database.CreateCharacter(server, client.AccountID, slot, name, _class, gender, face, hair, colour);

			builder.New(0x86);
			{
				builder += client.AccountID * 8 + slot;
				builder += (byte)result[0];

				//b = builder.Data;
			}

			client.Send(builder, "NewMyChartr");
		}
	}
}