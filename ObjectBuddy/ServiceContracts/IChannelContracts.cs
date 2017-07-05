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
using System.ServiceModel;

#endregion

namespace Minerva
{
	[ServiceContract]
	public interface IChannelContracts
	{
		[OperationContract]
		int GetUser(ulong magic);

		[OperationContract]
		bool AddChannel(byte server, byte channel, int type, uint ip, short port, short maxPlayers);

		[OperationContract]
		bool UpdatePlayerCount(byte server, byte channel, short players);
	}
}