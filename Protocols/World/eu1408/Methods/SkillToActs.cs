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

using System.Collections.Generic;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void PerformAction(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var charID = packet.ReadInt();
			var action = packet.ReadInt();

			builder.New(0x137);
			{
				builder += charID;
				builder += charID;
				builder += action;

				//b = builder.Data;
			}

			var clients = client.Metadata["clients"] as Dictionary<ulong, ClientHandler>;

			foreach (var c in clients.Values)
			{
				//var b2 = b;
				c.Send(builder, "SkillToActs");

				if (c == client) continue;

				/*if (inc.ActionID == 0x271B)
				{
					p = new PacketBuilder(
					p2.CharacterID = og.CharacterID;
					p2.State = 1;

					b = p2.ToByteArray();
					c.Send(ref b, "SetCharState");
				}*/
			}
		}
	}
}