using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ToolCustomiser
{
    class CustomiserConfig
    {
        /// <summary>
        /// Offset of the tool configuration in this executable
        /// </summary>
        public long Offset;

        public void Write(BinaryWriter writer)
        {
            writer.Write(GuidArray);
            writer.Write(_version);
            writer.Write(Offset);
        }

        public void Write(Stream stream)
        {
            Write(new BinaryWriter(stream));
        }

        public bool Read(BinaryReader reader)
        {
            byte[] header = reader.ReadBytes(GuidArray.Length);
            if (!header.SequenceEqual(GuidArray))
                return false; // bad header/GUID
            if (reader.ReadUInt32() != _version)
                return false; // unsupported version
            Offset = reader.ReadInt64();
            return true;
        }

        public bool Read(Stream stream)
        {
            return Read(new BinaryReader(stream));
        }

        // {B6FB965C-07EE-4EFA-BE1A-D28BF4E9AC18}
        public readonly static Guid Guid = new(0xb6fb965c, 0x7ee, 0x4efa, 0xbe, 0x1a, 0xd2, 0x8b, 0xf4, 0xe9, 0xac, 0x18);
        public readonly static byte[] GuidArray = Guid.ToByteArray();

        /// <summary>
        /// Length when serialized
        /// </summary>
        public readonly static int SerializedLength = GuidArray.Length + sizeof(uint) + sizeof(ulong);

        private readonly static uint _version = 0;
    }
}
