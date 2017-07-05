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

using PacketInfo = System.Tuple<string, Minerva.PacketMethod>;

#endregion

namespace Minerva
{
	partial class PacketProtocol
	{
		public static string Requires()
		{
			return null;
		}

		public static void Initialise(Dictionary<ushort, PacketInfo> methods, Dictionary<string, PacketConstructor> constructors)
		{
			//methods[0x] = new PacketInfo("", );
			methods[0x0085] = new PacketInfo("GetMyChartr", GetCharacters);
			methods[0x0086] = new PacketInfo("NewMyChartr", CreateCharacter);
			methods[0x0087] = new PacketInfo("DelMyChartr", DeleteCharacter);
			methods[0x008C] = new PacketInfo("Connect2Svr", Connect);
			methods[0x008E] = new PacketInfo("Initialized", EnterWorld);
			methods[0x008F] = new PacketInfo("Uninitialize", LeaveWorld);
			methods[0x0094] = new PacketInfo("GetSvrTime", GetServerTime);
			methods[0x0096] = new PacketInfo("ItemEquipS0", EquipItem);
			methods[0x0099] = new PacketInfo("ItemLooting", LootItem);
			methods[0x009D] = new PacketInfo("ItemDroping", DropItem);
			methods[0x009E] = new PacketInfo("ItemHolding", GrabItem);
			methods[0x009F] = new PacketInfo("ItemPutting", PlaceItem);
			methods[0x00BE] = new PacketInfo("MoveBegined", MoveToLocation);
			methods[0x00BF] = new PacketInfo("MoveEnded", ArrivedAtLocation);
			methods[0x00C0] = new PacketInfo("MoveChanged", AlterDestination);
			methods[0x00C2] = new PacketInfo("MoveTilePos", ChangeMapCell);
			methods[0x00C3] = new PacketInfo("MessageEvnt", MessageEvent);
			methods[0x00F4] = new PacketInfo("WarpCommand", Warp);
			methods[0x0136] = new PacketInfo("SkillToActs", PerformAction);
			methods[0x0142] = new PacketInfo("ChangeStyle", ChangeStyle);
			methods[0x0144] = new PacketInfo("ChargeInfo", ChargeInfo);
			methods[0x0189] = new PacketInfo("LoudMsgChannel", SayInShout);
			methods[0x01A2] = new PacketInfo("QueryCashItem", QueryCashItem);
			methods[0x01AE] = new PacketInfo("Mail", Mail);
			methods[0x01D0] = new PacketInfo("ServerEnv", ServerEnv);
			methods[0x0320] = new PacketInfo("CheckUserPrivacyData", CheckUserPrivacyData);
			methods[0x0408] = new PacketInfo("UnknownPacket1", UnknownPacket1);

			//constructors[""] = ;
			constructors["ItemDisposed"] = PC_ItemDisposed;
			constructors["ItemDropped"] = PC_ItemDropped;
			constructors["MobSpawned"] = PC_MobSpawned;
			constructors["UpdateStats"] = PC_UpdateStats;
		}
	}
}