using System;
using System.Collections.Generic;

namespace Vanta.Comm.Device.Mitsubishi.PLC.McProtocol.Communication
{
    internal static class MelsecDeviceAccessCatalog
    {
        private static readonly Dictionary<string, MelsecDeviceAccessSpec> s_specs =
            new Dictionary<string, MelsecDeviceAccessSpec>(StringComparer.OrdinalIgnoreCase)
            {
                { "X", new MelsecDeviceAccessSpec("X", 0x9C, true) },
                { "Y", new MelsecDeviceAccessSpec("Y", 0x9D, true) },
                { "M", new MelsecDeviceAccessSpec("M", 0x90, true) },
                { "L", new MelsecDeviceAccessSpec("L", 0x92, true) },
                { "F", new MelsecDeviceAccessSpec("F", 0x93, true) },
                { "V", new MelsecDeviceAccessSpec("V", 0x94, true) },
                { "B", new MelsecDeviceAccessSpec("B", 0xA0, true) },
                { "TS", new MelsecDeviceAccessSpec("TS", 0xC1, true) },
                { "TC", new MelsecDeviceAccessSpec("TC", 0xC0, true) },
                { "SS", new MelsecDeviceAccessSpec("SS", 0xC7, true) },
                { "SC", new MelsecDeviceAccessSpec("SC", 0xC6, true) },
                { "CS", new MelsecDeviceAccessSpec("CS", 0xC4, true) },
                { "CC", new MelsecDeviceAccessSpec("CC", 0xC3, true) },
                { "SB", new MelsecDeviceAccessSpec("SB", 0xA1, true) },
                { "SM", new MelsecDeviceAccessSpec("SM", 0x91, true) },
                { "DX", new MelsecDeviceAccessSpec("DX", 0xA2, true) },
                { "DY", new MelsecDeviceAccessSpec("DY", 0xA3, true) },
                { "D", new MelsecDeviceAccessSpec("D", 0xA8, false) },
                { "W", new MelsecDeviceAccessSpec("W", 0xB4, false) },
                { "TN", new MelsecDeviceAccessSpec("TN", 0xC2, false) },
                { "SN", new MelsecDeviceAccessSpec("SN", 0xC8, false) },
                { "CN", new MelsecDeviceAccessSpec("CN", 0xC5, false) },
                { "SW", new MelsecDeviceAccessSpec("SW", 0xB5, false) },
                { "SD", new MelsecDeviceAccessSpec("SD", 0xA9, false) },
                { "Z", new MelsecDeviceAccessSpec("Z", 0xCC, false) },
                { "R", new MelsecDeviceAccessSpec("R", 0xAF, false) },
                { "ZR", new MelsecDeviceAccessSpec("ZR", 0xB0, false) },
            };

        public static bool TryGetSpec(string memoryHead, out MelsecDeviceAccessSpec? spec)
        {
            if (string.IsNullOrWhiteSpace(memoryHead))
            {
                spec = null;
                return false;
            }

            string normalized = memoryHead.Trim();
            return s_specs.TryGetValue(normalized, out spec);
        }
    }
}
