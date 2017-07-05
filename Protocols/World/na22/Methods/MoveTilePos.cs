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
		public static void ChangeMapCell(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			//byte[] b;

			packet.Skip(4);
			var x = packet.ReadShort();
			var y = packet.ReadShort();

			var map = client.Metadata["map"] as IMap;
			map.MoveClient(client, x / 16, y / 16);

			var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

			if (clients.Count > 1)
			{
				builder.New(0xC8);
				{
					builder += (short)(clients.Count - 1);

					foreach (var c in clients)
					{
						if (c == client) continue;
						//if (!c.Metadata.ContainsKey("id") || (uint)c.Metadata["cid"] == 0) continue;

						var id = (uint)c.Metadata["cid"];
						var level = (uint)c.Metadata["level"];
						var timestamp = (uint)c.Metadata["timestamp"];
						var px = (ushort)c.Metadata["x"];
						var py = (ushort)c.Metadata["y"];
						var dx = (ushort)c.Metadata["dest_x"];
						var dy = (ushort)c.Metadata["dest_y"];
						var style = (uint)c.Metadata["style"];

						builder += (int)id;
						builder += (short)0x000D;
						builder += (short)0x0100;
						builder += (int)level;
						builder += Environment.TickCount - (int)timestamp;
						builder += (short)px;
						builder += (short)py;
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

						var equipment = (List<CItem>)c.Metadata["equipment"];

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

				// nested wooooooo!
				{
					var id = (uint)client.Metadata["cid"];
					var level = (uint)client.Metadata["level"];
					var timestamp = (uint)client.Metadata["timestamp"];
					var px = (ushort)client.Metadata["x"];
					var py = (ushort)client.Metadata["y"];
					var dx = (ushort)client.Metadata["dest_x"];
					var dy = (ushort)client.Metadata["dest_y"];
					var style = (uint)client.Metadata["style"];

					builder.New(0xC8);
					{
						builder += (ushort)0x3101;
						builder += (int)id;
						builder += (short)0x000D;
						builder += (short)0x0100;
						builder += (int)level;
						builder += Environment.TickCount - (int)timestamp;
						builder += (short)px;
						builder += (short)py;
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

						var equipment = (List<CItem>)client.Metadata["equipment"];

						builder += (byte)equipment.Count;
						builder += (short)0;
						builder += (byte)0;
						builder += 0;

						var name = client.Metadata["name"] as string;

						builder += (byte)(name.Length + 1);
						builder += name;
						builder += (byte)0;

						foreach (var e in equipment)
						{
							builder += e.ID;
							builder += (byte)e.Slot;
						}

						//b = builder.Data;
					}

					foreach (var c in clients)
					{
						if (c == client) continue;

						//var b2 = b;

						c.Send(builder, "UnknownPacket_0xC8");
					}
				}
			}
		}
	}
}