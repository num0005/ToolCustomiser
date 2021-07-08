using System.IO;

namespace ToolCustomiser
{
    class DetailLevel
    {
        public DetailLevel(string name) {
            Name = name;
        }

        public readonly string Name;

        public float MaxUndividedDelta;
        public float MinimumSideLength;
        public float MaxInitialLitLength;
        public float MaxInitialUnlitLength;

        public void Write(BinaryWriter writer)
        {
            writer.Write(MaxUndividedDelta);
            writer.Write(MinimumSideLength);
            writer.Write(MaxInitialLitLength);
            writer.Write(MaxInitialUnlitLength);
        }

        public void Write(Stream stream)
        {
            Write(new BinaryWriter(stream));
        }

        public bool Read(BinaryReader reader)
        {
            MaxUndividedDelta = reader.ReadSingle();
            MinimumSideLength = reader.ReadSingle();
            MaxInitialLitLength = reader.ReadSingle();
            MaxInitialUnlitLength = reader.ReadSingle();

            return true;
        }
        public bool Read(Stream stream)
        {
            return Read(new BinaryReader(stream));
        }
    }
}
