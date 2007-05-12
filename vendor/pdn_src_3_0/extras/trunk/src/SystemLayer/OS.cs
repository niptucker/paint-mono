/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using Microsoft.Win32;
using System;
using System.Globalization;

namespace PaintDotNet.SystemLayer
{
    public static class OS
    {
        public static bool IsVistaOrLater
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6;
            }
        }

        public static Version WindowsXP
        {
            get
            {
                return new Version(5, 1);
            }
        }

        public static Version WindowsServer2003
        {
            get
            {
                return new Version(5, 2);
            }
        }

        public static Version WindowsVista
        {
            get
            {
                return new Version(6, 0);
            }
        }

        public static string Revision
        {
            get
            {
                NativeStructs.OSVERSIONINFOEX osviex = new NativeStructs.OSVERSIONINFOEX();
                osviex.dwOSVersionInfoSize = (uint)NativeStructs.OSVERSIONINFOEX.SizeOf;
                bool result = SafeNativeMethods.GetVersionEx(ref osviex);

                if (result)
                {
                    return osviex.szCSDVersion;
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public static OSType Type
        {
            get
            {
                NativeStructs.OSVERSIONINFOEX osviex = new NativeStructs.OSVERSIONINFOEX();
                osviex.dwOSVersionInfoSize = (uint)NativeStructs.OSVERSIONINFOEX.SizeOf;
                bool result = SafeNativeMethods.GetVersionEx(ref osviex);
                OSType type;

                if (result)
                {
                    if (Enum.IsDefined(typeof(OSType), (OSType)osviex.wProductType))
                    {
                        type = (OSType)osviex.wProductType;
                    }
                    else
                    {
                        type = OSType.Unknown;
                    }
                }
                else
                {
                    type = OSType.Unknown;
                }

                return type;
            }
        }

        public static bool IsDotNetVersionInstalled(int major, int minor, int build)
        {
            const string regKeyNameFormat = "Software\\Microsoft\\NET Framework Setup\\NDP\\v{0}.{1}.{2}";
            const string regValueName = "Install";

            string regKeyName = string.Format(regKeyNameFormat, major.ToString(CultureInfo.InvariantCulture),
                minor.ToString(CultureInfo.InvariantCulture), build.ToString(CultureInfo.InvariantCulture));

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regKeyName, false))
            {
                object value = null;

                if (key != null)
                {
                    value = key.GetValue(regValueName);
                }

                return (value != null && value is int && (int)value == 1);
            }
        }

        public static bool CheckWindowsVersion(int major, int minor, short servicePack)
        {
            NativeStructs.OSVERSIONINFOEX osvi = new NativeStructs.OSVERSIONINFOEX();
            osvi.dwOSVersionInfoSize = (uint)NativeStructs.OSVERSIONINFOEX.SizeOf;
            osvi.dwMajorVersion = (uint)major;
            osvi.dwMinorVersion = (uint)minor;
            osvi.wServicePackMajor = (ushort)servicePack;

            ulong mask = 0;
            mask = NativeMethods.VerSetConditionMask(mask, NativeConstants.VER_MAJORVERSION, NativeConstants.VER_GREATER_EQUAL);
            mask = NativeMethods.VerSetConditionMask(mask, NativeConstants.VER_MINORVERSION, NativeConstants.VER_GREATER_EQUAL);
            mask = NativeMethods.VerSetConditionMask(mask, NativeConstants.VER_SERVICEPACKMAJOR, NativeConstants.VER_GREATER_EQUAL);

            bool result = NativeMethods.VerifyVersionInfo(
                ref osvi,
                NativeConstants.VER_MAJORVERSION |
                    NativeConstants.VER_MINORVERSION |
                    NativeConstants.VER_SERVICEPACKMAJOR,
                mask);

            return result;
        }

        public static bool CheckOSRequirement()
        {
			return true;
        }
    }
}
