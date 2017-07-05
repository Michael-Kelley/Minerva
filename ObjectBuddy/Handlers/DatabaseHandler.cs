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

#endregion

namespace Minerva
{
    public class DatabaseHandler
    {
        IDatabaseProtocol protocol;

        public DatabaseHandler(string type, string ip, string db, string user, string pass)
        {
            var file = String.Format("lib/protocols/database/{0}.dll", type);
            var asm = System.Reflection.Assembly.LoadFrom(file);
            var args = new object[] { ip, db, user, pass };
            var ptype = asm.GetType("Minerva.DatabaseProtocol");
            this.protocol = Activator.CreateInstance(ptype, args) as IDatabaseProtocol;

            this.protocol.Connect();
			this.protocol.RebuildIndices();
        }

        ~ DatabaseHandler()
        {
            protocol.Disconnect();
        }

        public bool FetchAccount(string user) { return protocol.FetchAccount(user); }
        public bool ReadRow() { return protocol.ReadRow(); }
        public bool VerifyPassword(int account, string hash) { return protocol.VerifyPassword(account, hash); }
        public void GetFullCharacter(int account, int slot) { protocol.GetFullCharacter(account, slot); }
        public void GetCharacters(int account) { protocol.GetCharacters(account); }
        //public void GetEquipment(int characterId) { protocol.GetEquipment(characterId); }
		//public void GetStats(int characterId) { protocol.GetStats(characterId); }
		public void GetInventory(int characterId) { protocol.GetInventory(characterId); }
		public void GetSkills(int characterId) { protocol.GetSkills(characterId); }
		public void GetQuickslots(int characterId) { protocol.GetQuickslots(characterId); }
		public int[] CreateCharacter(int account, byte slot, string name, byte _class, bool gender, byte face, byte hair, byte colour, int[] initialStats, List<Tuple<byte[], int, byte>> initialItems, List<int[]> initialSkills, List<int[]> initialQuickslots) { return protocol.CreateCharacter(account, slot, name, _class, gender, face, hair, colour, initialStats, initialItems, initialSkills, initialQuickslots); }
        public byte DeleteCharacter(int account, int slot) { return protocol.DeleteCharacter(account, slot); }
		public void UpdateCharacterPosition(int account, int slot, byte map, byte x, byte y) { protocol.UpdateCharacterPosition(account, slot, map, x, y); }
		public void MoveItem(int characterId, int oldslot, int newslot) { protocol.MoveItem(characterId, oldslot, newslot); }
		public void RemoveItem(int characterId, int slot) { protocol.RemoveItem(characterId, slot); }
		public void AddItem(int characterId, int slot, byte[] item, int amount) { protocol.AddItem(characterId, slot, item, amount); }
		public void GetItem(int characterId, int slot) { protocol.GetItem(characterId, slot); }
		public void EquipItem(int characterId, int itemslot, string equipslot) { protocol.EquipItem(characterId, itemslot, equipslot); }

        public object this[string key]
        {
            get { return protocol[key]; }
        }
    }
}