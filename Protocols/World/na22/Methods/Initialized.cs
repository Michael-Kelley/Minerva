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
using System.Timers;

using DItem = Minerva.Structures.Database.Item;
using CEquipment = Minerva.Structures.Client.Equipment;
using CItem = Minerva.Structures.Client.Item;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static void EnterWorld(PacketReader packet, PacketBuilder builder, ClientHandler client, EventHandler events)
		{
			byte[] b;

			b = new byte[0x0C] { 0xE2, 0xB7, 0x0C, 0x00, 0x45, 0x01, 0x08, 0x3B, 0x21, 0xAF, 0x07, 0x01 };	// Looks like it sends an updated premium time to the client.
			client.Send(ref b, "UnknownPacket_0x145");

			var id = packet.ReadInt();
			client.Metadata["cid"] = id;
			client.Metadata["slot"] = (int)id % 8;

			var unk0 = packet.ReadShort();

			var database = client.Metadata["database"] as IDatabaseContracts;
			var server = (int)client.Metadata["server"];

			var fullcharacter = database.GetFullCharacter(server, client.AccountID, (int)client.Metadata["slot"]);
			var character = fullcharacter.Item1;
			var equipment = fullcharacter.Item2;
			var stats = fullcharacter.Item3;
			var items = fullcharacter.Item4;
			var skills = fullcharacter.Item5;
			var quickslots = fullcharacter.Item6;

			client.Metadata["id"] = character.Item1;

			var level = character.Item4;
			var map = character.Rest.Item3;
			var x = (ushort)character.Rest.Item4;
			var y = (ushort)character.Rest.Item5;

			client.Metadata["x"] = x;
			client.Metadata["y"] = y;
			client.Metadata["dest_x"] = x;
			client.Metadata["dest_y"] = y;

			client.Metadata["level"] = level;
			client.Metadata["hp"] = stats.Item1;
			client.Metadata["max_hp"] = stats.Item2;
			client.Metadata["mp"] = stats.Item3;
			client.Metadata["max_mp"] = stats.Item4;

			var name = character.Item3;

			client.Metadata["name"] = name;

			var style = (uint)character.Item5;
			style += stats.Rest.Item5 << 3;
			style += (uint)(character.Item6 << 8);
			style += (uint)(character.Rest.Item1 << 13);
			style += (uint)(character.Item7 << 17);
			style += (!character.Rest.Item2) ? 0 : (uint)(1 << 26);

			client.Metadata["style"] = style;

			var cequipment = new List<CEquipment>();

			for (int i = 0; i < equipment.Count; i++)
			{
				if (equipment[i] != null)
				{
					var de = (DItem)(equipment[i].ToStructure<DItem>());
					var slot = (EquipmentSlots)i;
					cequipment.Add(de.ToClient(slot));
				}
			}

			client.Metadata["equipment"] = cequipment;

			var ditems = new List<byte[]>();
			var citems = new List<byte[]>();

			foreach (var i in items)
			{
				var di = (DItem)(i.Item1.ToStructure<DItem>());
				var slot = i.Item3;
				var ci = (CItem)(di.ToClient(slot));
				var item = ci.ToByteArray();

				if (i.Item2 != 0)
					BitConverter.GetBytes(i.Item2).CopyTo(item, 2);

				ditems.Add(di.ToByteArray());
				citems.Add(item);
			}

			client.Metadata["inventory"] = ditems;

			builder.New(0x8E);
			{
				builder += new byte[50];
				builder += (short)unk0;
				builder += (byte)0;
				builder += map;
				builder += (short)x;
				builder += (short)y;
				builder += (long)stats.Item7;	// XP
				builder += (long)stats.Rest.Rest.Item5;	// Alz
				builder += (int)level;
				builder += 0;		// ?
				builder += stats.Rest.Item1;	// STR
				builder += stats.Rest.Item3;	// DEX
				builder += stats.Rest.Item2;	// INT
				builder += 0;		// ?
				builder += stats.Rest.Item6;		// Sword Rank
				builder += stats.Rest.Rest.Item2;	// Magic Rank
				builder += new byte[6];
				builder += stats.Item1;	// Current HP
				builder += stats.Item2;	// Max HP
				builder += stats.Item3;	// Current MP
				builder += stats.Item4;	// Max MP
				builder += stats.Item5;	// Current SP
				builder += stats.Item6;	// Max SP
				builder += stats.Rest.Item7;		// Sword Skill XP
				builder += stats.Rest.Rest.Item1;	// Sword Skill Points
				builder += stats.Rest.Rest.Item3;	// Magic Skill XP
				builder += stats.Rest.Rest.Item4;	// Magic Skill Points
				builder += (ushort)0;		// Sword Rank Progress
				builder += (ushort)0;		// Magic Rank Progress
				builder += 0;		// ?
				builder += 0;		// ?
				builder += stats.Rest.Item4;	// Honour
				builder += 0;		// Penalty XP
				builder += 0;		// ?
				builder += (ushort)0;		// ?
				builder += (byte)127;		// Chat IP
				builder += (byte)0;
				builder += (byte)0;
				builder += (byte)1;
				builder += (ushort)38121; // Chat Port
				builder += (byte)0;	// Nation
				builder += 0;			// ?
				builder += 0;			// Warp Codes
				builder += (uint)(1 << (map - 1));	// Map Codes
				builder += style;
				builder += new byte[3];
				builder += (byte)cequipment.Count;
				builder += (ushort)citems.Count;
				builder += (byte)skills.Count;
				builder += quickslots.Count;
				builder += new byte[128];	// Quest Data
				builder += new byte[128];	// Quest Flags
				builder += new byte[128];	// Quest Dungeon Data
				builder += new byte[128];	// Quest Dungeon Flags
				builder += (byte)1;	// Craft Level 0
				builder += (byte)1;	// Craft Level 1
				builder += (byte)1;	// Craft Level 2
				builder += (byte)1;	// Craft Level 3
				builder += (byte)1;	// Craft Level 4
				builder += (byte)0;	// Craft XP 0
				builder += (byte)0;	// Craft XP 1
				builder += (byte)0;	// Craft XP 2
				builder += (byte)0;	// Craft XP 3
				builder += (byte)0;	// Craft XP 4
				builder += new byte[16];	// Craft Flags
				builder += 0;		// Craft Type
				builder += (byte)0;	// ?
				builder += (byte)(name.Length + 1);
				builder += name;

				foreach (var e in cequipment)
					builder += e.ToByteArray();

				foreach (var i in citems)
					builder += i;

				foreach (var s in skills)
				{
					builder += (ushort)s[0];	// Skill ID
					builder += (byte)s[1];	// Skill Level
					builder += (byte)s[2];	// Skill Slot
				}

				foreach (var q in quickslots)
				{
					builder += (byte)q[0];	// Skill ID (referenced by Skill Slot)
					builder += (byte)q[1];	// Slot (in quickslot bars)
				}

				//b = builder.Data;
			}

			client.Send(builder, "Initialised");

			events.Warped("world.700.Initialized", client, map, x, y);

			#region PSEUDOCODE
			/*
			 * ushort header
			 * ushort size
			 * ushort opcode
			 * ushort players?
			 * 
			 * foreach player
			 * 
			 * uint characterID (acc * 8 + slot)
			 * ushort unk0
			 * byte unk1
			 * byte unk2
			 * uint unk3
			 * uint timestamp? (probably that milliseconds since server start crap)
			 * ushort x_current
			 * ushort y_current
			 * ushort x_destination
			 * ushort y_destination
			 * byte pk_level
			 * byte[6] unk4
			 * uint style
			 * byte animation
			 * byte[9] unk5 (one of these is most likely an animation keystamp of some sort)
			 * byte isDead
			 * byte equipment_count
			 * ushort unk6
			 * byte isSelling
			 * uint unk7
			 * byte name_length (+1)
			 * byte[name_length - 1] name
			 * byte guild_length
			 * byte[guild_length] guild
			 * byte unk8
			 * byte isHelmVisible
			 * byte unk9
			 * 
			 * a series of ushort-byte pairs follow, and contains the current equipment
			 * foreach equipment
			 * 
			 * ushort item_id (merged with bonus and isBound, obviously)
			 * byte slot
			 * 
*/
			#endregion

			var clients = (client.Metadata["map"] as IMap).GetSurroundingClients(client, 2);

			if (clients.Count > 1)
			{
				builder.New(0xC8);
				{
					builder += (short)(clients.Count - 1);

					foreach (var c in clients)
					{
						if (c == client) continue;
						if (!c.Metadata.ContainsKey("id") || (uint)c.Metadata["cid"] == 0) continue;

						var cid = (uint)c.Metadata["cid"];
						level = (uint)c.Metadata["level"];
						var timestamp = (uint)c.Metadata["timestamp"];
						x = (ushort)c.Metadata["x"];
						y = (ushort)c.Metadata["y"];
						var dx = (ushort)c.Metadata["dest_x"];
						var dy = (ushort)c.Metadata["dest_y"];
						style = (uint)c.Metadata["style"];

						builder += (int)cid;
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

						cequipment = (List<CEquipment>)c.Metadata["equipment"];

						builder += (byte)cequipment.Count;
						builder += (short)0;
						builder += (byte)0;	// 1 = has private shop open
						builder += 0;

						name = c.Metadata["name"] as string;

						builder += (byte)(name.Length + 1);
						builder += name;
						builder += (byte)0;

						foreach (var e in cequipment)
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

						var cid = (uint)client.Metadata["cid"];
						level = (uint)client.Metadata["level"];
						var timestamp = (uint)client.Metadata["timestamp"];
						x = (ushort)client.Metadata["x"];
						y = (ushort)client.Metadata["y"];
						var dx = (ushort)client.Metadata["dest_x"];
						var dy = (ushort)client.Metadata["dest_y"];
						style = (uint)client.Metadata["style"];

						builder += (int)cid;
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

						cequipment = (List<CEquipment>)client.Metadata["equipment"];

						builder += (byte)cequipment.Count;
						builder += (short)0;
						builder += (byte)0;	// 1 = has private shop open
						builder += 0;

						name = c.Metadata["name"] as string;

						builder += (byte)(name.Length + 1);
						builder += name;
						builder += (byte)0;

						foreach (var e in cequipment)
						{
							builder += (short)e.ID;
							builder += (byte)e.Slot;
						}

						//b = builder.Data;
					}

					c.Send(builder, "UnknownPacket_0xC8");
				}
			}

			b = new byte[0x48] { 0xE2, 0xB7, 0x48, 0x00, 0x6D, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
								 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
								 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
								 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x97, 0xB6, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00,
								 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

			client.Send(ref b, "UnknownPacket_0x026D");
		}
	}
}