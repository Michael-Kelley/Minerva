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
		public static void AuthAccount(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			int passLength = packet.ReadByte();
			int userLength = packet.ReadByte();
			passLength -= userLength;
			var user = packet.ReadString(userLength);
			var pass = packet.ReadString(passLength);

			var ip = client.RemoteEndPoint.ToString().Split(':')[0];

			int auth = 0;
			bool online = false;

			var database = client.Metadata["database"] as IDatabaseContracts;
			var dbresult = database.FetchAccount(user);

            builder.New(0x7d2);
            
			{
				// Check username and password
				if (dbresult != null)
				{
					if (dbresult.Item1 == GenerateHash(pass))
					{
                        
						auth = dbresult.Item2;

						if (auth == 0) { builder += (byte)AccountStatus.Unverified; }
						else if (auth == 2) { builder += (byte)AccountStatus.Banned; }
						else
						{
							online = dbresult.Item3;
							if (online) { builder += (byte)AccountStatus.Online; }
							else
							{
								client.AccountID = dbresult.Item4;
								builder += (byte)AccountStatus.Normal;
								events.SuccessfulLogin("login.700.AuthAccount", new LoginEventArgs(user, pass, ip, LoginResult.Success, client.AccountID));
							}
						}
					}
					else
					{
						builder += (byte)AccountStatus.Incorrect;
						events.FailedLogin("login.700.AuthAccount", new LoginEventArgs(user, pass, ip, LoginResult.WrongPassword));
					}
				}
				else
				{
					builder += (byte)AccountStatus.Incorrect;
					events.FailedLogin("login.700.AuthAccount", new LoginEventArgs(user, pass, ip, LoginResult.UnknownUsername));
				}

				builder += 0x0022F756;
				builder += (short)0x6301;
				builder += new byte[14];
				builder += "000102030405060708090A0B0C0D0E0F";	// Key used when accessing cash shop and guild board.
				builder += (short)0x0100;
				builder += new byte[12];
				builder += (byte)0;
				// For each server, append byte server_id and byte character_count

				//b = builder.Data;
			}

			client.Send(builder, "AuthAccount");

			if (auth == 1 && !online)
			{
                //var cash = "http://shop.cabalonline.com/login.aspx?v1=";
                //var cashdead = "http://shop.cabalonline.com/login.aspx?isdead=1&v1=";
                //var guild = "http://nguild.cabalonline.com/CabalGuild/SecureLogin.aspx?EncVal=";

                //builder.New(0x80);	// Cash shop and guild board URL's.
                //{
                //    builder += (short)0xB4;
                //    builder += (short)0xB2;
                //    builder += cash.Length;
                //    builder += cash;
                //    builder += cashdead.Length;
                //    builder += cashdead;
                //    builder += 0;
                //    builder += guild.Length;
                //    builder += guild;
                //    builder += 0;

                //    //b = builder.Data;
                //}

                //client.Send(builder, "CashGuildURLs");

                //builder.New(0x78);	// Unknown packet
                //{
                //    builder += (byte)9;
                //    builder += (short)0;

                //    //b = builder.Data;
                //}

                //client.Send(builder, "UnknownPacket_0x78");

                //builder.New(0x7D2);	// Unknown packet
                //{
                //    builder += (short)0x7D2;

                //    b = builder.Data;
                //}

                //client.Send(builder, "2002");

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
			//byte[] b;

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
								uint padding;	// Possibly character count?
								byte channels;
								foreach (channel)
								{
									byte id;
									short players;
									byte[22] padding;
									short maxplayers;
									uint ip;
									short port;
									int type;
								}
							}
*/
			#endregion

			var p = new PacketBuilder();
			p.New(0x79);
			{
				p += (byte)channels.Count;

				foreach (var s in channels)
				{
					p += (short)s.Key;
					p += 0;
					p += (byte)s.Value.Count;

					foreach (var c in s.Value)
					{
						p += (byte)c.Key;
						p += c.Value.Item5;
						p += new byte[22];
						p += c.Value.Item4;

						var wan = (uint)client.Metadata["wan"];
						var lan = (uint)client.Metadata["lan"];

						var isLocal = client.RemoteEndPoint.ToString().Contains("127.0.0.1");
						var isLan = client.RemoteEndPoint.ToString().Contains("192.168.");
						var samePC = c.Value.Item2 == wan;

						p += (isLocal && samePC) ? 0x0100007F : (isLan && samePC) ? (int)lan : (int)c.Value.Item2;
						p += c.Value.Item3;
						p += c.Value.Item1;
					}
				}

				//b = p.Data;
			}

            
			client.Send(p, "ServerList");
		}

		public static string GenerateHash(string value)
		{
			var data = System.Text.Encoding.ASCII.GetBytes(value);
			data = System.Security.Cryptography.SHA1.Create().ComputeHash(data);
			return BitConverter.ToString(data).Replace("-", "").ToLower();
		}
	}
}