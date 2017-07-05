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
	class Incoming
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VerifyLinks
		{
			ushort header;
			ushort size;
			int padding;
			ushort opcode;
			public uint Timestamp;
			public ushort Count;
			byte unk0;
			byte unk1;
			uint unk2;
		}
	}
}