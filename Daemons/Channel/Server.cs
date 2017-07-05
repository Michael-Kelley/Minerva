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
using System.Xml.Linq;

#endregion

namespace Minerva
{
	class Server
	{
		TcpListener listener;
		Thread thread;

		Dictionary<ulong, ClientHandler> clients;
		Dictionary<int, Map> maps;

		PacketHandler packets;
		EventHandler events;
		//ScriptHandler scripts;

		int ticks = Environment.TickCount;
		int count = 1;

		int server, channel;

		IChannelContracts proxy;
		IDatabaseContracts database;

		public Server(int server, int channel)
		{
#if DEBUG
			Thread.Sleep(5000);
#endif

			this.server = server;
			this.channel = channel;

			Console.Title = ".:: Minerva - Cabal Online Sandbox - Channel Server ::.";
			Console.CursorVisible = false;

			int start = Environment.TickCount;

			Log.Start();	// Start logging service

			clients = new Dictionary<ulong, ClientHandler>();
			events = new EventHandler();
			events.OnClientDisconnect += (sender, client) => { Log.Notice("Client {0} disconnected from Channel Server", client.RemoteEndPoint); clients.Remove((ulong)client.Metadata["magic"]); };
			events.OnError += (sender, message) => { Log.Error(message); };
			events.OnReceivePacket += (sender, e) => { Log.Received(e.Name, e.Opcode, e.Length); };
			events.OnSendPacket += (sender, e) => { Log.Sent(e.Name, e.Opcode, e.Length); };
			events.OnWarp += (sender, client, map, x, y) => { client.Metadata["map"] = maps[map]; maps[map].MoveClient(client, x / 16, y / 16); };

			Util.Info.PrintLogo();
			Console.WriteLine();
			Util.Info.PrintInfo();
			Console.WriteLine();
			Console.ForegroundColor = Log.DefaultFG;

			/*Console.WriteLine("Compiling and registering scripts...");
			scripts = new ScriptHandler();
			scripts.Concatenate("Events", new string[] { "mscorlib" });
			scripts.Run("Events");
			scripts.CreateInstance("Events");
			dynamic result = scripts.Invoke("_init_", events);*/

			try
			{
				Log.Message("Reading configuration...", Log.DefaultFG);
				Configuration.Load(String.Format("Channel_{0}_{1}", server, channel));

				Log.Message("Loading warp data...", Log.DefaultFG);
				var xml = XDocument.Load("data/cabal.xml");

				var node = xml.Root.Element("map");
				var deadReturn = new Dictionary<int, int[]>();

				foreach (var n in node.Elements("map_index"))
					deadReturn.Add(int.Parse(n.Attribute("world_id").Value), new[] { int.Parse(n.Attribute("dead_warp").Value),
																					 int.Parse(n.Attribute("return_warp").Value) });

				node = xml.Root.Element("warp_point");
				var warps = new List<WarpIndex>();

				foreach (var n in node.Elements("warp_index"))
					warps.Add(new WarpIndex(int.Parse(n.Attribute("x").Value),
											int.Parse(n.Attribute("y").Value),
											int.Parse(n.Attribute("nation1x").Value),
											int.Parse(n.Attribute("nation1y").Value),
											int.Parse(n.Attribute("nation2x").Value),
											int.Parse(n.Attribute("nation2y").Value),
											int.Parse(n.Attribute("w_code").Value),
											int.Parse(n.Attribute("Fee").Value),
											int.Parse(n.Attribute("WorldIdx").Value),
											int.Parse(n.Attribute("level").Value)));

				node = xml.Root.Element("warp_npc");

				var warpNPCs = new Dictionary<int, Dictionary<int, List<WarpList>>>();

				foreach (var n in node.Elements("world"))
				{
					var world = new Dictionary<int, List<WarpList>>();
					var npcs = n.Elements("npc");

					foreach (var npc in npcs)
					{
						var list = new List<WarpList>();
						var warpList = npc.Elements("warp_list");

						foreach (var w in warpList)
							list.Add(new WarpList(int.Parse(w.Attribute("order").Value),
												  int.Parse(w.Attribute("type").Value),
												  int.Parse(w.Attribute("target_id").Value),
												  int.Parse(w.Attribute("level").Value),
												  int.Parse(w.Attribute("Fee").Value),
												  (w.Attribute("warp_item").Value != "") ? int.Parse(w.Attribute("warp_item").Value.Split(':')[0]) : 0,
												  (w.Attribute("warp_item").Value != "") ? int.Parse(w.Attribute("warp_item").Value.Split(':')[1]): 0,
												  (w.Attribute("quest_id").Value != "") ? int.Parse(w.Attribute("quest_id").Value) : 0,
												  int.Parse(w.Attribute("gps_view").Value) == 1));

						world.Add(int.Parse(npc.FirstAttribute.Value), list);
					}

					warpNPCs.Add(int.Parse(n.FirstAttribute.Value), world);
				}

				Log.Message("Registering packets...", Log.DefaultFG);
				packets = new PacketHandler("world", Configuration.Protocol, events);

				var factory = new ChannelFactory<IChannelContracts>(new NetTcpBinding(SecurityMode.None),
																	new EndpointAddress(String.Format("net.tcp://{0}:9002/Channel", Configuration.MasterIP)));
				proxy = factory.CreateChannel();

				var dbfactory = new ChannelFactory<IDatabaseContracts>(new NetTcpBinding(SecurityMode.None),
																	   new EndpointAddress(String.Format("net.tcp://{0}:9004/Database", Configuration.MasterIP)));
				database = dbfactory.CreateChannel();

				Log.Notice("Created IPC channels");

				var aa = Configuration.IP.GetAddressBytes();
				var address = BitConverter.ToUInt32(aa, 0);

				proxy.AddChannel((byte)server, (byte)channel, 0, address, (short)Configuration.Port, (short)100);

				Log.Notice("Registered channel with Master");

				maps = new Dictionary<int, Map>();

				for (int i = 1; i < 31; i++)
					maps[i] = new Map(i, warps, (deadReturn.ContainsKey(i) ? deadReturn[i][0] : 1), (deadReturn.ContainsKey(i) ? deadReturn[i][1] : 1), (warpNPCs.ContainsKey(i) ? warpNPCs[i] : null));

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

		private void Listen()
		{
			listener.Start();

			Log.Notice("Channel Server listening for clients on {0}", listener.LocalEndpoint);

			while (true)
			{
				// blocks until a client has connected to the server
				TcpClient client = this.listener.AcceptTcpClient();

				Log.Notice("Client {0} connected to Channel Server", client.Client.RemoteEndPoint);

				int timestamp = Environment.TickCount - ticks;
				ulong key = ((ulong)count << 32) + (ulong)timestamp;
				var c = new ClientHandler(client, packets, events);
				c.Metadata["timestamp"] = (uint)timestamp;
				c.Metadata["count"] = (ushort)count++;
				c.Metadata["magic"] = key;
				c.Metadata["proxy"] = proxy;
				c.Metadata["database"] = database;
				c.Metadata["clients"] = clients;
				c.Metadata["server"] = server;
				c.Start();
				clients.Add(key, c);
			}
		}
	}
}