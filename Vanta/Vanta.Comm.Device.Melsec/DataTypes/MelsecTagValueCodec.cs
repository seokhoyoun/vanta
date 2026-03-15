using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Vanta.Comm.Contracts.Models;

namespace Vanta.Comm.Device.Melsec.DataTypes
{
    internal static class MelsecTagValueCodec
    {
        private enum ScalarType
        {
            Unknown = 0,
            Bool = 1,
            Int16 = 2,
            UInt16 = 3,
            Int32 = 4,
            UInt32 = 5,
            Int64 = 6,
            UInt64 = 7,
            Single = 8,
            Double = 9,
            String = 10,
        }

        private sealed class TypeDescriptor
        {
            public ScalarType ScalarType { get; set; }

            public bool IsArray { get; set; }

            public int DeclaredLength { get; set; }
        }

        public static int ResolveWordLength(TagDefinition tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            TypeDescriptor descriptor = ParseType(tag.DataType);
            int addressLength = tag.AddressLength;

            if (addressLength <= 0)
            {
                addressLength = 1;
            }

            if (descriptor.ScalarType == ScalarType.Unknown)
            {
                return addressLength;
            }

            if (descriptor.ScalarType == ScalarType.String)
            {
                if (descriptor.DeclaredLength > 0)
                {
                    int stringWordLength = descriptor.DeclaredLength / 2;
                    if ((descriptor.DeclaredLength % 2) != 0)
                    {
                        stringWordLength++;
                    }

                    if (stringWordLength < 1)
                    {
                        stringWordLength = 1;
                    }

                    if (addressLength > stringWordLength)
                    {
                        return addressLength;
                    }

                    return stringWordLength;
                }

                return addressLength;
            }

            int scalarWordLength = GetScalarWordLength(descriptor.ScalarType);

            if (descriptor.IsArray)
            {
                if (descriptor.DeclaredLength > 0)
                {
                    int arrayWordLength = scalarWordLength * descriptor.DeclaredLength;

                    if (addressLength > arrayWordLength)
                    {
                        return addressLength;
                    }

                    return arrayWordLength;
                }

                return addressLength;
            }

            if (addressLength > scalarWordLength)
            {
                return addressLength;
            }

            return scalarWordLength;
        }

        public static string ConvertBitValueToString(TagDefinition tag, int bitValue)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            TypeDescriptor descriptor = ParseType(tag.DataType);

            if (descriptor.ScalarType == ScalarType.Bool)
            {
                if (bitValue == 0)
                {
                    return bool.FalseString.ToLowerInvariant();
                }

                return bool.TrueString.ToLowerInvariant();
            }

            return bitValue.ToString(CultureInfo.InvariantCulture);
        }

        public static bool TryConvertStringToBitValue(TagDefinition tag, string value, out int bitValue)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return TryParseBooleanLike(value, out bitValue);
        }

        public static string ConvertWordsToTagValue(TagDefinition tag, IReadOnlyList<int> values)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            TypeDescriptor descriptor = ParseType(tag.DataType);

            if (descriptor.ScalarType == ScalarType.Unknown)
            {
                return ConvertUnknownWordsToString(values);
            }

            if (descriptor.ScalarType == ScalarType.String)
            {
                return ConvertWordsToAsciiString(values);
            }

            if (descriptor.IsArray)
            {
                return ConvertArrayToString(descriptor, values);
            }

            return ConvertScalarToString(descriptor.ScalarType, values, 0);
        }

        public static bool TryConvertTagValueToWords(
            TagDefinition tag,
            string value,
            int wordLength,
            out int[] values)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            TypeDescriptor descriptor = ParseType(tag.DataType);

            if (descriptor.ScalarType == ScalarType.Unknown)
            {
                return TryParseUnknownWords(value, wordLength, out values);
            }

            if (descriptor.ScalarType == ScalarType.String)
            {
                return TryEncodeAsciiString(value, wordLength, out values);
            }

            if (descriptor.IsArray)
            {
                return TryParseArray(descriptor, value, wordLength, out values);
            }

            return TryParseScalar(descriptor.ScalarType, value, wordLength, out values);
        }

        private static TypeDescriptor ParseType(string dataTypeText)
        {
            TypeDescriptor descriptor = new TypeDescriptor();
            descriptor.ScalarType = ScalarType.Unknown;

            if (string.IsNullOrWhiteSpace(dataTypeText))
            {
                return descriptor;
            }

            string normalized = NormalizeTypeText(dataTypeText);
            string baseType = normalized;
            int declaredLength = 0;
            bool isArray = false;

            if (normalized.EndsWith("]", StringComparison.Ordinal))
            {
                int openIndex = normalized.LastIndexOf('[');
                if (openIndex > 0)
                {
                    string lengthText = normalized.Substring(openIndex + 1, normalized.Length - openIndex - 2);

                    if (lengthText.Length > 0)
                    {
                        int.TryParse(lengthText, NumberStyles.Integer, CultureInfo.InvariantCulture, out declaredLength);
                    }

                    baseType = normalized.Substring(0, openIndex);

                    if (!string.Equals(baseType, "string", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(baseType, "ascii", StringComparison.OrdinalIgnoreCase))
                    {
                        isArray = true;
                    }
                }
            }

            descriptor.ScalarType = ParseScalarType(baseType);
            descriptor.IsArray = isArray;
            descriptor.DeclaredLength = declaredLength;

            return descriptor;
        }

        private static string NormalizeTypeText(string dataTypeText)
        {
            StringBuilder builder = new StringBuilder();
            int index;

            for (index = 0; index < dataTypeText.Length; index++)
            {
                char ch = dataTypeText[index];

                if (!char.IsWhiteSpace(ch))
                {
                    builder.Append(char.ToLowerInvariant(ch));
                }
            }

            return builder.ToString();
        }

        private static ScalarType ParseScalarType(string baseType)
        {
            if (baseType == "bool" || baseType == "boolean" || baseType == "bit")
            {
                return ScalarType.Bool;
            }

            if (baseType == "short" || baseType == "int16" || baseType == "i16")
            {
                return ScalarType.Int16;
            }

            if (baseType == "ushort" || baseType == "uint16" || baseType == "word" || baseType == "u16")
            {
                return ScalarType.UInt16;
            }

            if (baseType == "int" || baseType == "int32" || baseType == "i32" || baseType == "dint")
            {
                return ScalarType.Int32;
            }

            if (baseType == "uint" || baseType == "uint32" || baseType == "u32" || baseType == "udint")
            {
                return ScalarType.UInt32;
            }

            if (baseType == "long" || baseType == "int64" || baseType == "i64" || baseType == "lint")
            {
                return ScalarType.Int64;
            }

            if (baseType == "ulong" || baseType == "uint64" || baseType == "u64" || baseType == "ulint")
            {
                return ScalarType.UInt64;
            }

            if (baseType == "float" || baseType == "single" || baseType == "real")
            {
                return ScalarType.Single;
            }

            if (baseType == "double" || baseType == "lreal")
            {
                return ScalarType.Double;
            }

            if (baseType == "string" || baseType == "ascii")
            {
                return ScalarType.String;
            }

            return ScalarType.Unknown;
        }

        private static int GetScalarWordLength(ScalarType scalarType)
        {
            switch (scalarType)
            {
                case ScalarType.Bool:
                case ScalarType.Int16:
                case ScalarType.UInt16:
                    return 1;

                case ScalarType.Int32:
                case ScalarType.UInt32:
                case ScalarType.Single:
                    return 2;

                case ScalarType.Int64:
                case ScalarType.UInt64:
                case ScalarType.Double:
                    return 4;

            }

            return 1;
        }

        private static string ConvertUnknownWordsToString(IReadOnlyList<int> values)
        {
            if (values.Count == 0)
            {
                return "0";
            }

            if (values.Count == 1)
            {
                return values[0].ToString(CultureInfo.InvariantCulture);
            }

            StringBuilder builder = new StringBuilder();
            int index;

            for (index = 0; index < values.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(',');
                }

                builder.Append(values[index].ToString(CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }

        private static string ConvertArrayToString(TypeDescriptor descriptor, IReadOnlyList<int> values)
        {
            int scalarWordLength = GetScalarWordLength(descriptor.ScalarType);
            int count = descriptor.DeclaredLength;

            if (count <= 0)
            {
                count = values.Count / scalarWordLength;
            }

            if (count <= 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            int index;

            for (index = 0; index < count; index++)
            {
                if (index > 0)
                {
                    builder.Append(',');
                }

                builder.Append(ConvertScalarToString(descriptor.ScalarType, values, index * scalarWordLength));
            }

            return builder.ToString();
        }

        private static string ConvertScalarToString(ScalarType scalarType, IReadOnlyList<int> values, int startIndex)
        {
            switch (scalarType)
            {
                case ScalarType.Bool:
                    if (GetWordValue(values, startIndex) == 0)
                    {
                        return bool.FalseString.ToLowerInvariant();
                    }

                    return bool.TrueString.ToLowerInvariant();

                case ScalarType.Int16:
                    return ((short)(GetWordValue(values, startIndex) & 0xFFFF)).ToString(CultureInfo.InvariantCulture);

                case ScalarType.UInt16:
                    return ((ushort)(GetWordValue(values, startIndex) & 0xFFFF)).ToString(CultureInfo.InvariantCulture);

                case ScalarType.Int32:
                    return ToInt32(values, startIndex).ToString(CultureInfo.InvariantCulture);

                case ScalarType.UInt32:
                    return ToUInt32(values, startIndex).ToString(CultureInfo.InvariantCulture);

                case ScalarType.Int64:
                    return ToInt64(values, startIndex).ToString(CultureInfo.InvariantCulture);

                case ScalarType.UInt64:
                    return ToUInt64(values, startIndex).ToString(CultureInfo.InvariantCulture);

                case ScalarType.Single:
                    return ToSingle(values, startIndex).ToString("R", CultureInfo.InvariantCulture);

                case ScalarType.Double:
                    return ToDouble(values, startIndex).ToString("R", CultureInfo.InvariantCulture);

            }

            return GetWordValue(values, startIndex).ToString(CultureInfo.InvariantCulture);
        }

        private static string ConvertWordsToAsciiString(IReadOnlyList<int> values)
        {
            if (values.Count == 0)
            {
                return string.Empty;
            }

            byte[] buffer = CreateByteBuffer(values, 0, values.Count);
            int length = buffer.Length;

            while (length > 0 && buffer[length - 1] == 0)
            {
                length--;
            }

            return Encoding.ASCII.GetString(buffer, 0, length);
        }

        private static bool TryParseUnknownWords(string value, int wordLength, out int[] values)
        {
            values = new int[wordLength];

            if (wordLength <= 0)
            {
                return false;
            }

            if (wordLength == 1)
            {
                int parsedSingle;
                if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedSingle))
                {
                    return false;
                }

                values[0] = parsedSingle;
                return true;
            }

            List<string> tokens = SplitCsv(value);
            if (tokens.Count != wordLength)
            {
                return false;
            }

            int index;
            for (index = 0; index < tokens.Count; index++)
            {
                int parsedItem;
                if (!int.TryParse(tokens[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedItem))
                {
                    return false;
                }

                values[index] = parsedItem;
            }

            return true;
        }

        private static bool TryEncodeAsciiString(string value, int wordLength, out int[] values)
        {
            values = new int[wordLength];

            if (wordLength <= 0)
            {
                return false;
            }

            string safeValue = value;
            if (safeValue == null)
            {
                safeValue = string.Empty;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(safeValue);
            int capacity = wordLength * 2;

            if (bytes.Length > capacity)
            {
                return false;
            }

            int index;
            for (index = 0; index < bytes.Length; index += 2)
            {
                byte low = bytes[index];
                byte high = 0;

                if ((index + 1) < bytes.Length)
                {
                    high = bytes[index + 1];
                }

                int wordIndex = index / 2;
                values[wordIndex] = low | (high << 8);
            }

            return true;
        }

        private static bool TryParseArray(
            TypeDescriptor descriptor,
            string value,
            int wordLength,
            out int[] values)
        {
            values = new int[wordLength];

            if (wordLength <= 0)
            {
                return false;
            }

            int scalarWordLength = GetScalarWordLength(descriptor.ScalarType);
            if (scalarWordLength <= 0)
            {
                return false;
            }

            List<string> tokens = SplitCsv(value);
            int expectedCount = descriptor.DeclaredLength;

            if (expectedCount <= 0)
            {
                expectedCount = wordLength / scalarWordLength;
            }

            if (expectedCount <= 0)
            {
                return false;
            }

            if (tokens.Count != expectedCount)
            {
                return false;
            }

            int requiredLength = expectedCount * scalarWordLength;
            if (requiredLength > wordLength)
            {
                return false;
            }

            int index;
            for (index = 0; index < tokens.Count; index++)
            {
                int[] itemValues;
                if (!TryParseScalar(descriptor.ScalarType, tokens[index], scalarWordLength, out itemValues))
                {
                    return false;
                }

                CopyWords(itemValues, 0, values, index * scalarWordLength, scalarWordLength);
            }

            return true;
        }

        private static bool TryParseScalar(
            ScalarType scalarType,
            string value,
            int wordLength,
            out int[] values)
        {
            int requiredLength = GetScalarWordLength(scalarType);
            int totalLength = wordLength;

            if (totalLength < requiredLength)
            {
                totalLength = requiredLength;
            }

            values = new int[totalLength];

            switch (scalarType)
            {
                case ScalarType.Bool:
                {
                    int boolValue;
                    if (!TryParseBooleanLike(value, out boolValue))
                    {
                        return false;
                    }

                    values[0] = boolValue;
                    return true;
                }

                case ScalarType.Int16:
                {
                    short parsedInt16;
                    if (!short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedInt16))
                    {
                        return false;
                    }

                    values[0] = (ushort)parsedInt16;
                    return true;
                }

                case ScalarType.UInt16:
                {
                    ushort parsedUInt16;
                    if (!ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedUInt16))
                    {
                        return false;
                    }

                    values[0] = parsedUInt16;
                    return true;
                }

                case ScalarType.Int32:
                {
                    int parsedInt32;
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedInt32))
                    {
                        return false;
                    }

                    byte[] buffer = BitConverter.GetBytes(parsedInt32);
                    WriteBytesToWords(buffer, values, 0, requiredLength);
                    return true;
                }

                case ScalarType.UInt32:
                {
                    uint parsedUInt32;
                    if (!uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedUInt32))
                    {
                        return false;
                    }

                    byte[] buffer = BitConverter.GetBytes(parsedUInt32);
                    WriteBytesToWords(buffer, values, 0, requiredLength);
                    return true;
                }

                case ScalarType.Int64:
                {
                    long parsedInt64;
                    if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedInt64))
                    {
                        return false;
                    }

                    byte[] buffer = BitConverter.GetBytes(parsedInt64);
                    WriteBytesToWords(buffer, values, 0, requiredLength);
                    return true;
                }

                case ScalarType.UInt64:
                {
                    ulong parsedUInt64;
                    if (!ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedUInt64))
                    {
                        return false;
                    }

                    byte[] buffer = BitConverter.GetBytes(parsedUInt64);
                    WriteBytesToWords(buffer, values, 0, requiredLength);
                    return true;
                }

                case ScalarType.Single:
                {
                    float parsedSingle;
                    if (!float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out parsedSingle))
                    {
                        return false;
                    }

                    byte[] buffer = BitConverter.GetBytes(parsedSingle);
                    WriteBytesToWords(buffer, values, 0, requiredLength);
                    return true;
                }

                case ScalarType.Double:
                {
                    double parsedDouble;
                    if (!double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out parsedDouble))
                    {
                        return false;
                    }

                    byte[] buffer = BitConverter.GetBytes(parsedDouble);
                    WriteBytesToWords(buffer, values, 0, requiredLength);
                    return true;
                }

            }

            return false;
        }

        private static bool TryParseBooleanLike(string value, out int bitValue)
        {
            bitValue = 0;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalized = value.Trim();

            if (string.Equals(normalized, "1", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "true", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "on", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "yes", StringComparison.OrdinalIgnoreCase))
            {
                bitValue = 1;
                return true;
            }

            if (string.Equals(normalized, "0", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "false", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "off", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "no", StringComparison.OrdinalIgnoreCase))
            {
                bitValue = 0;
                return true;
            }

            return false;
        }

        private static List<string> SplitCsv(string text)
        {
            List<string> items = new List<string>();
            StringBuilder builder = new StringBuilder();
            int depth = 0;
            int index;

            if (text == null)
            {
                items.Add(string.Empty);
                return items;
            }

            for (index = 0; index < text.Length; index++)
            {
                char ch = text[index];

                switch (ch)
                {
                    case '(':
                    case '[':
                    case '{':
                        depth++;
                        builder.Append(ch);
                        break;

                    case ')':
                    case ']':
                    case '}':
                        if (depth > 0)
                        {
                            depth--;
                        }

                        builder.Append(ch);
                        break;

                    case ',':
                        if (depth == 0)
                        {
                            items.Add(builder.ToString().Trim());
                            builder.Length = 0;
                        }
                        else
                        {
                            builder.Append(ch);
                        }
                        break;

                    default:
                        builder.Append(ch);
                        break;
                }
            }

            items.Add(builder.ToString().Trim());
            return items;
        }

        private static int GetWordValue(IReadOnlyList<int> values, int index)
        {
            if (index < 0 || index >= values.Count)
            {
                return 0;
            }

            return values[index];
        }

        private static int ToInt32(IReadOnlyList<int> values, int startIndex)
        {
            byte[] buffer = CreateByteBuffer(values, startIndex, 2);
            return BitConverter.ToInt32(buffer, 0);
        }

        private static uint ToUInt32(IReadOnlyList<int> values, int startIndex)
        {
            byte[] buffer = CreateByteBuffer(values, startIndex, 2);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private static long ToInt64(IReadOnlyList<int> values, int startIndex)
        {
            byte[] buffer = CreateByteBuffer(values, startIndex, 4);
            return BitConverter.ToInt64(buffer, 0);
        }

        private static ulong ToUInt64(IReadOnlyList<int> values, int startIndex)
        {
            byte[] buffer = CreateByteBuffer(values, startIndex, 4);
            return BitConverter.ToUInt64(buffer, 0);
        }

        private static float ToSingle(IReadOnlyList<int> values, int startIndex)
        {
            byte[] buffer = CreateByteBuffer(values, startIndex, 2);
            return BitConverter.ToSingle(buffer, 0);
        }

        private static double ToDouble(IReadOnlyList<int> values, int startIndex)
        {
            byte[] buffer = CreateByteBuffer(values, startIndex, 4);
            return BitConverter.ToDouble(buffer, 0);
        }

        private static byte[] CreateByteBuffer(IReadOnlyList<int> values, int startIndex, int wordLength)
        {
            byte[] buffer = new byte[wordLength * 2];
            int index;

            for (index = 0; index < wordLength; index++)
            {
                int word = GetWordValue(values, startIndex + index);
                buffer[index * 2] = (byte)(word & 0xFF);
                buffer[(index * 2) + 1] = (byte)((word >> 8) & 0xFF);
            }

            return buffer;
        }

        private static void WriteBytesToWords(byte[] bytes, int[] destination, int destinationStartIndex, int wordLength)
        {
            int index;

            for (index = 0; index < wordLength; index++)
            {
                int byteIndex = index * 2;
                byte low = 0;
                byte high = 0;

                if (byteIndex < bytes.Length)
                {
                    low = bytes[byteIndex];
                }

                if ((byteIndex + 1) < bytes.Length)
                {
                    high = bytes[byteIndex + 1];
                }

                destination[destinationStartIndex + index] = low | (high << 8);
            }
        }

        private static void CopyWords(
            int[] source,
            int sourceStartIndex,
            int[] destination,
            int destinationStartIndex,
            int length)
        {
            int index;

            for (index = 0; index < length; index++)
            {
                destination[destinationStartIndex + index] = source[sourceStartIndex + index];
            }
        }
    }
}
