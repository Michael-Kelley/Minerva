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
    public interface IDatabaseProtocol
    {
        object this[string key] { get; }

        void Connect();
        void Disconnect();
        bool ReadRow();
		void RebuildIndices();

        // Login

        bool FetchAccount(string user);

        // Channel

        bool VerifyPassword(int account, string hash);
        void GetFullCharacter(int account, int slot);
        void GetCharacters(int account);
		//void GetStats(int character);
        //void GetEquipment(int character);
        void GetInventory(int character);
		void GetSkills(int character);
		void GetQuickslots(int character);
		int[] CreateCharacter(int account, byte slot, string name, byte _class, bool gender, byte face, byte hair, byte colour, int[] initialStats, List<Tuple<byte[], int, byte>> initialItems, List<int[]> initialSkills, List<int[]> initialQuickslots);
        byte DeleteCharacter(int account, int slot);
		void UpdateCharacterPosition(int account, int slot, byte map, byte x, byte y);
		void MoveItem(int characterId, int oldslot, int newslot);
		void RemoveItem(int characterId, int slot);
		void AddItem(int characterId, int slot, byte[] item, int amount);
		void GetItem(int characterId, int slot);
		void EquipItem(int characterId, int itemslot, string equipslot);

        // Chat
    }
}