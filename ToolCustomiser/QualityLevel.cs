using System.IO;

namespace ToolCustomiser
{
    class QualityLevel
    {
        public float DefaultConvergence;
        /// <summary>
        /// Square root of the value
        /// </summary>
        public short SamplesPerSkyLight;

        public readonly DetailLevel[] LODs = { new("High"), new("Medium"), new("Low"), new("Turd") };

        public void Write(BinaryWriter writer)
        {
            writer.Write(DefaultConvergence);
            writer.Write((int)SamplesPerSkyLight); // cast to add padding

            foreach (DetailLevel lod in LODs)
                lod.Write(writer);
        }

        public void Write(Stream stream)
        {
            Write(new BinaryWriter(stream));
        }

        public bool Read(BinaryReader reader)
        {
            DefaultConvergence = reader.ReadSingle();
            SamplesPerSkyLight = reader.ReadInt16();
            reader.BaseStream.Seek(2, SeekOrigin.Current); // skip pad

            bool success = true;

            foreach (DetailLevel lod in LODs)
                success = success && lod.Read(reader);

            return success;
        }

        public bool Read(Stream stream)
        {
            return Read(new BinaryReader(stream));
        }
    }
}
