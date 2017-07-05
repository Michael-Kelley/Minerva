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
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading;

#endregion

namespace Minerva
{
	class Server
	{
		TcpListener listener;
		Thread thread;

		HashSet<ClientHandler> clients;

		PacketHandler packets;
		EventHandler events;
		//ScriptHandler scripts;

		IChatContracts proxy;
		IDatabaseContracts database;

		public Server()
		{
			Console.Title = ".:: Minerva - Cabal Online Sandbox - Chat Server ::.";
			Console.CursorVisible = false;

			int start = Environment.TickCount;

			Log.Start();	// Start logging service

			clients = new HashSet<ClientHandler>();
			events = new EventHandler();
			events.OnClientDisconnect += (sender, client) => { Log.Notice("Client {0} disconnected from Chat Server", client.RemoteEndPoint); clients.Remove(client); };
			events.OnError += (sender, message) => Log.Error(message);
			events.OnReceivePacket += (sender, e) => Log.Received(e.Name, e.Opcode, e.Length);
			events.OnSendPacket += (sender, e) => Log.Sent(e.Name, e.Opcode, e.Length);

			Util.Info.PrintLogo();
			Console.WriteLine();
			Util.Info.PrintInfo();
			Console.WriteLine();
			Console.ForegroundColor = Log.DefaultFG;

			/*Log.Message("Compiling and registering scripts...", Log.DefaultFG);
			scripts = new ScriptHandler();
			scripts.Concatenate("Events", new string[] { "mscorlib" });
			scripts.Run("Events");
			scripts.CreateInstance("Events");
			dynamic result = scripts.Invoke("_init_", events);*/

			try
			{
				Log.Message("Reading configuration...", Log.DefaultFG);
				Configuration.Load("Chat");

				Log.Message("Registering packets...", Log.DefaultFG);
				packets = new PacketHandler("chat", Configuration.Protocol, events);

				var factory = new ChannelFactory<IChatContracts>(new NetTcpBinding(SecurityMode.None),
																 new EndpointAddress("net.tcp://localhost:9003/Chat"));
				proxy = factory.CreateChannel();

				var dbfactory = new ChannelFactory<IDatabaseContracts>(new NetTcpBinding(SecurityMode.None),
																	   new EndpointAddress("net.tcp://localhost:9004/Database"));
				database = dbfactory.CreateChannel();

				Log.Notice("Created IPC channels");

				listener = new TcpListener(System.Net.IPAddress.Any, Configuration.Port);
				thread = new Thread(Listen);
				thread.Start();

				Log.Notice("Minerva started in: {0} seconds", (float)(Environment.TickCount - start) / 1000.0f);
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
#if DEBUG
				throw e;
#endif
			}
		}

		void Listen()
		{
			listener.Start();

			Log.Notice("Chat Server listening for clients on {0}", listener.LocalEndpoint);

			while (true)
			{
				// Waits for a client to connect to the server
				TcpClient client = this.listener.AcceptTcpClient();

				Log.Notice("Client {0} connected to Chat Server", client.Client.RemoteEndPoint);

				var c = new ClientHandler(client, packets, events);
				c.Metadata["proxy"] = proxy;
				c.Metadata["database"] = database;
				c.Start();
				clients.Add(c);
			}
		}
	}
}