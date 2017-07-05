#region Includes

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

#endregion

namespace Minerva
{
	public partial class Server
	{
		#region Private Fields

		TcpListener listener;
		Thread thread;

		TcpClient client;
		NetworkStream stream;

		TelnetHandler telnet;

		#endregion

		public Server()
		{
			Console.Title = ".:: Minerva - Cabal Online Sandbox - Remote Console Server ::.";
			Console.WindowHeight = 28;
			Console.WindowWidth = 80;
			Console.CursorVisible = false;

			int start = Environment.TickCount;

			telnet = new TelnetHandler();

			Util.Info.PrintLogo();
			Console.WriteLine();
			Util.Info.PrintInfo();
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.DarkGreen;

			Console.WriteLine("Registering telnet commands...");
			telnet.Register();

			try
			{
				Configuration.Load("RConSvr");

				listener = new TcpListener(Configuration.IP, Configuration.Port);
				thread = new Thread(new ThreadStart(ListenForClients));

				thread.Start();

				Log.Notice("Minerva started in: {0} seconds", (float)(Environment.TickCount - start) / 1000.0f);
			}
			catch (Exception e) { Log.Error(e.Message); throw e; }
		}

		private void ListenForClients()
		{
			listener.Start();

			Log.Notice("RConSvr listening for clients on {0}", listener.LocalEndpoint);

			while (true)
			{
				// blocks until a client has connected to the server
				TcpClient _client = this.listener.AcceptTcpClient();

				Log.Notice("Client {0} connected to RConSvr", _client.Client.RemoteEndPoint);

				// Create a thread to handle communication with connected client
				var clientThread = new Thread(new ParameterizedThreadStart(HandleComm));
				clientThread.Start(_client);
			}
		}

		private void HandleComm(object _client)
		{
			client = (TcpClient)_client;
			stream = client.GetStream();
			telnet.Writer = new StreamWriter(stream);
			telnet.Reader = new StreamReader(stream);

			byte[] message = new byte[4096];
			int bytesRead;

			while (true)
			{
				bytesRead = 0;

				try
				{
					// blocks until a client sends a message
					bytesRead = stream.Read(message, 0, 4096);
				}
				catch (Exception e)
				{
					// a socket error has occured
					Console.WriteLine(e.Message);
					break;
				}

				if (bytesRead == 0)
				{
					// the client has disconnected from the server
					Log.Notice("Client {0} disconnected from Login Server", client.Client.RemoteEndPoint);
					break;
				}

				// message has successfully been received
				byte[] msg = new byte[bytesRead];
				Array.ConstrainedCopy(message, 0, msg, 0, bytesRead);

				foreach (var m in msg)
					Console.Write(m.ToString("X2") + ", ");

				Console.WriteLine();
			}

			telnet.Clear();
			telnet.Prompt("Username:");

			if (telnet.Command == null)
			{
				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Log.Notice("Client {0} disconnected from RConSvr", client.Client.RemoteEndPoint);
				client.Close();
			}
			else
			{
				if (telnet.Command == Configuration.User)
				{
					telnet.Clear();
					telnet.Prompt("Password:");

					if (telnet.Command == null)
					{
						Console.ForegroundColor = ConsoleColor.DarkGreen;
						Log.Notice("Client {0} disconnected from RConSvr", client.Client.RemoteEndPoint);
						client.Close();
						return;
					}

					if (telnet.Command == Configuration.Pass)
					{
						telnet.Clear();
						telnet.Write(TelnetForeColour.Yellow);
						telnet.WriteLine(String.Format("Welcome, {0}!", Configuration.User));
						telnet.WriteLine();
						telnet.WriteLine("Type 'help' for a list of commands.");
						telnet.Flush();

						while (telnet.Quit != true)
						{
							telnet.Prompt();
							telnet.Parse(this);
						}

						Console.ForegroundColor = ConsoleColor.DarkGreen;
						Log.Notice("Client {0} disconnected from RConSvr", client.Client.RemoteEndPoint);
						client.Close();
					}
					else
					{
						telnet.Error("Incorrect password.");
						Log.Error(String.Format("RCon Client {0} failed to login.", client.Client.RemoteEndPoint));
						telnet.Prompt("Press any key to close this application");
						client.Close();
					}
				}
				else
				{
					telnet.Error("Incorrect username.");
					Log.Error(String.Format("RCon Client {0} failed to login.", client.Client.RemoteEndPoint));
					telnet.Prompt("Press any key to close this application");
					client.Close();
				}
			}
		}
	}
}