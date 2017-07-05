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

using System.Runtime.InteropServices;

#endregion

namespace Minerva
{
	public class Outgoing
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VerifyLinks
		{
			ushort header;
			ushort size;
			ushort opcode;
			public byte Server;
			public byte Channel;

			public VerifyLinks(bool init)
			{
				this = new VerifyLinks();
				header = 0xB7E2;
				size = 0x08;
				opcode = 0x66;
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct AuthAccount
		{
			ushort header;
			public ushort Size;
			ushort opcode;
			public AccountStatus Status;
			unsafe fixed byte p0[0x3F];

			public AuthAccount(bool init)
			{
				this = new AuthAccount();
				header = 0xB7E2;
				Size = 0x46;
				opcode = 0x67;
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct CheckVersion
		{
			ushort header;
			public ushort Size;
			ushort opcode;
			public uint Version;
			unsafe fixed byte p0[0x0C];

			public CheckVersion(bool init)
			{
				this = new CheckVersion();
				header = 0xB7E2;
				Size = 0x16;
				opcode = 0x7A;
			}
		}
	}
}