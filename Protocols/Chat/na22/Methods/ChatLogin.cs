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
using System.Text;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		[Packet(0x193, "ChatLogin")]
		public virtual void Login(ref byte[] message, ClientHandler client, EventHandler events)
		{
			var id = BitConverter.ToUInt32(message, 0x0A);
			var timestamp = BitConverter.ToUInt32(message, 0x12);
			byte unk = message[0x19];
			var dataLength = BitConverter.ToUInt16(message, 0x1A);
			var length = BitConverter.ToUInt16(message, 0x1C);
			var name = Encoding.ASCII.GetString(message, 0x1E, length);

			var b = new byte[0x07] { 0xE2, 0xB7, 0x07, 0x00, 0x93, 0x01, 0x00 };

			client.Send(ref b, "ChatLogin");

			b = new byte[0x0A] { 0xE2, 0xB7, 0x0A, 0x00, 0xC5, 0x01, 0x01, 0x00,
								 0x00, 0x00 };

			client.Send(ref b, "UnknownPacket_0x01C5");
		}
	}
}