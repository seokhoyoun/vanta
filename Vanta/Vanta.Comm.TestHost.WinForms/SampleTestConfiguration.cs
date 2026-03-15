using System;
using System.Collections.Generic;
using System.Text;
using Vanta.Comm.Contracts.Enums;
using Vanta.Comm.Contracts.Models;
using Vanta.Comm.Simulation.Profiles;

namespace Vanta.Comm.TestHost.WinForms
{
    internal static class SampleTestConfiguration
    {
        public static DeviceDefinition CreateDevice()
        {
            DeviceDefinition device = new DeviceDefinition();
            device.DeviceId = 1;
            device.DeviceName = "Simulated MELSEC";
            device.DriverModule = "MELSEC";
            device.CommunicationType = CommunicationType.Melsec;
            device.ThreadFlags = ThreadEnableFlags.Double;
            device.NetworkNumber = 1;
            device.StationNumber = 0;
            device.DeviceIpAddress = "127.0.0.1";
            device.DevicePort = 5000;
            device.Timeout = 3000;
            device.ScanTime = 500;
            device.IsEnabled = true;

            return device;
        }

        public static BlockDefinition CreateBlock()
        {
            BlockDefinition block = new BlockDefinition();
            block.BlockSequence = 1000;
            block.DeviceId = 1;
            block.BlockName = "D100_SAMPLE";
            block.BlockHead = "D";
            block.BaseAddress = "100";
            block.BlockLength = 64;
            block.AddressFormat = AddressFormat.Decimal;
            block.MemoryKind = MemoryKind.Word;
            block.IsBlockUsed = true;
            block.IsEnabled = true;

            return block;
        }

        public static List<TagDefinition> CreateTags()
        {
            List<TagDefinition> tags = new List<TagDefinition>();

            tags.Add(CreateBitTag());
            tags.Add(CreateInt32Tag());
            tags.Add(CreateFloatArrayTag());
            tags.Add(CreateStringTag());
            tags.Add(CreateInt16ArrayTag());
            tags.Add(CreateDoubleTag());

            return tags;
        }

        public static DeviceSimulationProfile CreateProfile()
        {
            DeviceSimulationProfile profile = new DeviceSimulationProfile();
            List<DeviceSimulationMemoryPreset> presets = new List<DeviceSimulationMemoryPreset>();

            presets.Add(CreatePreset("M", 100, new int[] { 1 }));
            presets.Add(CreatePreset("D", 100, EncodeInt32(123456)));
            presets.Add(CreatePreset("D", 110, EncodeSingles(new float[] { 12.5f, 24.75f })));
            presets.Add(CreatePreset("D", 120, EncodeAscii("READY", 8)));
            presets.Add(CreatePreset("D", 130, new int[] { 10, 20, 30, 40 }));
            presets.Add(CreatePreset("D", 140, EncodeDouble(123.456d)));

            profile.Presets = presets;
            return profile;
        }

        private static TagDefinition CreateBitTag()
        {
            TagDefinition tag = new TagDefinition();
            tag.TagSequence = 1;
            tag.DeviceId = 1;
            tag.TagName = "M100_0";
            tag.AddressHead = "M";
            tag.TagAddress = "100";
            tag.BitDigit = 0;
            tag.MemoryKind = MemoryKind.Bit;
            tag.DataShapeKind = DataShapeKind.Scalar;
            tag.DataType = "bool";
            tag.IsEnabled = true;

            return tag;
        }

        private static TagDefinition CreateInt32Tag()
        {
            TagDefinition tag = new TagDefinition();
            tag.TagSequence = 2;
            tag.DeviceId = 1;
            tag.BlockSequence = 1000;
            tag.TagName = "D100_INT32";
            tag.AddressHead = "D";
            tag.TagAddress = "100";
            tag.MemoryKind = MemoryKind.Word;
            tag.AddressLength = 2;
            tag.DataShapeKind = DataShapeKind.Scalar;
            tag.DataType = "int32";
            tag.IsEnabled = true;

            return tag;
        }

        private static TagDefinition CreateFloatArrayTag()
        {
            TagDefinition tag = new TagDefinition();
            tag.TagSequence = 3;
            tag.DeviceId = 1;
            tag.BlockSequence = 1000;
            tag.TagName = "D110_FLOAT_ARRAY";
            tag.AddressHead = "D";
            tag.TagAddress = "110";
            tag.MemoryKind = MemoryKind.Word;
            tag.AddressLength = 4;
            tag.DataShapeKind = DataShapeKind.Array;
            tag.ElementShapeKind = DataShapeKind.Scalar;
            tag.ElementCount = 2;
            tag.DataType = "float[2]";
            tag.IsEnabled = true;

            return tag;
        }

        private static TagDefinition CreateStringTag()
        {
            TagDefinition tag = new TagDefinition();
            tag.TagSequence = 4;
            tag.DeviceId = 1;
            tag.BlockSequence = 1000;
            tag.TagName = "D120_STRING";
            tag.AddressHead = "D";
            tag.TagAddress = "120";
            tag.MemoryKind = MemoryKind.Word;
            tag.AddressLength = 8;
            tag.DataShapeKind = DataShapeKind.Scalar;
            tag.DataType = "string[16]";
            tag.IsEnabled = true;

            return tag;
        }

        private static TagDefinition CreateInt16ArrayTag()
        {
            TagDefinition tag = new TagDefinition();
            tag.TagSequence = 5;
            tag.DeviceId = 1;
            tag.BlockSequence = 1000;
            tag.TagName = "D130_INT16_ARRAY";
            tag.AddressHead = "D";
            tag.TagAddress = "130";
            tag.MemoryKind = MemoryKind.Word;
            tag.AddressLength = 4;
            tag.DataShapeKind = DataShapeKind.Array;
            tag.ElementShapeKind = DataShapeKind.Scalar;
            tag.ElementCount = 4;
            tag.DataType = "int16[4]";
            tag.IsEnabled = true;

            return tag;
        }

        private static TagDefinition CreateDoubleTag()
        {
            TagDefinition tag = new TagDefinition();
            tag.TagSequence = 6;
            tag.DeviceId = 1;
            tag.BlockSequence = 1000;
            tag.TagName = "D140_DOUBLE";
            tag.AddressHead = "D";
            tag.TagAddress = "140";
            tag.MemoryKind = MemoryKind.Word;
            tag.AddressLength = 4;
            tag.DataShapeKind = DataShapeKind.Scalar;
            tag.DataType = "double";
            tag.IsEnabled = true;

            return tag;
        }

        private static DeviceSimulationMemoryPreset CreatePreset(string memoryHead, int startAddress, IReadOnlyList<int> values)
        {
            DeviceSimulationMemoryPreset preset = new DeviceSimulationMemoryPreset();
            preset.MemoryHead = memoryHead;
            preset.StartAddress = startAddress;
            preset.Values = values;

            return preset;
        }

        private static int[] EncodeInt32(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return EncodeWords(bytes, 2);
        }

        private static int[] EncodeDouble(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return EncodeWords(bytes, 4);
        }

        private static int[] EncodeSingles(float[] values)
        {
            List<int> words = new List<int>();
            int index;

            for (index = 0; index < values.Length; index++)
            {
                byte[] bytes = BitConverter.GetBytes(values[index]);
                int[] encoded = EncodeWords(bytes, 2);
                words.Add(encoded[0]);
                words.Add(encoded[1]);
            }

            return words.ToArray();
        }

        private static int[] EncodeAscii(string text, int wordLength)
        {
            int[] words = new int[wordLength];
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            int index;

            for (index = 0; index < bytes.Length; index += 2)
            {
                byte low = bytes[index];
                byte high = 0;

                if ((index + 1) < bytes.Length)
                {
                    high = bytes[index + 1];
                }

                words[index / 2] = low | (high << 8);
            }

            return words;
        }

        private static int[] EncodeWords(byte[] bytes, int wordLength)
        {
            int[] words = new int[wordLength];
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

                words[index] = low | (high << 8);
            }

            return words;
        }
    }
}
