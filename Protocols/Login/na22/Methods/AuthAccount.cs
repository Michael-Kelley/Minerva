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
		[Packet(0x67, "AuthAccount")]
		public virtual void AuthAccount(ref byte[] message, ClientHandler client, EventHandler events)
		{
			var p = new Outgoing.AuthAccount(true);

			// Check username and password
			int userLength = message[0x0B];
			int passLength = message[0x0A] - userLength;
			var user = System.Text.Encoding.ASCII.GetString(message, 0x0C, userLength);
			var pass = System.Text.Encoding.ASCII.GetString(message, 0x0C + userLength, passLength);
			var ip = client.RemoteEndPoint.ToString().Split(':')[0];		   

			int auth = 0;
			bool online = false;

			var database = client.Metadata["database"] as IDatabaseContracts;
			var dbresult = database.FetchAccount(user);

			if (dbresult != null)
			{
				if (dbresult.Item1 == GenerateHash(pass))
				{
					auth = dbresult.Item2;

					if (auth == 0) { p.Status = AccountStatus.Unverified; }
					else if (auth == 2) { p.Status = AccountStatus.Banned; }
					else
					{
						online = dbresult.Item3;
						if (online) { p.Status = AccountStatus.Online; }
						else
						{
							client.AccountID = dbresult.Item4;
							p.Status = AccountStatus.Normal;
							events.SuccessfulLogin(this, new LoginEventArgs(user, pass, ip, LoginResult.Success, client.AccountID));
						}
					}
				}
				else { p.Status = AccountStatus.Incorrect;
					   events.FailedLogin(this, new LoginEventArgs(user, pass, ip, LoginResult.WrongPassword)); }
			}
			else { p.Status = AccountStatus.Incorrect;
				   events.FailedLogin(this, new LoginEventArgs(user, pass, ip, LoginResult.UnknownUsername)); }

			var b = p.ToByteArray();
			client.Send(ref b, "AuthAccount");

			if (auth == 1 && !online)
			{
				SendChannelList(client);

				var timer = new System.Timers.Timer(5000);
				timer.AutoReset = true;
				timer.Elapsed += (sender, e) => { SendChannelList(client); };

				timer.Start();
				client.Metadata["timer"] = timer;
			}
		}

		static void SendChannelList(ClientHandler client)
		{
			var proxy = client.Metadata["proxy"] as ILoginContracts;
			var channels = proxy.GetChannels();

			#region PSEUDOCODE
/*
							short header;
							short size;
							short opcode;
							byte servers;
							foreach (server)
							{
								short id;
								byte channels;
								foreach (channel)
								{
									byte id;
									short players;
									short maxplayers;
									uint ip;
									short port;
									int type;
								}
							}
*/
			#endregion

			var p = new PacketBuilder(0x79);

			/*p += (byte)1;
			p += (byte)0x01;
			p += (byte)0x10;
			p += (byte)0x01;
			p += -1;
			p += -1;
			p += -1;
			p += -1;
			p += -1;
			p += (byte)0xFF;*/

			p += (byte)channels.Count;

			foreach (var s in channels)
			{
				p += (short)s.Key;
				p += (byte)s.Value.Count;

				foreach (var c in s.Value)
				{
					p += (byte)c.Key;
					p += c.Value.Item5;
					p += c.Value.Item4;

					var wan = (uint)client.Metadata["wan"];
					var lan = (uint)client.Metadata["lan"];

					var isLocal = client.RemoteEndPoint.ToString().Contains("127.0.0.1");
					var isLan = client.RemoteEndPoint.ToString().Contains("192.168.");
					var samePC = c.Value.Item2 == wan;

					p += (isLocal && samePC) ? 0x0100007F : (isLan && samePC) ? lan : c.Value.Item2;
					p += c.Value.Item3;
					p += c.Value.Item1;
				}
			}

			var b = p.Data;

			client.Send(ref b, "ServerList", true);
		}

		public static string GenerateHash(string value)
		{
			var data = System.Text.Encoding.ASCII.GetBytes(value);
			data = System.Security.Cryptography.SHA1.Create().ComputeHash(data);
			return BitConverter.ToString(data).Replace("-", "").ToLower();
		}
	}
}