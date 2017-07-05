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
	// Config-loading class
	public class Configuration
	{
		public static string IP;
		public static string LoginDB;
		public static string LoginDBIP;
		public static string LoginDBUser;
		public static string LoginDBPass;
		public static string LoginDBType;

		public static Dictionary<int, string> ServerDBs;
		public static Dictionary<int, string> ServerDBIPs;
		public static Dictionary<int, string> ServerDBUsers;
		public static Dictionary<int, string> ServerDBPasses;
		public static Dictionary<int, string> ServerDBTypes;

		public static void Load()
		{
			var conf = new IniReader("conf/Master.ini");

			IP = conf.GetValue("listen", "ip", "0.0.0.0");
			LoginDBType = conf.GetValue("logindb", "type", "");
			LoginDB = conf.GetValue("logindb", "name", "");
			LoginDBIP = conf.GetValue("logindb", "ip", "");
			LoginDBUser = conf.GetValue("logindb", "user", "");
			LoginDBPass = conf.GetValue("logindb", "password", "");

			ServerDBs = new Dictionary<int, string>();
			ServerDBIPs = new Dictionary<int, string>();
			ServerDBUsers = new Dictionary<int, string>();
			ServerDBPasses = new Dictionary<int, string>();
			ServerDBTypes = new Dictionary<int, string>();
		}

		public static void LoadServer(int server)
		{
			var conf = new IniReader("conf/Master.ini");
			var section = String.Format("server{0}db", server);

			ServerDBTypes.Add(server, conf.GetValue(section, "type", ""));
			ServerDBs.Add(server, conf.GetValue(section, "name", ""));
			ServerDBIPs.Add(server, conf.GetValue(section, "ip", ""));
			ServerDBUsers.Add(server, conf.GetValue(section, "user", ""));
			ServerDBPasses.Add(server, conf.GetValue(section, "password", ""));
		}
	}
}