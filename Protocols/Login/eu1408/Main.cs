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
			methods[0x65] = new PacketInfo("Connect2Svr", Connect);
			methods[0x7A] = new PacketInfo("CheckVersion", CheckVersion);
            methods[0x7d2] = new PacketInfo("AuthAccount", AuthAccount);
            
		}

	}
}