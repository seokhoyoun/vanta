using System;
using System.Globalization;
using Vanta.Comm.Contracts.Enums;

namespace Vanta.Comm.Device.Melsec.Addressing
{
    public sealed class MelsecAddressParser
    {
        public MelsecAddress Parse(string memoryHead, string addressText, AddressFormat addressFormat)
        {
            if (string.IsNullOrWhiteSpace(memoryHead))
            {
                throw new InvalidOperationException("MELSEC memory head is required.");
            }

            int address = ParseAddress(addressText, addressFormat);
            return new MelsecAddress(memoryHead.Trim(), address);
        }

        public int ParseAddress(string addressText, AddressFormat addressFormat)
        {
            if (string.IsNullOrWhiteSpace(addressText))
            {
                return 0;
            }

            string normalized = addressText.Trim();

            if (addressFormat == AddressFormat.Hexadecimal)
            {
                return int.Parse(normalized, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return int.Parse(normalized, CultureInfo.InvariantCulture);
        }
    }
}
