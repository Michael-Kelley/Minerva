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
using System.Diagnostics;
using System.Runtime.InteropServices;

#endregion

namespace Minerva.Util
{
    class Info
    {
		[DllImport("User32.dll", SetLastError = false)]
		static extern int GetSystemMetrics(int nIndex);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetVersionEx(ref OSVERSIONINFOEX lpVersionInfo);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		struct OSVERSIONINFOEX
		{
			public static int SizeOf
			{
				get
				{
					return Marshal.SizeOf(typeof(OSVERSIONINFOEX));
				}
			}

			public uint dwOSVersionInfoSize;
			public uint dwMajorVersion;
			public uint dwMinorVersion;
			public uint dwBuildNumber;
			public uint dwPlatformId;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;

			public ushort wServicePackMajor;

			public ushort wServicePackMinor;
			public ushort wSuiteMask;
			public byte wProductType;
			public byte wReserved;
		}


        private const int VER_NT_WORKSTATION = 1;
        private const int VER_NT_DOMAIN_CONTROLLER = 2;
        private const int VER_NT_SERVER = 3;
        private const int VER_SUITE_SMALLBUSINESS = 1;
        private const int VER_SUITE_ENTERPRISE = 2;
        private const int VER_SUITE_TERMINAL = 16;
        private const int VER_SUITE_DATACENTER = 128;
        private const int VER_SUITE_SINGLEUSERTS = 256;
        private const int VER_SUITE_PERSONAL = 512;
        private const int VER_SUITE_BLADE = 1024;
        private const int VER_SUITE_WH_SERVER = 8000;

        private static string OSProductType
        {
            get
            {
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                OperatingSystem osInfo = Environment.OSVersion;
                osVersionInfo.dwOSVersionInfoSize = (uint)OSVERSIONINFOEX.SizeOf;
                if (!GetVersionEx(ref osVersionInfo))
                    return "";
                else
                {
                    if (osInfo.Version.Major == 4)
                    {
                        if (osVersionInfo.wProductType == VER_NT_WORKSTATION)
                            // Windows NT 4.0 Workstation
                            return "Workstation";
                        else
                            if (osVersionInfo.wProductType == VER_NT_SERVER)
                                // Windows NT 4.0 Server
                                return "Server";
                            else
                                return "";
                    }
                    else if (osInfo.Version.Major == 5)
                    {
                        if (osVersionInfo.wProductType == VER_NT_WORKSTATION)
                        {
                            if ((osVersionInfo.wSuiteMask & VER_SUITE_PERSONAL) ==
                                   VER_SUITE_PERSONAL)
                                // Windows XP Home Edition
                                return "Home Edition";
                            else
                                // Windows XP / Windows 2000 Professional
                                return "Professional";
                        }
                        else if (osVersionInfo.wProductType == VER_NT_SERVER)
                        {
                            if (osInfo.Version.Minor == 0)
                            {
                                if ((osVersionInfo.wSuiteMask & VER_SUITE_DATACENTER) ==
                                       VER_SUITE_DATACENTER)
                                    // Windows 2000 Datacenter Server
                                    return "Datacenter Server";
                                else
                                    if ((osVersionInfo.wSuiteMask & VER_SUITE_ENTERPRISE) ==
                                           VER_SUITE_ENTERPRISE)
                                        // Windows 2000 Advanced Server
                                        return "Advanced Server";
                                    else
                                        // Windows 2000 Server
                                        return "Server";
                            }
                        }
                        else
                        {
                            if ((osVersionInfo.wSuiteMask & VER_SUITE_DATACENTER) ==
                                   VER_SUITE_DATACENTER)
                                // Windows Server 2003 Datacenter Edition
                                return "Datacenter Edition";
                            else
                                if ((osVersionInfo.wSuiteMask & VER_SUITE_ENTERPRISE) ==
                                       VER_SUITE_ENTERPRISE)
                                    // Windows Server 2003 Enterprise Edition
                                    return "Enterprise Edition";
                                else
                                    if ((osVersionInfo.wSuiteMask & VER_SUITE_BLADE) ==
                                           VER_SUITE_BLADE)
                                        // Windows Server 2003 Web Edition
                                        return "Web Edition";
                                    else
                                        // Windows Server 2003 Standard Edition
                                        return "Standard Edition";
                        }
                    }
                    else if (osInfo.Version.Major == 6)
                    {
                        if (osVersionInfo.wProductType == 1)
                            return "Ultimate Edition";
                        if (osVersionInfo.wProductType == 6)
                            return "Business Edition";
                        if (osVersionInfo.wProductType == 4)
                            return "Enterprise Edition";
                        if (osVersionInfo.wProductType == 2)
                            return "Home Basic Edition";
                        if (osVersionInfo.wProductType == 3)
                            return "Premium Edition";
                        if (osVersionInfo.wProductType == 11)
                            return "Starter Edition";
                        if (osVersionInfo.wProductType == 28)
                            return "Ultimate N Edition";
                        if (osVersionInfo.wProductType == 16)
                            return "Business N Edition";
                        if (osVersionInfo.wProductType == 27)
                            return "Enterprise N Edition";
                        if (osVersionInfo.wProductType == 5)
                            return "Home Basic N Edition";
                        if (osVersionInfo.wProductType == 26)
                            return "Premium N Edition";
                        if (osVersionInfo.wProductType == 17)
                            return "Web Server Edition";
                        if (osVersionInfo.wProductType == 15)
                            return "Enterprise IA64 Edition";
                        if (osVersionInfo.wProductType == 10 || osVersionInfo.wProductType == 14)
                            return "Enterprise Edition";
                        if (osVersionInfo.wProductType == 7 || osVersionInfo.wProductType == 13)
                            return "Standard Edition";
                        if (osVersionInfo.wProductType == 8 || osVersionInfo.wProductType == 12)
                            return "Datacenter Edition";
                    }
                }
                return "";
            }
        }

        private static string OSServicePack
        {
            get
            {
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = (uint)OSVERSIONINFOEX.SizeOf;

                bool version = GetVersionEx(ref osVersionInfo);

                if (!version || osVersionInfo.szCSDVersion == "")
                    return "None";
                else
                    return osVersionInfo.szCSDVersion;
            }
        }

        private static string OSName
        {
            get
            {
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                OperatingSystem osInfo = Environment.OSVersion;
				osVersionInfo.dwOSVersionInfoSize = (uint)OSVERSIONINFOEX.SizeOf;
				GetVersionEx(ref osVersionInfo);
                string osName = "UNKNOWN";

                switch (osInfo.Platform)
                {
                    case PlatformID.Win32Windows:
                        switch (osInfo.Version.Minor)
                        {
                            case 0:
                                osName = "Windows 95";
                                break;

                            case 10:
                                if (osInfo.Version.Revision.ToString() == "2222A")
                                    osName = "Windows 98 Second Edition";
                                else
                                    osName = "Windows 98";
                                break;

                            case 90:
                                osName = "Windows Me";
                                break;
                        }
                        break;

                    case PlatformID.Win32NT:
                        switch (osInfo.Version.Major)
                        {
                            case 3:
                                osName = "Windows NT 3.51";
                                break;

                            case 4:
                                osName = "Windows NT 4.0";
                                break;

                            case 5:
                                if (osInfo.Version.Minor == 0 && osVersionInfo.wSuiteMask == VER_NT_WORKSTATION)
                                    osName = "Windows 2000";
                                else if (osInfo.Version.Minor == 0 && osVersionInfo.wSuiteMask != VER_NT_WORKSTATION)
                                    osName = "Windows 2000 Server";
                                else if (osInfo.Version.Minor == 1 && GetSystemMetrics(88) != 0)  // SM_STARTER = 88
                                    osName = "Windows XP Starter Edition";
                                else if (osInfo.Version.Minor == 1 && GetSystemMetrics(86) != 0)  // SM_TABLETPC = 86
                                    osName = "Windows XP Tabled PC Edition";
                                else if (osInfo.Version.Minor == 1 && GetSystemMetrics(87) != 0)  // SM_TABLETPC = 87
                                    osName = "Windows XP Media Center Edition";
                                else if (osInfo.Version.Minor == 1)
                                    osName = "Windows XP";
                                else if (osInfo.Version.Minor == 2 && GetSystemMetrics(89) == 0)  // SM_SERVERR2 = 89
                                    osName = "Windows Server 2003";
                                else if (osInfo.Version.Minor == 2 && GetSystemMetrics(89) != 0)
                                    osName = "Windows Server 2003 R2";
                                else if (osInfo.Version.Minor == 2 && osVersionInfo.wSuiteMask == VER_SUITE_WH_SERVER)
                                    osName = "Windows Home Server";
                                break;

                            case 6:
                                if (osInfo.Version.Minor == 0 && osVersionInfo.wProductType == VER_NT_WORKSTATION)
                                    osName = "Windows Vista";
                                else if (osInfo.Version.Minor == 0 && osVersionInfo.wProductType != VER_NT_WORKSTATION)
                                    osName = "Windows Server 2008";
                                else if (osInfo.Version.Minor == 1 && osVersionInfo.wProductType == VER_NT_WORKSTATION)
                                    osName = "Windows 7";
                                else if (osInfo.Version.Minor == 1 && osVersionInfo.wProductType != VER_NT_WORKSTATION)
                                    osName = "Windows Server 2008 R2";
                                break;
                        }
                        break;
                }
                return osName;
            }
        }

        private static int OSArchitecture
        {
            get
            {
                string pa = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                return ((String.IsNullOrEmpty(pa) || String.Compare(pa, 0, "x86", 0, 3, true) == 0) ? 32 : 64);
            }
        }

        private static String OSVersion
        {
            get { return Environment.OSVersion.Version.ToString(); }
        }

        public static String AvailableMemory
        {
            get
            {
                PerformanceCounter ramCount = new PerformanceCounter("Memory", "Available MBytes");
                return ramCount.NextValue() + "MB";
            }
        }

        public static String CPUUsage
        {
            get
            {
                PerformanceCounter cpuCount = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                return cpuCount.NextValue() + "%";
            }
        }

        public static void PrintInfo()
        {
			Log.Message("===============================================================================", ConsoleColor.DarkYellow);
			var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			var offset = (Console.WindowWidth + version.ToString().Length + 8) / 2;
			Log.Message(String.Format("version {0}", version).PadLeft(offset, ' '), ConsoleColor.White);
            Log.Message("===============================================================================", ConsoleColor.DarkYellow);
            /*Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("System Information:");
            Console.Write(" OS Name".PadRight(20, '.'));
            Console.ForegroundColor = ConsoleColor.Gray; Console.Write(OSName + "\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" OS Version".PadRight(20, '.'));
            Console.ForegroundColor = ConsoleColor.Gray; Console.Write(OSVersion + "\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" OS Service Pack".PadRight(20, '.'));
            Console.ForegroundColor = ConsoleColor.Gray; Console.Write(OSServicePack + "\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" OS Product Type".PadRight(20, '.'));
            Console.ForegroundColor = ConsoleColor.Gray; Console.Write(OSProductType + "\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" OS Architecture".PadRight(20, '.'));
            Console.ForegroundColor = ConsoleColor.Gray; Console.Write(OSArchitecture + "\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" Available Memory".PadRight(20, '.'));
            Console.ForegroundColor = ConsoleColor.Gray; Console.Write(AvailableMemory + "\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" CPU Usage".PadRight(20, '.'));
            Console.ForegroundColor = ConsoleColor.Gray; Console.Write(CPUUsage + "\n");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("===============================================================================");*/
        }

        public static void PrintLogo()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(
@"           _____  _________ _        _______  ______             _____");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(
@"          /     \ \__   __/| \    /||  ____ \|  ___ \ |\     /| / ___ \
         | || || |   | |   |  \  | || |    \/| |   \ || |   | || |   | |");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(
@"         | || || |   | |   |   \ | || |__    | |___/ || |   | || |___| |
         | ||_|| |   | |   | |\ \| ||  __|   |     _/ | |   | ||  ___  |");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(
@"         | |   | |   | |   | | \   || |      | |\ \    \ \_/ / | |   | |
         | |   | |___| |___| |  \  || |____/\| | \ \__  \   /  | |   | |");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(
@"         |/     \|\_______/|/    \_||_______/|/   \__/   \_/   |/     \|");
        }
    }
}