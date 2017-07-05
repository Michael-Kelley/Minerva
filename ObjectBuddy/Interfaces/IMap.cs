#region Includes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Minerva
{
	public interface IMap
	{
		int ID { get; }
		void MoveClient(ClientHandler client, int gridRow, int gridColumn);
		void RemoveClient(ClientHandler client);
		List<ClientHandler> GetSurroundingClients(int gridRow, int gridColumn, int radius);
		List<ClientHandler> GetSurroundingClients(ClientHandler client, int radius);
		int[] GetWarpDestination(int npc, int order);
		void DropItem(int x, int y, int id, uint entity, uint owner, uint party = 0, int bonus = 0, int amount = 0, uint expiration = 0, int craft = 0, int craftBonus = 0, int upgrades = 0, int upgrade1 = 0, int upgrade2 = 0, int upgrade3 = 0, int upgrade4 = 0);
		byte[] LootItem(ClientHandler client, uint uid);
	}
}