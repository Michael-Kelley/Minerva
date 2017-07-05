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

using Channel = System.Tuple<int, uint, short, short, short>;
using Character = System.Tuple<uint, byte, string, uint, byte, byte, byte, System.Tuple<byte, bool, byte, byte, byte>>;
using CharacterEquipment = System.Collections.Generic.List<byte[]>;
using CharacterStats = System.Tuple<ushort, ushort, ushort, ushort, ushort, ushort, ulong, System.Tuple<uint, uint, uint, uint, uint, byte, ushort, System.Tuple<ushort, byte, ushort, ushort, ulong>>>;
using CharacterItems = System.Collections.Generic.List<System.Tuple<byte[], int, short>>;

#endregion

namespace Minerva
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class PipeServer : ILoginContracts, IChannelContracts, IChatContracts, IDatabaseContracts
	{
		Dictionary<ulong, int> users;
		SortedDictionary<byte, SortedDictionary<byte, Channel>> channels;

		DatabaseHandler logindb;
		Dictionary<int, DatabaseHandler> serverdbs;

		Dictionary<int, List<int>> initialCharStats;
		Dictionary<int, List<Tuple<byte[], int, byte>>> initialCharItems;
		Dictionary<int, List<int[]>> initialCharSkills;
		Dictionary<int, List<int[]>> initialCharQuickslots;

		public PipeServer(Dictionary<int, List<int>> stats, Dictionary<int, List<Tuple<byte[], int, byte>>> items, Dictionary<int, List<int[]>> skills, Dictionary<int, List<int[]>> quickslots)
		{
			initialCharStats = stats;
			initialCharItems = items;
			initialCharSkills = skills;
			initialCharQuickslots = quickslots;

			if (initialCharItems.ContainsKey(-1))
			{
				foreach (var i in initialCharItems)
					if (i.Key != -1)
						i.Value.AddRange(initialCharItems[-1]);

				initialCharItems.Remove(-1);
			}

			if (initialCharSkills.ContainsKey(-1))
			{
				foreach (var i in initialCharSkills)
					if (i.Key != -1)
						i.Value.AddRange(initialCharSkills[-1]);

				initialCharSkills.Remove(-1);
			}

			if (initialCharQuickslots.ContainsKey(-1))
			{
				foreach (var i in initialCharQuickslots)
					if (i.Key != -1)
						i.Value.AddRange(initialCharQuickslots[-1]);

				initialCharQuickslots.Remove(-1);
			}
		}

		public void RunPipe()
		{
			users = new Dictionary<ulong, int>();
			channels = new SortedDictionary<byte, SortedDictionary<byte, Channel>>();

			var login = new ServiceHost(this, new Uri[] { new Uri(String.Format("net.tcp://{0}:9001", Configuration.IP)) });
			var channel = new ServiceHost(this, new Uri[] { new Uri(String.Format("net.tcp://{0}:9002", Configuration.IP)) });
			var chat = new ServiceHost(this, new Uri[] { new Uri(String.Format("net.tcp://{0}:9003", Configuration.IP)) });
			var database = new ServiceHost(this, new Uri[] { new Uri(String.Format("net.tcp://{0}:9004", Configuration.IP)) });

			login.AddServiceEndpoint(typeof(ILoginContracts), new NetTcpBinding(SecurityMode.None), "Login");
			channel.AddServiceEndpoint(typeof(IChannelContracts), new NetTcpBinding(SecurityMode.None), "Channel");
			chat.AddServiceEndpoint(typeof(IChatContracts), new NetTcpBinding(SecurityMode.None), "Chat");
			database.AddServiceEndpoint(typeof(IDatabaseContracts), new NetTcpBinding(SecurityMode.None), "Database");

			login.Open();
			channel.Open();
			database.Open();

			Log.Message("Connecting to Login Database...", Log.DefaultFG);
			logindb = new DatabaseHandler(Configuration.LoginDBType, Configuration.LoginDBIP, Configuration.LoginDB, Configuration.LoginDBUser, Configuration.LoginDBPass);

			serverdbs = new Dictionary<int, DatabaseHandler>();
		}

		// Login

		public bool AddUser(ulong magic, int account)
		{
			lock (users)
			{
				if (users.ContainsKey(magic))
					return false;

				users.Add(magic, account);

				Log.Notice("User added: " + magic.ToString("X2"));

				return true;
			}
		}

		public SortedDictionary<byte, SortedDictionary<byte, Channel>> GetChannels()
		{
			lock (channels)
			{
				return channels;
			}
		}

		// Channel

		public int GetUser(ulong magic)
		{
			lock (users)
			{
				if (!users.ContainsKey(magic))
					return -1;

				var result = users[magic];
				users.Remove(magic);

				Log.Notice("User retrieved: " + magic.ToString("X2"));

				return result;
			}
		}

		public bool AddChannel(byte server, byte channel, int type, uint ip, short port, short maxPlayers)
		{
			lock (channels)
			{
				if (!channels.ContainsKey(server))
					channels.Add(server, new SortedDictionary<byte, Channel>());

				var s = channels[server];

				if (s.ContainsKey(channel))
					return false;

				s.Add(channel, new Channel(type, ip, port, maxPlayers, 0));

				if (!Configuration.ServerDBs.ContainsKey(server))
				{
					Configuration.LoadServer(server);

					Log.Message("Connecting to Database for Server " + server.ToString() + "...", Log.DefaultFG);
					serverdbs[server] = new DatabaseHandler(Configuration.ServerDBTypes[server], Configuration.ServerDBIPs[server], Configuration.ServerDBs[server], Configuration.ServerDBUsers[server], Configuration.ServerDBPasses[server]);
				}


				Log.Notice(String.Format("Channel registered: {0}, {1}", server, channel));

				return true;
			}
		}

		public bool UpdatePlayerCount(byte server, byte channel, short players)
		{
			lock (channels)
			{
				if (!channels.ContainsKey(server))
					return false;

				var s = channels[server];

				if (!s.ContainsKey(channel))
					return false;

				var c = s[channel];
				s[channel] = new Channel(c.Item1, c.Item2, c.Item3, c.Item4, players);

				Log.Notice(String.Format("Player count updated for channel {0}, {1}", server, channel));

				return true;
			}
		}

		// Chat

		public void Temp()
		{
			throw new NotImplementedException();
		}

		// Login DB

		public Tuple<string, int, bool, int> FetchAccount(string user)
		{
			if (logindb.FetchAccount(user))
			{
				logindb.ReadRow();

				return Tuple.Create(logindb["hash"].ToString(),
									(int)(logindb["auth"] as byte?),
									(logindb["online"] as bool?).Value,
									(logindb["id"] as int?).Value);
			}

			return null;
		}

		public bool VerifyPassword(int server, int account, string hash)
			{ return logindb.VerifyPassword(account, hash); }

		// Server DB

		public Tuple<Character, CharacterEquipment, CharacterStats, CharacterItems, List<int[]>, List<int[]>> GetFullCharacter(int server, int account, int slot)
		{
			serverdbs[server].GetFullCharacter(account, slot);
			serverdbs[server].ReadRow();

			var character = new Character((uint)(serverdbs[server]["id"] as int?).Value,
										  (serverdbs[server]["slot"] as byte?).Value,
										  serverdbs[server]["name"] as string,
										  (uint)(serverdbs[server]["lv"] as int?).Value,
										  (serverdbs[server]["class"] as byte?).Value,
										  (serverdbs[server]["face"] as byte?).Value,
										  (serverdbs[server]["hair"] as byte?).Value,
										  Tuple.Create((serverdbs[server]["colour"] as byte?).Value,
													   (serverdbs[server]["gender"] as bool?).Value,
													   (serverdbs[server]["map"] as byte?).Value,
													   (serverdbs[server]["x"] as byte?).Value,
													   (serverdbs[server]["y"] as byte?).Value));

			var equipment = new CharacterEquipment();

			equipment.Add(serverdbs[server]["head"] as byte[]);
			equipment.Add(serverdbs[server]["body"] as byte[]);
			equipment.Add(serverdbs[server]["hands"] as byte[]);
			equipment.Add(serverdbs[server]["feet"] as byte[]);
			equipment.Add(serverdbs[server]["righthand"] as byte[]);
			equipment.Add(serverdbs[server]["lefthand"] as byte[]);
			equipment.Add(serverdbs[server]["back"] as byte[]);

			var stats = new CharacterStats((ushort)(serverdbs[server]["curhp"] as short?).Value,
										   (ushort)(serverdbs[server]["maxhp"] as short?).Value,
										   (ushort)(serverdbs[server]["curmp"] as short?).Value,
										   (ushort)(serverdbs[server]["maxmp"] as short?).Value,
										   (ushort)(serverdbs[server]["cursp"] as short?).Value,
										   (ushort)(serverdbs[server]["maxsp"] as short?).Value,
										   (ulong)(serverdbs[server]["xp"] as long?).Value,
										   new Tuple<uint, uint, uint, uint, uint, byte, ushort, Tuple<ushort, byte, ushort, ushort, ulong>>(
											   (uint)(serverdbs[server]["str"] as int?).Value,
											   (uint)(serverdbs[server]["int"] as int?).Value,
											   (uint)(serverdbs[server]["dex"] as int?).Value,
											   (uint)(serverdbs[server]["honour"] as int?).Value,
											   (uint)(serverdbs[server]["rank"] as int?).Value,
											   (serverdbs[server]["swordrank"] as byte?).Value,
											   (ushort)(serverdbs[server]["swordxp"] as int?).Value,
											   Tuple.Create((ushort)(serverdbs[server]["swordpoints"] as int?).Value,
															(serverdbs[server]["magicrank"] as byte?).Value,
															(ushort)(serverdbs[server]["magicxp"] as int?).Value,
															(ushort)(serverdbs[server]["magicpoints"] as int?).Value,
															(ulong)(serverdbs[server]["alz"] as long?).Value)));

			serverdbs[server].GetInventory((int)character.Item1);

			var items = new CharacterItems();

			while (serverdbs[server].ReadRow())
				items.Add(Tuple.Create(serverdbs[server]["item"] as byte[],
									   (serverdbs[server]["amount"] as int?).Value,
									   (serverdbs[server]["slot"] as short?).Value));

			serverdbs[server].GetSkills((int)character.Item1);

			var skills = new List<int[]>();

			while (serverdbs[server].ReadRow())
				skills.Add(new[] { (int)(serverdbs[server]["skill"] as short?).Value,
								   (int)(serverdbs[server]["level"] as byte?).Value,
								   (int)(serverdbs[server]["slot"] as byte?).Value });

			serverdbs[server].GetQuickslots((int)character.Item1);

			var quickslots = new List<int[]>();

			while (serverdbs[server].ReadRow())
				quickslots.Add(new[] { (int)(serverdbs[server]["skill"] as byte?).Value,
								       (int)(serverdbs[server]["slot"] as byte?).Value });

			return Tuple.Create(character, equipment, stats, items, skills, quickslots);
		}

		public List<Tuple<Character, CharacterEquipment>> GetCharacters(int server, int account)
		{
			var result = new List<Tuple<Character, CharacterEquipment>>();

			serverdbs[server].GetCharacters(account);

			while (serverdbs[server].ReadRow())
			{
				var character = new Character((uint)(serverdbs[server]["id"] as int?).Value,
											  (serverdbs[server]["slot"] as byte?).Value,
											  serverdbs[server]["name"] as string,
											  (uint)(serverdbs[server]["lv"] as int?).Value,
											  (serverdbs[server]["class"] as byte?).Value,
											  (serverdbs[server]["face"] as byte?).Value,
											  (serverdbs[server]["hair"] as byte?).Value,
											  Tuple.Create((serverdbs[server]["colour"] as byte?).Value,
														   (serverdbs[server]["gender"] as bool?).Value,
														   (serverdbs[server]["map"] as byte?).Value,
														   (serverdbs[server]["x"] as byte?).Value,
														   (serverdbs[server]["y"] as byte?).Value));

				var equipment = new CharacterEquipment();

				equipment.Add(serverdbs[server]["head"] as byte[]);
				equipment.Add(serverdbs[server]["body"] as byte[]);
				equipment.Add(serverdbs[server]["hands"] as byte[]);
				equipment.Add(serverdbs[server]["feet"] as byte[]);
				equipment.Add(serverdbs[server]["righthand"] as byte[]);
				equipment.Add(serverdbs[server]["lefthand"] as byte[]);
				equipment.Add(serverdbs[server]["back"] as byte[]);

				result.Add(Tuple.Create(character, equipment));
			}

			return result;
		}

		/*public List<byte[]> GetEquipment(int server, int character)
		{
			var result = new List<byte[]>();

			serverdbs[server].GetEquipment(character);
			serverdbs[server].ReadRow();

			result.Add(serverdbs[server]["head"] as byte[]);
			result.Add(serverdbs[server]["body"] as byte[]);
			result.Add(serverdbs[server]["hands"] as byte[]);
			result.Add(serverdbs[server]["feet"] as byte[]);
			result.Add(serverdbs[server]["righthand"] as byte[]);
			result.Add(serverdbs[server]["lefthand"] as byte[]);
			result.Add(serverdbs[server]["back"] as byte[]);

			return result;
		}

		public CharacterStats GetStats(int server, int character)
		{
			serverdbs[server].GetStats(character);
			serverdbs[server].ReadRow();

			return new CharacterStats((ushort)(serverdbs[server]["curhp"] as short?).Value,
									  (ushort)(serverdbs[server]["maxhp"] as short?).Value,
									  (ushort)(serverdbs[server]["curmp"] as short?).Value,
									  (ushort)(serverdbs[server]["maxmp"] as short?).Value,
									  (ushort)(serverdbs[server]["cursp"] as short?).Value,
									  (ushort)(serverdbs[server]["maxsp"] as short?).Value,
									  (ulong)(serverdbs[server]["xp"] as long?).Value,
									  new Tuple<uint, uint, uint, uint, uint, byte, ushort, Tuple<ushort, byte, ushort, ushort, ulong>>(
										  (uint)(serverdbs[server]["str"] as int?).Value,
										  (uint)(serverdbs[server]["int"] as int?).Value,
										  (uint)(serverdbs[server]["dex"] as int?).Value,
										  (uint)(serverdbs[server]["honour"] as int?).Value,
										  (uint)(serverdbs[server]["rank"] as int?).Value,
										  (serverdbs[server]["swordrank"] as byte?).Value,
										  (ushort)(serverdbs[server]["swordxp"] as int?).Value,
										  Tuple.Create((ushort)(serverdbs[server]["swordpoints"] as int?).Value,
													   (serverdbs[server]["magicrank"] as byte?).Value,
													   (ushort)(serverdbs[server]["magicxp"] as int?).Value,
													   (ushort)(serverdbs[server]["magicpoints"] as int?).Value,
													   (ulong)(serverdbs[server]["alz"] as long?).Value)));

		}

		public void GetInventory(int server, int character)
		{
			throw new NotImplementedException();
		}*/

		public int[] CreateCharacter(int server, int account, byte slot, string name, byte _class, bool gender, byte face, byte hair, byte colour)
		{
			var stats = initialCharStats[_class];
			var items = initialCharItems[_class];
			var skills = initialCharSkills[_class];
			var quickslots = initialCharQuickslots[_class];			

			return serverdbs[server].CreateCharacter(account, slot, name, _class, gender, face, hair, colour, stats.ToArray(), items, skills, quickslots);
		}

		public byte DeleteCharacter(int server, int account, int slot)
		{
			return serverdbs[server].DeleteCharacter(account, slot);
		}

		public void UpdateCharacterPosition(int server, int account, int slot, byte map, byte x, byte y)
		{
			serverdbs[server].UpdateCharacterPosition(account, slot, map, x, y);
		}

		public void MoveItem(int server, int characterId, int oldslot, int newslot)
		{
			serverdbs[server].MoveItem(characterId, oldslot, newslot);
		}

		public void RemoveItem(int server, int characterId, int slot)
		{
			serverdbs[server].RemoveItem(characterId, slot);
		}

		public void AddItem(int server, int characterId, int slot, byte[] item, int amount = 0)
		{
			serverdbs[server].AddItem(characterId, slot, item, amount);
		}

		public Tuple<byte[], int> GetItem(int server, int characterId, int slot)
		{
			serverdbs[server].GetItem(characterId, slot);
			serverdbs[server].ReadRow();

			return Tuple.Create(serverdbs[server]["item"] as byte[],
								(serverdbs[server]["amount"] as int?).Value);
		}

		public void EquipItem(int server, int characterId, int itemslot, string equipslot)
		{
			serverdbs[server].EquipItem(characterId, itemslot, equipslot);
		}
	}
}