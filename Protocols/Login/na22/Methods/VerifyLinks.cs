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
		[Packet(0x66, "VerifyLinks")]
		public virtual void VerifyLinks(ref byte[] message, ClientHandler client, EventHandler events)
		{
			var packet = (Incoming.VerifyLinks)message.ToStructure<Incoming.VerifyLinks>();
			var key = ((ulong)packet.Count << 32) + (ulong)packet.Timestamp;

			var proxy = client.Metadata["proxy"] as ILoginContracts;
			proxy.AddUser(key, client.AccountID);

			var p = new Outgoing.VerifyLinks(true);
			p.Server = 1;
			p.Channel = 1;

			var b = p.ToByteArray();

			client.Send(ref b, "VerifyLinks");

			(client.Metadata["timer"] as System.Timers.Timer).Stop();
		}
	}
}