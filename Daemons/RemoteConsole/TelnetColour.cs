#region Includes

using System;
using System.IO;

#endregion

namespace Minerva
{
	public sealed class TelnetForeColour
	{
		public static readonly string None = String.Format("{0}[37;1m", (char)0x1B);
		public static readonly string Black = String.Format("{0}[30;22m", (char)0x1B);
		public static readonly string DarkRed = String.Format("{0}[31;22m", (char)0x1B);
		public static readonly string DarkGreen = String.Format("{0}[32;22m", (char)0x1B);
		public static readonly string DarkYellow = String.Format("{0}[33;22m", (char)0x1B);
		public static readonly string DarkBlue = String.Format("{0}[34;22m", (char)0x1B);
		public static readonly string DarkMagenta = String.Format("{0}[35;22m", (char)0x1B);
		public static readonly string DarkCyan = String.Format("{0}[36;22m", (char)0x1B);
		public static readonly string Grey = String.Format("{0}[37;22m", (char)0x1B);
		public static readonly string DarkGrey = String.Format("{0}[30;1m", (char)0x1B);
		public static readonly string Red = String.Format("{0}[31;1m", (char)0x1B);
		public static readonly string Green = String.Format("{0}[32;1m", (char)0x1B);
		public static readonly string Yellow = String.Format("{0}[33;1m", (char)0x1B);
		public static readonly string Blue = String.Format("{0}[34;1m", (char)0x1B);
		public static readonly string Magenta = String.Format("{0}[35;1m", (char)0x1B);
		public static readonly string Cyan = String.Format("{0}[36;1m", (char)0x1B);
		public static readonly string White = String.Format("{0}[37;1m", (char)0x1B);
	}

	public sealed class TelnetBackColour
	{
		public static readonly string None = String.Format("{0}[40m", (char)0x1B);
		public static readonly string Black = String.Format("{0}[40m", (char)0x1B);
		public static readonly string Red = String.Format("{0}[41m", (char)0x1B);
		public static readonly string Green = String.Format("{0}[42m", (char)0x1B);
		public static readonly string Yellow = String.Format("{0}[43m", (char)0x1B);
		public static readonly string Blue = String.Format("{0}[44m", (char)0x1B);
		public static readonly string Magenta = String.Format("{0}[45m", (char)0x1B);
		public static readonly string Cyan = String.Format("{0}[46m", (char)0x1B);
		public static readonly string Grey = String.Format("{0}[47m", (char)0x1B);
	}
}