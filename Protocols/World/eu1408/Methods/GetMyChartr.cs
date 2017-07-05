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
using System.Collections.Generic;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void GetCharacters(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var proxy = client.Metadata["proxy"] as IChannelContracts;
			client.AccountID = proxy.GetUser((ulong)client.Metadata["magic"]);

			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];

			// Get characters from dbo.characters
			var characters = database.GetCharacters(server, client.AccountID);

			builder.New(0x85);
			{
				builder += 1;
				builder += 0;
				builder += 0;
				builder += (byte)0;
				builder += (byte)1;

				for (int i = 0; i < characters.Count; i++)
				{
					// Now, let's get the character's inventory
					//var eq = database.GetEquipment(server, (int)characters[i].Item1);
					var eq = characters[i].Item2;

					ushort head = (eq[0] != null) ?
								  (ushort)(BitConverter.ToUInt16(eq[0], 0) + (eq[0][0x02] * 0x2000)) :
								  (ushort)0;

					ushort body = (eq[1] != null) ?
								  (ushort)(BitConverter.ToUInt16(eq[1], 0) + (eq[1][0x02] * 0x2000)) :
								  (ushort)0;

					ushort hands = (eq[2] != null) ?
								   (ushort)(BitConverter.ToUInt16(eq[2], 0) + (eq[2][0x02] * 0x2000)) :
								   (ushort)0;

					ushort feet = (eq[3] != null) ?
								  (ushort)(BitConverter.ToUInt16(eq[3], 0) + (eq[3][0x02] * 0x2000)) :
								  (ushort)0;

					ushort righthand = (eq[4] != null) ?
									   (ushort)(BitConverter.ToUInt16(eq[4], 0) + (eq[4][0x02] * 0x2000)) :
									   (ushort)0;

					ushort lefthand = (eq[5] != null) ?
									  (ushort)(BitConverter.ToUInt16(eq[5], 0) + (eq[5][0x02] * 0x2000)) :
									  (ushort)0;

					ushort back = (eq[6] != null) ?
								  (ushort)(BitConverter.ToUInt16(eq[6], 0) + (eq[6][0x02] * 0x2000)) :
								  (ushort)0;

					var character = characters[i].Item1;
					var id = client.AccountID * 8 + character.Item2;
					var style = (uint)character.Item5;
					style += (uint)(character.Item6 << 8);
					style += (uint)(character.Rest.Item1 << 13);
					style += (uint)(character.Item7 << 17);
					style += (!(bool)character.Rest.Item2) ? 0 : (uint)(1 << 26);

					builder += id;
					builder += (int)style;
					builder += (int)character.Item4;	// level
					builder += (byte)0;
					builder += (byte)1;
					builder += (byte)1;
					builder += (short)0;
					builder += 0x004C4B40;	// some kind of UNIX timestamp?
					builder += 0;
					builder += (byte)0;
					builder += character.Rest.Item3;	// map
					builder += (short)character.Rest.Item4;	// x
					builder += (short)character.Rest.Item5;	// y
					builder += (short)head;
					builder += (short)body;
					builder += (short)hands;
					builder += (short)feet;
					builder += (short)righthand;
					builder += (short)lefthand;
					builder += (short)back;
					builder += (short)0;
					builder += 0;
					builder += 0;
					builder += 0;
					builder += (byte)(character.Item3.Length + 1);
					builder += character.Item3;
				}

				//b = builder.Data;
			}

			client.Send(builder, "GetMyChartr");
		}
	}
}