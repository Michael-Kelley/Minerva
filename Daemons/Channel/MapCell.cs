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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

#endregion

namespace Minerva
{
	class MapCell
	{
		Map map;
		ConcurrentDictionary<int, ClientHandler> clients;
		bool _isactive;
		int timestamp;
		public int List;
		ushort itemIDs;		
		//List<Mob> mobs;

		public int Row, Column;
		public ConcurrentDictionary<uint, ItemEntity> Items;

		public bool IsActive
			{ get { return _isactive; } }

		public ICollection<ClientHandler> Clients
		{
			get { return clients.Values; }
		}

		public bool HasClients
			{ get { return (clients.Count > 0); } }

		public MapCell(Map owner, int row, int column)
		{
			map = owner;
			clients = new ConcurrentDictionary<int, ClientHandler>();
			Items = new ConcurrentDictionary<uint, ItemEntity>();
			Row = row;
			Column = column;
		}

		public void AddClient(ClientHandler client)
		{
			clients.TryAdd(client.AccountID, client);
			client.Metadata["cell"] = this;
		}

		public void RemoveClient(ClientHandler client)
		{
			ClientHandler temp;
			clients.TryRemove(client.AccountID, out temp);
			temp = null;
		}

		public void Activate()
		{
			if (!_isactive)
			{
				// Compare current tickcount with timestamp, then discard items, update mob HP, etc. accordingly.
				_isactive = true;
			}
		}

		public void Deactivate()
		{
			if (_isactive)
			{
				timestamp = Environment.TickCount;
				_isactive = false;
			}
		}

		public void Update()
		{
			foreach (var c in clients.Values)
			{
				var hp = c.CreatePacket("UpdateStats", c, (byte)3, (ushort)0);	// HP update packet (client, subopcode, damaged_health)
				var mp = c.CreatePacket("UpdateStats", c, (byte)4, (ushort)0);	// MP update packet

				c.Send(hp, "UpdateStats_HP", true);
				c.Send(mp, "UpdateStats_MP", true);

				foreach (var i in Items)
				{
					var expired = i.Value.UpdateOrDie();

					if (expired)
					{
						var disposed = c.CreatePacket("ItemDisposed", i.Key);
						RemoveItem(i.Key, disposed);
					}
				}
			}
		}

		public uint AddItem(ItemEntity item, int map)
		{
			uint uid = unchecked(itemIDs++);
			uid += (uint)(Row * 16 + Column) << 24;
			uid += (uint)map << 32;
			Items.TryAdd(uid, item);

			return uid;
		}

		public void RemoveItem(uint uid, PacketBuilder packet)
		{
			ItemEntity item = null;
			Items.TryRemove(uid, out item);
			item = null;

			var surrounding = map.GetSurroundingClients(this, 3);

			foreach (var s in surrounding)
				s.Send(packet, "ItemDisposed");
		}
	}
}