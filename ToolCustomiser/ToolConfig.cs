using System.IO;

namespace ToolCustomiser
{
    class ToolConfig
    {
        public void Write(BinaryWriter writer)
        {
            DebugQuality.Write(writer);
            FinalQuality.Write(writer);
        }

        public void Write(Stream stream)
        {
            Write(new BinaryWriter(stream));
        }

        public bool Read(BinaryReader reader)
        {
            return DebugQuality.Read(reader) && FinalQuality.Read(reader);
        }

        public bool Read(Stream stream)
        {
            return Read(new BinaryReader(stream));
        }

        public readonly QualityLevel DebugQuality = new();
        public readonly QualityLevel FinalQuality = new();
    }
}
