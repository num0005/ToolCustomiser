using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace ToolCustomiser
{
    class ExternalConfig
    {
        public ExternalConfig(ToolConfig toolConfig)
        {
            ToolConfig = toolConfig;
        }

        public ExternalConfig()
        {
        }

        public readonly ToolConfig ToolConfig = new();

        public void Write(BinaryWriter writer)
        {
            // header
            writer.Write(_guidArray);
            writer.Write(_version);

            ToolConfig.Write(writer);
        }

        public void Write(Stream stream)
        {
            Write(new BinaryWriter(stream));
        }

        public bool Read(BinaryReader reader)
        {
            // header
            byte[] header = reader.ReadBytes(_guidArray.Length);
            if (!header.SequenceEqual(_guidArray))
                return false; // bad header/GUID
            if (reader.ReadUInt32() != _version)
                return false; // unsupported version

            return ToolConfig.Read(reader);
        }

        public bool Read(Stream stream)
        {
            return Read(new BinaryReader(stream));
        }

        // {B9EC284E-B1B7-4BBB-84BF-3682CC1E9D76}
        private readonly static Guid _guid = new(0xb9ec284e, 0xb1b7, 0x4bbb, 0x84, 0xbf, 0x36, 0x82, 0xcc, 0x1e, 0x9d, 0x76);
        private readonly static byte[] _guidArray = _guid.ToByteArray();

        private readonly static uint _version = 0;
    }
}
