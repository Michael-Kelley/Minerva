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
		[Packet(0x65, "Connect2Svr")]
		public virtual void Connect(ref byte[] message, ClientHandler client, EventHandler events)
		{
			var b = new byte[0x12]
						{0xE2, 0xB7, 0x12, 0x00, 0x65, 0x00, 0x1B, 0x82,
						 0xBC, 0x34, 0x0F, 0xFF, 0x54, 0x12, 0x32, 0x00,
						 0x16, 0x11									 };

			var key = BitConverter.ToUInt32(b, 0x6);
			var step = BitConverter.ToUInt16(b, 0x10);
			client.ChangeKey(key, step);

			client.Send(ref b, "Connect2Svr");
		}
	}
}