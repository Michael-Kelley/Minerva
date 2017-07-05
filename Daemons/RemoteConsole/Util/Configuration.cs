#region Includes

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Minerva
{
	#region INI

	// INI-handling class
	public class IniReader
	{
		[DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileIntA", CharSet = CharSet.Ansi)]
		private static extern int GetPrivateProfileInt(string lpApplicationName, string lpKeyName, int nDefault, string lpFileName);
		[DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStringA", CharSet = CharSet.Ansi)]
		private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);
		[DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA", CharSet = CharSet.Ansi)]
		private static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
		[DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileSectionNamesA", CharSet = CharSet.Ansi)]
		private static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer, int nSize, string lpFileName);
		[DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileSectionA", CharSet = CharSet.Ansi)]
		private static extern int WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);

		public IniReader(string file)
		{
			Filename = Directory.GetCurrentDirectory() + @"\" + file;
		}

		public string Filename
		{
			get { return m_Filename; }
			set { m_Filename = value; }
		}

		public string Section
		{
			get { return m_Section; }
			set { m_Section = value; }
		}

		public int ReadInteger(string section, string key, int defVal)
		{
			return GetPrivateProfileInt(section, key, defVal, Filename);
		}

		public int ReadInteger(string section, string key)
		{
			return ReadInteger(section, key, 0);
		}

		public int ReadInteger(string key, int defVal)
		{
			return ReadInteger(Section, key, defVal);
		}

		public int ReadInteger(string key)
		{
			return ReadInteger(key, 0);
		}

		public string ReadString(string section, string key, string defVal)
		{
			StringBuilder sb = new StringBuilder(MAX_ENTRY);
			int Ret = GetPrivateProfileString(section, key, defVal, sb, MAX_ENTRY, Filename);
			return sb.ToString();
		}

		public string ReadString(string section, string key)
		{
			return ReadString(section, key, "");
		}

		public string ReadString(string key)
		{
			return ReadString(Section, key);
		}

		public long ReadLong(string section, string key, long defVal)
		{
			return long.Parse(ReadString(section, key, defVal.ToString()));
		}

		public long ReadLong(string section, string key)
		{
			return ReadLong(section, key, 0);
		}

		public long ReadLong(string key, long defVal)
		{
			return ReadLong(Section, key, defVal);
		}

		public long ReadLong(string key)
		{
			return ReadLong(key, 0);
		}

		public byte[] ReadByteArray(string section, string key)
		{
			try { return Convert.FromBase64String(ReadString(section, key)); }
			catch { }
			return null;
		}

		public byte[] ReadByteArray(string key)
		{
			return ReadByteArray(Section, key);
		}

		public bool ReadBoolean(string section, string key, bool defVal)
		{
			return Boolean.Parse(ReadString(section, key, defVal.ToString()));
		}

		public bool ReadBoolean(string section, string key)
		{
			return ReadBoolean(section, key, false);
		}

		public bool ReadBoolean(string key, bool defVal)
		{
			return ReadBoolean(Section, key, defVal);
		}

		public bool ReadBoolean(string key)
		{
			return ReadBoolean(Section, key);
		}

		public bool Write(string section, string key, int value)
		{
			return Write(section, key, value.ToString());
		}

		public bool Write(string key, int value)
		{
			return Write(Section, key, value);
		}

		public bool Write(string section, string key, string value)
		{
			return (WritePrivateProfileString(section, key, value, Filename) != 0);
		}

		public bool Write(string key, string value)
		{
			return Write(Section, key, value);
		}

		public bool Write(string section, string key, long value)
		{
			return Write(section, key, value.ToString());
		}

		public bool Write(string key, long value)
		{
			return Write(Section, key, value);
		}

		public bool Write(string section, string key, byte[] value)
		{
			if (value == null)
				return Write(section, key, (string)null);
			else
				return Write(section, key, value, 0, value.Length);
		}

		public bool Write(string key, byte[] value)
		{
			return Write(Section, key, value);
		}

		public bool Write(string section, string key, byte[] value, int offset, int length)
		{
			if (value == null)
				return Write(section, key, (string)null);
			else
				return Write(section, key, Convert.ToBase64String(value, offset, length));
		}

		public bool Write(string section, string key, bool value)
		{
			return Write(section, key, value.ToString());
		}

		public bool Write(string key, bool value)
		{
			return Write(Section, key, value);
		}

		public bool DeleteKey(string section, string key)
		{
			return (WritePrivateProfileString(section, key, null, Filename) != 0);
		}

		public bool DeleteKey(string key)
		{
			return (WritePrivateProfileString(Section, key, null, Filename) != 0);
		}

		public bool DeleteSection(string section)
		{
			return WritePrivateProfileSection(section, null, Filename) != 0;
		}

		public ArrayList GetSectionNames()
		{
			try
			{
				byte[] buffer = new byte[MAX_ENTRY];
				GetPrivateProfileSectionNames(buffer, MAX_ENTRY, Filename);
				string[] parts = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');
				return new ArrayList(parts);
			}
			catch { }
			return null;
		}

		//Private variables and constants
		private string m_Filename;
		private string m_Section;
		private const int MAX_ENTRY = 32768;
	}

	#endregion

	// Config-loading class
	public class Configuration
	{
		/// <summary>A struct containing static values for colours used by the console.</summary>
		public struct Colours
		{
			/// <summary>The default background colour to use when outputting text to the console.</summary>
			public static ConsoleColor DefaultBG = ConsoleColor.Black;
			/// <summary>The default foreground colour to use when outputting text to the console.</summary>
			public static ConsoleColor DefaultFG = ConsoleColor.DarkGreen;
			/// <summary>The colour to use when displaying an error message in the console.</summary>
			public static ConsoleColor Error = ConsoleColor.Red;
			/// <summary>The colour to use when displaying a warning message in the console.</summary>
			public static ConsoleColor Warning = ConsoleColor.Yellow;
			/// <summary>The colour to use when displaying a notice message in the console.</summary>
			public static ConsoleColor Notice = ConsoleColor.Green;
		}

		public static IPAddress IP;
		public static int Port;
		public static string User;
		public static string Pass;

		public static void Load(string name)
		{
			IniReader conf = new IniReader(String.Format("conf/{0}.ini", name));

			IP = IPAddress.Parse(conf.ReadString("listen", "ip"));
			Port = conf.ReadInteger("listen", "port");
			User = conf.ReadString("auth", "user");
			Pass = conf.ReadString("auth", "password");
		}
	}
}