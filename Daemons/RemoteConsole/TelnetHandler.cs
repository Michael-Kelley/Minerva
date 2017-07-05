#region Includes

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#endregion

namespace Minerva
{
	class TelnetHandler
	{
		#region Private Fields

		string _command;
		Dictionary<string, MethodInfo> _commands;
		Dictionary<string, TelnetCommandDescriptor> _descriptors;

		string logo = String.Format(
@"{0}		   _____  _________ _		_______  ______			 _____
{1}		  /	 \ \__   __/| \	/||  ____ \|  ___ \ |\	 /| / ___ \
		 | || || |   | |   |  \  | || |	\/| |   \ || |   | || |   | |
{2}		 | || || |   | |   |   \ | || |__	| |___/ || |   | || |___| |
		 | ||_|| |   | |   | |\ \| ||  __|   |	 _/ | |   | ||  ___  |
{1}		 | |   | |   | |   | | \   || |	  | |\ \	\ \_/ / | |   | |
		 | |   | |___| |___| |  \  || |____/\| | \ \__  \   /  | |   | |
{0}		 |/	 \|\_______/|/	\_||_______/|/   \__/   \_/   |/	 \|

{3}==============================================================================={4}",
			TelnetForeColour.DarkGrey, TelnetForeColour.Grey, TelnetForeColour.White,
			TelnetForeColour.DarkYellow, TelnetForeColour.None);

		#endregion

		#region Public Fields

		public StreamReader Reader;
		public bool Quit;
		public StreamWriter Writer;

		#endregion

		#region Constructor

		public TelnetHandler()
		{
			_commands = new Dictionary<string, MethodInfo>();
			_descriptors = new Dictionary<string, TelnetCommandDescriptor>();
			Quit = false;
		}

		#endregion

		#region Accessors

		public string Command { get { return _command; } }
		public Dictionary<string, MethodInfo> Commands { get { return _commands; } }
		public Dictionary<string, TelnetCommandDescriptor> Descriptors { get { return _descriptors; } }

		#endregion

		#region Public Methods

		public void Clear()
		{
			Writer.Write(String.Format("{0}[1J{0}[H", (char)0x1B));
			WriteLogo();
			Writer.WriteLine();
			Writer.Flush();
		}

		public void Error(string error)
		{
			Writer.WriteLine("{0}{1}##ERROR##{2}{3} {4}{5}",
							  TelnetBackColour.Red, TelnetForeColour.White,
							  TelnetBackColour.None, TelnetForeColour.Red,
							  error, TelnetForeColour.None);
			Writer.Flush();
		}

		public void Flush()
		{
			Writer.Write(TelnetForeColour.None);
			Writer.Flush();
		}

		public void Parse(object sender)
		{
			if (_command == null)
				return;

			MethodInfo method = null;
			string[] c = _command.Split(' ');

			if (_commands.ContainsKey(c[0]))
			{
				method = _commands[c[0]];

				if (c.Length - 1 != method.GetParameters().Length)
				{
					Error("Incorrect command syntax.");
					_commands["helpcmd"].Invoke(sender, new object[] { c[0] });
				}
				else
				{
					if (c.Length == 1)
						method.Invoke(sender, null);
					else
					{
						List<object> parameters = new List<object>();
						for (int i = 1; i < c.Length; i++)
							parameters.Add(
								Convert.ChangeType(
									c[i], method.GetParameters()[i - 1].ParameterType));

						method.Invoke(sender, parameters.ToArray());
					}
				}
			}
			else
				Error("Unknown command.");
		}

		public void Prompt()
		{
			Writer.Write("> ");
			Writer.Flush();
			_command = Reader.ReadLine();
		}

		public void Prompt(string message)
		{
			Writer.WriteLine(message);
			Writer.Write("> ");
			Writer.Flush();
			_command = Reader.ReadLine();
		}

		public void Register()
		{
			var methods = typeof(Server).GetMethods();
			TelnetCommandAttribute command = null;

			foreach (MethodInfo method in methods)
			{
				command = Attribute.GetCustomAttribute(method, typeof(TelnetCommandAttribute), false) as TelnetCommandAttribute;

				if (command == null)
					continue;

				var parameters = method.GetParameters();
				var parameterNames = new List<string>();

				foreach (var p in parameters)
					parameterNames.Add(p.Name);

				var descriptor = new TelnetCommandDescriptor(command.Name, command.Description,
															 parameterNames.ToArray(), command.ParameterDescriptions);

				_commands.Add(command.Name, method);
				_descriptors.Add(command.Name, descriptor);
			}
		}

		public void Write(string message) { Writer.Write(message); }

		public void WriteLine() { Writer.WriteLine(); }
		public void WriteLine(int value) { Writer.WriteLine(value); }
		public void WriteLine(object value) { Writer.WriteLine(value); }
		public void WriteLine(string value) { Writer.WriteLine(value); }

		public void WriteLogo() { Writer.WriteLine(logo); }

		#endregion
	}
}
