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
		public static void VerifyLinks(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var timestamp = packet.ReadInt();
			var count = packet.ReadShort();

			var key = ((ulong)count << 32) + (ulong)timestamp;

			var proxy = client.Metadata["proxy"] as ILoginContracts;
			proxy.AddUser(key, client.AccountID);

			builder.New(0x66);
			{
				builder += (byte)1;	// server
				builder += (byte)1;	// channel

				//b = builder.Data;
			}

			client.Send(builder, "VerifyLinks");

			(client.Metadata["timer"] as System.Timers.Timer).Stop();
		}
	}
}