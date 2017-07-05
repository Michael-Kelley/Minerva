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

using CEquipment = Minerva.Structures.Client.Equipment;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void Warp(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			var npc = packet.ReadByte();
			var unk0 = packet.ReadShort();
			var order = packet.ReadShort();

			var map = client.Metadata["map"] as IMap;
			int[] dest = map.GetWarpDestination(npc, (int)order);

			var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

			if (clients.Count > 1)
			{
				var id = (uint)client.Metadata["cid"];

				builder.New(0xC9);
				{
					builder += (int)id;
					builder += (byte)dest[0];

					//b = builder.Data;
				}

				foreach (var c in clients)
				{
					if (c == client) continue;

					//var b2 = b;

					c.Send(builder, "UnknownPacket_0xC9");
				}

				builder.New(0xC9);
				{
					foreach (var c in clients)
					{
						if (c == client) continue;

						id = (uint)c.Metadata["cid"];

						builder += (int)id;
					}

					builder += (byte)dest[0];
					//b = builder.Data;
				}

				client.Send(builder, "UnknownPacket_0xC9");
			}

			builder.New(0xF4);
			{
				builder += (short)dest[1];	// x
				builder += (short)dest[2];	// y
				builder += 0;
				builder += 0;
				builder += 0;
				builder += 0;
				builder += (byte)0;
				builder += 0;

				//b = builder.Data;
			}

			client.Send(builder, "WarpCommand");

			client.Metadata["x"] = (ushort)dest[1];
			client.Metadata["y"] = (ushort)dest[2];

			map.RemoveClient(client);
			events.Warped("world.700.WarpCommand", client, dest[0], dest[1], dest[2]);

			clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

			if (clients.Count > 1)
			{
				builder.New(0xC8);
				{
					builder += (byte)(clients.Count - 1);
					builder += (byte)0x31;

					foreach (var c in clients)
					{
						if (c == client) continue;
						//if (!c.Metadata.ContainsKey("id") || (uint)c.Metadata["cid"] == 0) continue;

						var id = (uint)c.Metadata["cid"];
						var level = (uint)c.Metadata["level"];
						var timestamp = (uint)c.Metadata["timestamp"];
						var x = (ushort)c.Metadata["x"];
						var y = (ushort)c.Metadata["y"];
						var dx = (ushort)c.Metadata["dest_x"];
						var dy = (ushort)c.Metadata["dest_y"];
						var style = (uint)c.Metadata["style"];

						builder += (int)id;
						builder += (short)0x000D;
						builder += (short)0x0100;
						builder += (int)level;
						builder += Environment.TickCount - (int)timestamp;
						builder += (short)x;
						builder += (short)y;
						builder += (short)dx;
						builder += (short)dy;
						builder += (byte)0;	// PK penalty
						builder += 0;
						builder += (short)0;
						builder += (int)style;
						builder += (byte)0;	// Animation ID
						builder += 0;
						builder += 0;
						builder += (byte)0;
						builder += (byte)0;	// 1 = dead

						var equipment = (List<CEquipment>)c.Metadata["equipment"];

						builder += (byte)equipment.Count;
						builder += (short)0;
						builder += (byte)0;	// 1 = has private shop open
						builder += 0;

						var name = c.Metadata["name"] as string;

						builder += (byte)(name.Length + 1);
						builder += name;
						builder += (byte)0;

						foreach (var e in equipment)
						{
							builder += (short)e.ID;
							builder += (byte)e.Slot;
						}
					}

					//b = builder.Data;
				}

				client.Send(builder, "UnknownPacket_0xC8");

				foreach (var c in clients)
				{
					if (c == client) continue;

					builder.New(0xC8);
					{
						builder += (short)0x3101;

						var id = (uint)client.Metadata["cid"];
						var level = (uint)client.Metadata["level"];
						var timestamp = (uint)client.Metadata["timestamp"];
						var x = (ushort)client.Metadata["x"];
						var y = (ushort)client.Metadata["y"];
						var dx = (ushort)client.Metadata["dest_x"];
						var dy = (ushort)client.Metadata["dest_y"];
						var style = (uint)client.Metadata["style"];

						builder += (int)id;
						builder += (short)0x000D;
						builder += (short)0x0100;
						builder += (int)level;
						builder += Environment.TickCount - (int)timestamp;
						builder += (short)x;
						builder += (short)y;
						builder += (short)dx;
						builder += (short)dy;
						builder += (byte)0;	// PK penalty
						builder += 0;
						builder += (short)0;
						builder += (int)style;
						builder += (byte)0;	// Animation ID
						builder += 0;
						builder += 0;
						builder += (byte)0;
						builder += (byte)0;	// 1 = dead

						var equipment = (List<CEquipment>)client.Metadata["equipment"];

						builder += (byte)equipment.Count;
						builder += (short)0;
						builder += (byte)0;	// 1 = has private shop open
						builder += 0;

						var name = c.Metadata["name"] as string;

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
			}

			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];
			var slot = (int)((uint)client.Metadata["cid"] % 8);
			database.UpdateCharacterPosition(server, client.AccountID, slot, (byte)dest[0], (byte)dest[1], (byte)dest[2]);
		}
	}
}