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
		[Packet(0x192, "ChatAuthAccount")]
		public virtual void ChatAuthAccount(ref byte[] message, ClientHandler client, EventHandler events)
		{
			var p = new Outgoing.ChatAuthAccount(true);

			// Check username and password
			var pid = BitConverter.ToUInt32(message, 0x0A);
			//var cid = (uint)client.Metadata["cid"];
			var length = BitConverter.ToUInt16(message, 0x10);
			var user = Encoding.ASCII.GetString(message, 0x12, length);
			length = BitConverter.ToUInt16(message, 0x12 + length);
			var pass = Encoding.ASCII.GetString(message, 0x14 + user.Length, length);	   

			int auth = 0;

			var database = client.Metadata["database"] as IDatabaseContracts;
			var dbresult = database.FetchAccount(user);

			if (/*pid == cid && */dbresult != null)
			{
				p.CharID = pid;

				if (dbresult.Item1 == GenerateHash(pass))
				{
					auth = dbresult.Item2;

					if (auth == 0)
						{ p.Status = AccountStatus.Unverified; }
					else if (auth == 2)
						{ p.Status = AccountStatus.Banned; }
					else
						p.Status = AccountStatus.Normal;
				}
				else { p.Status = AccountStatus.Incorrect; }
			}
			else { p.Status = AccountStatus.Incorrect; }

			var b = p.ToByteArray();
			client.Send(ref b, "ChatAuthAccount");
		}

		public static string GenerateHash(string value)
		{
			var data = System.Text.Encoding.ASCII.GetBytes(value);
			data = System.Security.Cryptography.SHA1.Create().ComputeHash(data);
			return BitConverter.ToString(data).Replace("-", "").ToLower();
		}
	}
}