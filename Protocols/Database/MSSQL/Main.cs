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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

#endregion

namespace Minerva
{
	public class DatabaseProtocol : IDatabaseProtocol
	{
		SqlConnection sql;
		SqlDataReader reader;

		public DatabaseProtocol(string ip, string db, string user, string pass)
		{
			sql = new SqlConnection(String.Format("Server={0};Database={1};User={2};Password={3}",
												  ip, db, user, pass));
		}

		void Cleanup()
		{
			if (reader != null && !reader.IsClosed)
				reader.Close();
		}

		public void Connect()
			{ sql.Open(); }

		public void Disconnect()
		{
			reader.Close();
			reader.Dispose();
			reader = null;

			sql.Close();
			sql.Dispose();
			sql = null;
		}

		public object this[string key]
			{ get { return reader[key]; } }

		public bool ReadRow()
			{ return reader.Read(); }

		public void RebuildIndices()
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = "exec sp_MSforeachtable 'ALTER INDEX ALL ON ? REBUILD'";
			cmd.ExecuteNonQuery();
			cmd.Dispose();
		}

		public bool FetchAccount(string user)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT * FROM [dbo].[accounts] WHERE [username] LIKE '{0}'", user);
			reader = cmd.ExecuteReader();
			cmd.Dispose();

			return reader.HasRows;
		}

		public bool VerifyPassword(int account, string hash)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT hash from [dbo].[accounts] WHERE [id] = {0}", account);
			var dbresult = cmd.ExecuteScalar();

			if (dbresult != null && (string)dbresult == hash)
				return true;

			cmd.Dispose();

			return false;
		}

		public byte DeleteCharacter(int account, int slot)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			var trans = sql.BeginTransaction("DeleteCharacter");
			cmd.Transaction = trans;

			cmd.CommandText = String.Format("SELECT id FROM [dbo].[characters] WHERE [slot] = {0} AND [account] = {1}", slot, account);
			var dbresult = cmd.ExecuteScalar();

			if (dbresult == null)
			{
				trans.Rollback();
				cmd.Dispose();

				return 1;	// Unknown DB error.
			}

			var id = (int)dbresult;

			try
			{
				cmd.CommandText = String.Format("INSERT INTO deleted_characters (account, slot, name, lv, class, gender, face, hair, colour, created, deleted) " +
												"SELECT account, slot, name, lv, class, gender, face, hair, colour, created, GETDATE() " +
												"FROM [dbo].[characters] WHERE [account] = {0} and [slot] = {1}",
												account, slot);
				cmd.ExecuteNonQuery();
				cmd.CommandText = String.Format("INSERT INTO deleted_equipment (head, body, hands, feet, lefthand, righthand, neck, finger1, finger2, leftear, rightear, leftwrist, rightwrist, back, card) " +
												"SELECT head, body, hands, feet, lefthand, righthand, neck, finger1, finger2, leftear, rightear, leftwrist, rightwrist, back, card " +
												"FROM [dbo].[characters_equipment] WHERE [id] = {0}",
												id);
				cmd.ExecuteNonQuery();
				cmd.CommandText = String.Format("DELETE characters_stats WHERE id = {0}; " +
												"DELETE characters_quickslots WHERE id = {0}; " +
												"DELETE characters_skills WHERE id = {0}; " +
												"DELETE characters_items WHERE id = {0}; " +
												"DELETE characters_equipment WHERE id = {0}; " +
												"DELETE characters WHERE id = {0};",
												id);
				cmd.ExecuteNonQuery();

				trans.Commit();
				cmd.Dispose();

				return 0xA1;
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				trans.Rollback();
			}

			cmd.Dispose();

			return 0;
		}

		public void GetFullCharacter(int account, int slot)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT * FROM [dbo].[characters], [dbo].[characters_equipment], [dbo].[characters_stats] " +
											"WHERE [dbo].[characters].[account] = {0} AND [dbo].[characters].[slot] = {1} " +
											"AND [dbo].[characters].[id] = [dbo].[characters_equipment].[id] " +
											"AND [dbo].[characters].[id] = [dbo].[characters_stats].[id]",
											account, slot);
			reader = cmd.ExecuteReader();
			cmd.Dispose();
		}

		public void GetCharacters(int account)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT * FROM [dbo].[characters], [dbo].[characters_equipment] " +
											"WHERE [dbo].[characters].[account] = {0}" + 
											"AND [dbo].[characters].[id] = [dbo].[characters_equipment].[id]"
											, account);
			reader = cmd.ExecuteReader();
			cmd.Dispose();
		}

		public void GetStats(int characterId)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT * FROM [dbo].[characters_stats] WHERE [id] = {0}", characterId);
			reader = cmd.ExecuteReader();
			cmd.Dispose();
		}

		public void GetEquipment(int characterId)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT * FROM [dbo].[characters_equipment] WHERE [id] = {0}", characterId);
			reader = cmd.ExecuteReader();
			cmd.Dispose();
		}

		public void GetInventory(int characterId)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT item, amount, slot FROM [dbo].[characters_items] WHERE [id] = {0}", characterId);
			reader = cmd.ExecuteReader();
			cmd.Dispose();
		}

		public void GetSkills(int characterId)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT skill, level, slot FROM [dbo].[characters_skills] WHERE [id] = {0}", characterId);
			reader = cmd.ExecuteReader();
			cmd.Dispose();
		}

		public void GetQuickslots(int characterId)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT skill, slot FROM [dbo].[characters_quickslots] WHERE [id] = {0}", characterId);
			reader = cmd.ExecuteReader();
			cmd.Dispose();
		}

		public int[] CreateCharacter(int account, byte slot, string name, byte _class, bool gender, byte face, byte hair, byte colour, int[] initialStats, List<Tuple<byte[], int, byte>> initialItems, List<int[]> initialSkills, List<int[]> initialQuickslots)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			var trans = sql.BeginTransaction("CreateCharacter");
			cmd.Transaction = trans;

			// Check if the name is taken.
			cmd.CommandText = String.Format("SELECT id FROM [dbo].[characters] WHERE [name] = '{0}'", name);

			if (cmd.ExecuteScalar() != null)
			{
				trans.Rollback();
				cmd.Dispose();
				return new[] { 3, 0 };	// Character name already taken.
			}

			// Check if the character slot is taken.
			cmd.CommandText = String.Format("SELECT id FROM [dbo].[characters] WHERE [slot] = {0} AND [account] = {1}", slot, account);

			if (cmd.ExecuteScalar() != null)
			{
				trans.Rollback();
				cmd.Dispose();
				return new[] { 1, 0 };	// Character slot already taken.
			}

			try
			{
				cmd.CommandText = String.Format("INSERT INTO [dbo].[characters] (account, slot, name, lv, class, gender, face, hair, colour, map, x, y, created) " +
												"VALUES ({0}, {1}, '{2}', 1, {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, @created); " +
												"INSERT INTO [dbo].[characters_equipment] (body, hands, feet, lefthand, righthand) " +
												"VALUES (@body, @hands, @feet, @lefthand, @righthand); " + 
												"INSERT INTO [dbo].[characters_stats] (curhp, maxhp, curmp, maxmp, cursp, maxsp, xp, str, int, dex, honour, rank, swordrank, swordxp, swordpoints, magicrank, magicxp, magicpoints, alz) " +
												"VALUES ({11}, {11}, {12}, {12}, 0, 0, 0, {13}, {14}, {15}, 0, 1, 1, 0, 0, 1, 0, 0, 0); " +
												"SELECT SCOPE_IDENTITY();",
												account, slot, name, _class, gender, face, hair, colour, initialStats[0], initialStats[1], initialStats[2],
												initialStats[8], initialStats[9], initialStats[10], initialStats[11], initialStats[12]);
				cmd.Parameters.Add("@created", SqlDbType.DateTime).SqlValue = new System.Data.SqlTypes.SqlDateTime(DateTime.Now);;
				cmd.Parameters.Add("@body", SqlDbType.Binary, 15).Value = BitConverter.GetBytes(initialStats[3]);
				cmd.Parameters.Add("@hands", SqlDbType.Binary, 15).Value = BitConverter.GetBytes(initialStats[4]);
				cmd.Parameters.Add("@feet", SqlDbType.Binary, 15).Value = BitConverter.GetBytes(initialStats[5]);
				cmd.Parameters.Add("@lefthand", SqlDbType.Binary, 15).Value = (initialStats[7] != -1) ? BitConverter.GetBytes(initialStats[7]) : System.Data.SqlTypes.SqlBinary.Null;
				cmd.Parameters.Add("@righthand", SqlDbType.Binary, 15).Value = BitConverter.GetBytes(initialStats[6]);
				var id = (int)(decimal)cmd.ExecuteScalar();

				foreach (var i in initialItems)
				{
					cmd.CommandText = String.Format("INSERT INTO [dbo].[characters_items] " +
													"VALUES ({0}, @item{2}, {1}, {2})",
													id, i.Item2, i.Item3);
					cmd.Parameters.Add(String.Format("@item{0}", i.Item3), SqlDbType.Binary, 15).Value = i.Item1;
					cmd.ExecuteNonQuery();
				}

				foreach (var s in initialSkills)
				{
					cmd.CommandText = String.Format("INSERT INTO [dbo].[characters_skills] " +
													"VALUES ({0}, {1}, {2}, {3})",
													id, s[0], s[1], s[2]);
					cmd.ExecuteNonQuery();
				}

				foreach (var q in initialQuickslots)
				{
					cmd.CommandText = String.Format("INSERT INTO [dbo].[characters_quickslots] " +
													"VALUES ({0}, {1}, {2})",
													id, q[0], q[1]);
					cmd.ExecuteNonQuery();
				}

				trans.Commit();
				cmd.Dispose();

				return new[] { 161, id };	// Character creation succeeded.
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				trans.Rollback();
			}

			cmd.Parameters.Clear();
			cmd.Dispose();
			return new[] { 0, 0 };	// Unknown DB error.
		}

		public void UpdateCharacterPosition(int account, int slot, byte map, byte x, byte y)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("UPDATE [dbo].[characters] SET map = {0}, x = {1}, y = {2} WHERE [account] = {3} AND [slot] = {4}",
											map, x, y, account, slot);
			cmd.ExecuteNonQuery();
			cmd.Dispose();
		}

		public void MoveItem(int characterId, int oldslot, int newslot)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("UPDATE [dbo].[characters_items] SET slot = {1} WHERE [id] = {0} AND [slot] = {2}",
											characterId, newslot, oldslot);
			cmd.ExecuteNonQuery();
			cmd.Dispose();
		}

		public void RemoveItem(int characterId, int slot)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("DELETE [dbo].[characters_items] WHERE [id] = {0} AND [slot] = {1}",
											characterId, slot);
			cmd.ExecuteNonQuery();
			cmd.Dispose();
		}

		public void AddItem(int characterId, int slot, byte[] item, int amount)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("INSERT INTO [dbo].[characters_items] " +
											"VALUES ({0}, @item, {1}, {2})",
											characterId, amount, slot);
			cmd.Parameters.Add("@item", SqlDbType.Binary, 15).Value = item;
			cmd.ExecuteNonQuery();
			cmd.Dispose();
		}

		public void GetItem(int characterId, int slot)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT * FROM [dbo].[characters_items] WHERE [id] = {0} AND [slot] = {1}",
											characterId, slot);
			reader = cmd.ExecuteReader();
			cmd.Dispose();
		}

		public void EquipItem(int characterId, int itemslot, string equipslot)
		{
			Cleanup();

			var cmd = sql.CreateCommand();
			cmd.CommandText = String.Format("SELECT item FROM [dbo].[characters_items] WHERE id = {0} AND slot = {1}",
											characterId, itemslot);
			var item = cmd.ExecuteScalar() as byte[];
			cmd.CommandText = String.Format("UPDATE [dbo].[characters_equipment] SET {1} = @item " +
											"WHERE [id] = {0}; " +
											"DELETE [dbo].[characters_items] WHERE id = {0} AND slot = {2};",
											characterId, equipslot, itemslot);
			cmd.Parameters.Add("@item", SqlDbType.Binary, 15).Value = item;
			cmd.ExecuteNonQuery();
			cmd.Dispose();
		}
	}
}