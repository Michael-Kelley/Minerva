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
using System.Threading;

#endregion

namespace Minerva
{
	public class Log
	{
		// The default background colour to use when outputting text to the console.
		public static ConsoleColor DefaultBG = ConsoleColor.Black;
		// The default foreground colour to use when outputting text to the console.
		public static ConsoleColor DefaultFG = ConsoleColor.DarkGreen;
		// The colour to use when displaying an error message in the console.
		static ConsoleColor error = ConsoleColor.Red;
		// The colour to use when displaying a warning message in the console.
		static ConsoleColor warning = ConsoleColor.Yellow;
		// The colour to use when displaying a notice message in the console.
		static ConsoleColor notice = ConsoleColor.Green;

		static Queue<LogMessage> messages = new Queue<LogMessage>();
		static bool stopped = false;

		static void Listen()
		{
			while (!stopped)
			{
				if (messages.Count > 0)
					lock (messages)
						Message(messages.Dequeue());
				else
					Thread.Sleep(150);
			}
		}

		static void Message(LogMessage m)
		{
			string message = m.Message;
			ConsoleColor colour = m.Colour;
			string tag = m.Tag;

			bool bright = !((int)colour < 8);
			Console.BackgroundColor = !bright ? (ConsoleColor)((int)colour | 8) : (ConsoleColor)((int)colour ^ 8);
			Console.ForegroundColor = !bright ? ConsoleColor.Black : ConsoleColor.White;
			if (tag != "") Console.Write(" ##{0}## ", tag);
			Console.BackgroundColor = DefaultBG;
			Console.ForegroundColor = colour;
			Console.Write((tag != "") ? " {0}\n" : "{0}\n", message);
			Console.ForegroundColor = DefaultFG;
		}

		public static void Received(string type, int opcode, int size)
			{ Message(String.Format("Received packet: CSC_{0} (opcode: {1}, size: {2})", type, opcode, size), DefaultFG); }

		public static void Sent(string type, int opcode, int size)
			{ Message(String.Format("Sent packet: SSC_{0} (opcode: {1}, size: {2})", type, opcode, size), DefaultFG); }

		public static void Error(string message)
			{ Message(message, error, "ERROR"); }
		public static void Error(string format, params object[] arg)
			{ Error(String.Format(format, arg)); }

		public static void Notice(string message)
			{ Message(message, notice, "NOTICE"); }
		public static void Notice(string format, params object[] arg)
			{ Notice(String.Format(format, arg)); }

		public static void Warning(string message)
			{ Message(message, warning, "WARNING"); }
		public static void Warning(string format, params object[] arg)
			{ Warning(String.Format(format, arg)); }

		public static void Message(string message, ConsoleColor colour, string tag = "")
		{
			lock (messages)
				messages.Enqueue(new LogMessage(message, colour, tag));
		}

		public static void Start()
		{
			var t = new Thread(new ThreadStart(Listen));
			t.Start();
		}

		public static void Stop()
			{ stopped = true; }
	}

	public struct LogMessage
	{
		public string Message;
		public ConsoleColor Colour;
		public string Tag;

		public LogMessage(string message, ConsoleColor colour, string tag = "")
		{
			Message = message;
			Colour = colour;
			Tag = tag;
		}
	}
}