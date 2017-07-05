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
using System.Linq;
using System.Text;
using System.Threading;

#endregion

namespace Minerva
{
	class Map : IMap
	{
		MapCell[,] cells;
		List<MapCell>[] active;
		List<WarpIndex> warps;
		Dictionary<int, List<WarpList>> warpNPCs;
		int _id;
		int deadWarp, returnWarp;
		int tickcount;
		int[] ticksleft;

		public int ID { get { return _id; } }

		public Map(int id, List<WarpIndex> warps, int deadWarp, int returnWarp, Dictionary<int, List<WarpList>> warpNPCs = null)
		{
			_id = id;
			this.deadWarp = deadWarp - 1;
			this.returnWarp = returnWarp - 1;
			this.warps = warps;
			this.warpNPCs = warpNPCs;

			cells = new MapCell[16,16];
			active = new List<MapCell>[10];
			ticksleft = new int[10];

			for (int i = 0; i < 10; i++)
			{
				active[i] = new List<MapCell>();
				ticksleft[i] = i * 100;
			}

			for (int c = 0; c < 16; c++)
				for (int r = 0; r < 16; r++)
					cells[r, c] = new MapCell(this, r, c);

			var thread = new Thread(Run);
			thread.Start();
		}

		void Run()
		{
			try
			{
				tickcount = Environment.TickCount;

				while (true)
				{
					int elapsed = Environment.TickCount - tickcount;
					tickcount = Environment.TickCount;

					for (int i = 0; i < 10; i++)
					{
						ticksleft[i] -= elapsed;

						if (ticksleft[i] <= 0)
						{
							foreach (var a in active[i])
								a.Update();

							ticksleft[i] += 1000;

							if (ticksleft[i] <= 900)
								Log.Warning("Couldn't keep up!");
						}
					}

					Thread.Sleep(10);
				}
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
			}
		}

		public void MoveClient(ClientHandler client, int gridRow, int gridColumn)
		{
			if (client.Metadata.ContainsKey("cell"))
			{
				var cell = client.Metadata["cell"] as MapCell;
				cell.RemoveClient(client);

				if (!cell.HasClients)
				{
					active[cell.List].Remove(cell);
					cell.List = -1;
					cell.Deactivate();
				}
			}

			var newgrid = cells[gridRow, gridColumn];

			if (!newgrid.HasClients)
			{
				var i = GetShortestCellList();
				active[i].Add(newgrid);
				newgrid.List = i;
			}

			newgrid.AddClient(client);
			newgrid.Activate();
		}

		int GetShortestCellList()
		{
			var result = 0;
			var count = int.MaxValue;

			for (int i = 0; i < 10; i++)
			{
				if (active[i].Count < count)
				{
					result = i;
					count = active[i].Count;
				}
			}

			return result;
		}

		public void RemoveClient(ClientHandler client)
			{ (client.Metadata["cell"] as MapCell).RemoveClient(client); }

		public List<ClientHandler> GetSurroundingClients(int gridRow, int gridColumn, int radius)
		{
			var result = new List<ClientHandler>();

			for (int c = -radius; c <= radius; c++)
				for (int r = -radius; r <= radius; r++)
					if (gridRow + r > -1 && gridRow + r < 16 && gridColumn + c > -1 && gridColumn + c < 16)
						result.AddRange(cells[gridRow + r, gridColumn + c].Clients);

			return result;
		}

		public List<ClientHandler> GetSurroundingClients(ClientHandler client, int radius)
			{ return GetSurroundingClients((MapCell)client.Metadata["cell"], radius); }

		internal List<ClientHandler> GetSurroundingClients(MapCell cell, int radius)
			{ return GetSurroundingClients(cell.Row, cell.Column, radius); }

		public int[] GetWarpDestination(int npc, int order)
		{
			if (order == -1)
				return new[] { _id, warps[returnWarp].X, warps[returnWarp].Y };

			var warp = warpNPCs[npc][order];
			var dest = warps[warp.TargetID - 1];

			return new[] { dest.WorldID, dest.X, dest.Y };
		}


		public void DropItem(int x, int y, int id, uint entity, uint owner, uint party = 0, int bonus = 0, int amount = 0, uint expiration = 0, int craft = 0, int craftBonus = 0, int upgrades = 0, int upgrade1 = 0, int upgrade2 = 0, int upgrade3 = 0, int upgrade4 = 0)
		{
			int row = x / 16;
			int column = y / 16;

			var item = new ItemEntity(15000, (ushort)id, owner, party, (byte)bonus, (uint)amount, expiration, (byte)craft, (byte)craftBonus, (byte)upgrades, (byte)upgrade1, (byte)upgrade2, (byte)upgrade3, (byte)upgrade4);
			var cell = cells[row, column];
			var uid = cell.AddItem(item, _id);

			var clients = GetSurroundingClients(cell, 3);

			PacketBuilder b = new PacketBuilder();

			if (clients.Count > 0)
				b = clients[0].CreatePacket("ItemDropped", (ushort)x, (ushort)y, (ushort)id, uid, entity);

			foreach (var c in clients)
				c.Send(b, "ItemDropped");
		}


		public byte[] LootItem(ClientHandler client, uint uid)
		{
			var cell = client.Metadata["cell"] as MapCell;

			if (!cell.Items.ContainsKey(uid))
				return null;

			var item = cell.Items[uid];
			var ditem = new Item();
			ditem.ID = item.ID;

			var disposed = client.CreatePacket("ItemDisposed", uid);
			cell.RemoveItem(uid, disposed);

			return ditem.ToByteArray();
		}
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
	struct Item
	{
		public ushort ID;
		public byte Bonus;
		public byte IsBound;
		public byte CraftType;
		public byte CraftBonus;
		public byte Upgrades;
		public byte Upgrade1;
		public byte Upgrade2;
		public byte Upgrade3;
		public byte Upgrade4;
		public uint ExpirationDate;
	}
}