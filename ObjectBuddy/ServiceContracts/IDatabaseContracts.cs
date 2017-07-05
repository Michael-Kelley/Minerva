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
using System.ServiceModel;

using Character = System.Tuple<uint, byte, string, uint, byte, byte, byte, System.Tuple<byte, bool, byte, byte, byte>>;
using CharacterEquipment = System.Collections.Generic.List<byte[]>;
using CharacterStats = System.Tuple<ushort, ushort, ushort, ushort, ushort, ushort, ulong, System.Tuple<uint, uint, uint, uint, uint, byte, ushort, System.Tuple<ushort, byte, ushort, ushort, ulong>>>;
using CharacterItems = System.Collections.Generic.List<System.Tuple<byte[], int, short>>;

#endregion

namespace Minerva
{
	[ServiceContract]
	public interface IDatabaseContracts
	{
		#region Login

		[OperationContract]
		Tuple<string, int, bool, int> FetchAccount(string user);

		#endregion

		#region World

		[OperationContract]
		bool VerifyPassword(int server, int account, string hash);

		[OperationContract]
		Tuple<Character, CharacterEquipment, CharacterStats, CharacterItems, List<int[]>, List<int[]>> GetFullCharacter(int server, int account, int slot);

		[OperationContract]
		List<Tuple<Character, CharacterEquipment>> GetCharacters(int server, int account);

		//[OperationContract]
		//List<byte[]> GetEquipment(int server, int character);

		//[OperationContract]
		//CharacterStats GetStats(int server, int character);

		//[OperationContract]
		//void GetInventory(int server, int character);

		[OperationContract]
		int[] CreateCharacter(int server, int account, byte slot, string name, byte _class, bool gender, byte face, byte hair, byte colour);

		[OperationContract]
		byte DeleteCharacter(int server, int account, int slot);

		[OperationContract]
		void UpdateCharacterPosition(int server, int account, int slot, byte map, byte x, byte y);

		[OperationContract]
		void MoveItem(int server, int characterId, int oldslot, int newslot);

		[OperationContract]
		void RemoveItem(int server, int characterId, int slot);

		[OperationContract]
		void AddItem(int server, int characterId, int slot, byte[] item, int amount);

		[OperationContract]
		Tuple<byte[], int> GetItem(int server, int characterId, int slot);

		[OperationContract]
		void EquipItem(int server, int characterId, int itemslot, string equipslot);

		#endregion

		#region Chat



		#endregion
	}
}