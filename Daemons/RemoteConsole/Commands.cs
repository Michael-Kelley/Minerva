#region Includes

using System.Collections;

#endregion

namespace Minerva
{
	public partial class Server
	{
		[TelnetCommand("help", "Displays this help.")]
		public void TCHelp()
		{
			telnet.WriteLine();
			telnet.Write(TelnetForeColour.DarkGrey);
			telnet.WriteLine("Available commands:");

			IDictionaryEnumerator commands = telnet.Descriptors.GetEnumerator();
			var commandsArray = new ArrayList();

			while (commands.MoveNext())
				commandsArray.Add(((TelnetCommandDescriptor)commands.Value).FunctionHeader);

			commandsArray.Sort();

			foreach (var s in commandsArray)
				telnet.WriteLine(s);

			telnet.Flush();
		}

		[TelnetCommand("helpcmd", "Displays help for a given command.", "The command to get help for")]
		public void TCHelp(string command)
		{
			if (!telnet.Commands.ContainsKey(command))
			{
				telnet.WriteLine();
				telnet.Write(TelnetForeColour.Red);
				telnet.WriteLine("No such command: " + command);
				telnet.Flush();
				return;
			}

			var descriptor = (TelnetCommandDescriptor)telnet.Descriptors[command];
			telnet.WriteLine();
			telnet.Write(TelnetForeColour.DarkGrey);
			string[] descs = descriptor.FullDescription.Split('\n');
			telnet.WriteLine(descs[0]);
			for (int i = 1; i < descs.Length; i++)
				telnet.WriteLine(descs[i]);
			telnet.Flush();
		}

		[TelnetCommand("cls", "Clears the screen.")]
		public void TCCls() { telnet.Clear(); }

		[TelnetCommand("quit", "Closes the current telnet session.")]
		public void TCQuit() { telnet.Quit = true; }

		[TelnetCommand("echo", "Echoes an integer.", "Number to echo.")]
		public void TCEcho(int number) { telnet.WriteLine(number); }
	}
}