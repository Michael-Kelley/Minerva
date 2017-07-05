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

using CItem = Minerva.Structures.Client.Equipment;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void MessageEvent(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var name = client.Metadata["name"] as string;
			var x = (ushort)client.Metadata["x"];
			var y = (ushort)client.Metadata["y"];
			var id = (uint)client.Metadata["id"];
			var cid = (uint)client.Metadata["cid"];
			var map = client.Metadata["map"] as IMap;

			int size = packet.ReadByte() - 3;
			packet.Skip(2);
			var type = packet.ReadByte();
			var msg = packet.ReadString(size);
			var smsg = msg.Split(' ');

			if (msg == "/quit")
			{
				client.Disconnect();
				return;
			}

			if (smsg.Length > 1 && smsg[0] == "/drop")
			{
				int itemid = 0;

				if (int.TryParse(smsg[1], out itemid))
					map.DropItem(x, y, itemid, cid, id);

				return;
			}

			var clients = map.GetSurroundingClients(client, 1);

			if (smsg.Length > 1 &&  smsg[0] == "/spawn")
			{
				ushort mobid = 0;

				if (!ushort.TryParse(smsg[1], out mobid))
					return;

				var p = client.CreatePacket("MobSpawned", mobid);

				client.Send(p, "MobSpawned");

				return;
			}

			if (msg == "/partytime")
			{
				uint pid = 1337;

				foreach (var c in clients)
				{
					builder.New(0xC9);
					{
						builder += (int)pid;
						builder += (byte)12;

						//b = builder.Data;
					}

					var timestamp = (uint)c.Metadata["timestamp"];
					var style = (uint)c.Metadata["style"];

					c.Send(builder, "UnknownPacket_0xC9");

					builder.New(0xC8);
					{
						builder += (short)0x3101;
						builder += (int)pid++;
						builder += (short)0x000D;
						builder += (short)0x0100;
						builder += 1;
						builder += Environment.TickCount - (int)timestamp;
						builder += (short)(x + 1);
						builder += (short)y;
						builder += (short)(x + 1);
						builder += (short)y;
						builder += (byte)0;
						builder += 0;
						builder += (short)0;
						builder += (int)style;
						builder += (byte)1;
						builder += (byte)0;
						builder += 0;
						builder += 0;
						builder += (byte)0;

						var equipment = (List<CItem>)c.Metadata["equipment"];

						builder += (byte)(equipment.Count);
						builder += (short)0;
						builder += (byte)0;
						builder += 0;

						name = "PARTY TIME!!";

						builder += (byte)(name.Length + 1);
						builder += name;
						builder += (byte)0;

						foreach (var e in equipment)
						{
							builder += (short)e.ID;
							builder += (byte)e.Slot;
						}

						//b = builder.Data;
					}

					c.Send(builder, "UnknownPacket_0xC8");
				}

				return;
			}

			foreach (var c in clients)
			{
				builder.New(0xD9);
				{
					builder += (int)cid;
					builder += (byte)(msg.Length + 3);
					builder += (byte)0xFE;
					builder += (byte)0xFE;
					builder += type;
					builder += msg;

					//b = builder.Data;
				}

				c.Send(builder, "MessageEvnt");
			}

			events.SaidLocal("world.700.MessageEvnt", (int)cid, name, msg);
		}
	}
}