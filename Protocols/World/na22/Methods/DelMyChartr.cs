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
		public static void DeleteCharacter(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			#region INCOMING
			/*
			short header;
			short size;
			int padding;
			short opcode;
			uint charID;
*/
			#endregion

			var charID = packet.ReadInt();

			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];

			builder.New(0x87);
			{
				builder += database.DeleteCharacter(server, (int)client.AccountID, charID - (int)client.AccountID * 8);

				//b = builder.Data;
			}

			client.Send(builder, "DelMyChartr");
		}
	}
}