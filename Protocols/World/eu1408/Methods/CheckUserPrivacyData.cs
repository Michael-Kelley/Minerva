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
		public static void CheckUserPrivacyData(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			packet.Skip(4);
			var pass = packet.ReadString(32).Trim('\0');

			pass = GenerateHash(pass);

			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];

			bool? auth = database.VerifyPassword(server, client.AccountID, pass);

			builder.New(0x320);
			{
				if (auth != null)
					builder += (bool)(auth) ? (byte)1 : (byte)0;
				else
					events.Error("world.700.CheckUserPrivacyData", "CheckUserPrivacyData: Account ID not found!");

				//b = builder.Data;
			}

			client.Send(builder, "CheckUserPrivacyData");
		}

		public static string GenerateHash(string value)
		{
			var data = System.Text.Encoding.ASCII.GetBytes(value);
			data = System.Security.Cryptography.SHA1.Create().ComputeHash(data);
			return BitConverter.ToString(data).Replace("-", "").ToLower();
		}
	}
}