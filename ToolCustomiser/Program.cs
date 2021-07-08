using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ToolCustomiser
{
    class Program
    {
        static bool IsPEFile(string file)
        {
            if (!File.Exists(file))
                return false;
            using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                if (reader.ReadUInt16() != 0x5A4D) // MZ
                    return false;
                reader.BaseStream.Seek(0x3A, SeekOrigin.Current); // skip MS-DOS header info
                uint pe_header_offset = reader.ReadUInt32();
                reader.BaseStream.Seek(pe_header_offset, SeekOrigin.Begin); // seek to PE header
                return reader.ReadUInt32() == 0x4550; // == "PE\0\0"
            }
        }

        static void CLIDetialLevel(DetailLevel level)
        {
            Console.WriteLine($"[Lod] {level.Name}");
            Console.WriteLine($"[Max Undivided Delta] {level.MaxUndividedDelta}");
            Console.WriteLine($"[Minimum Side Length] {level.MinimumSideLength}");
            Console.WriteLine($"[Max Initial Lit length] {level.MaxInitialLitLength}");
            Console.WriteLine($"[Max Initial Unlit length] {level.MaxInitialUnlitLength}");
            while (true)
            {
                string[] commands = new[] { "Back", "Undivided Delta Max", "Side Length Min", "Lit Length", "unlit Length" };
                string command = Sharprompt.Prompt.Select("Commands", commands);
                switch (command[0])
                {
                    case 'B':
                        return;
                    case 'U':
                        level.MaxUndividedDelta = Sharprompt.Prompt.Input<float>("Enter new max undivided delta", level.MaxUndividedDelta);
                        continue;
                    case 'S':
                        level.MinimumSideLength = Sharprompt.Prompt.Input<float>("Enter new minimum side length", level.MinimumSideLength);
                        continue;
                    case 'u':
                        level.MaxInitialLitLength = Sharprompt.Prompt.Input<float>("Enter new max initial unlit length", level.MaxInitialLitLength);
                        continue;
                    case 'L':
                        level.MaxInitialUnlitLength = Sharprompt.Prompt.Input<float>("Enter new max initial lit length", level.MaxInitialUnlitLength);
                        continue;
                }
            }
        }

        static void CLIQuality(QualityLevel level, string quality)
        {
            Console.WriteLine($"[Quality] {quality}");
            Console.WriteLine($"[Default convergence] {level.DefaultConvergence}");
            Console.WriteLine($"[square root of samples per sky light] {level.SamplesPerSkyLight}");

            Console.WriteLine("[LODs] = {");
            foreach (DetailLevel lod in level.LODs)
            {
                Console.WriteLine($"\t [{lod.Name}] " +
                    $"\t [Max Undivided Delta] {lod.MaxUndividedDelta} " +
                    $"\t [Minimum Side Length] {lod.MinimumSideLength} " +
                    $"\t [Max Initial Lit length] {lod.MaxInitialLitLength} " +
                    $"\t [Max Initial Unlit length] {lod.MaxInitialUnlitLength}");
            }
            Console.WriteLine("}");

            while (true)
            {
                string[] commands = new[] { "Back", "Default convergence", "Sky Samples", "LODs" };
                string command = Sharprompt.Prompt.Select("Commands", commands);
                switch (command[0])
                {
                    case 'B':
                        return;
                    case 'D':
                        level.DefaultConvergence = Sharprompt.Prompt.Input<float>("Enter new default convergence", level.DefaultConvergence);
                        continue;
                    case 'S':
                        level.SamplesPerSkyLight = Sharprompt.Prompt.Input<short>("Enter new square root of samples per sky light", level.SamplesPerSkyLight);
                        continue;
                    case 'L':
                        {
                            string selection = Sharprompt.Prompt.Select("Select LOD to edit", new string[] { "Cancel", "High", "Medium", "Low", "Turd" });
                            switch (selection[0])
                            {
                                case 'C':
                                    break;
                                case 'H':
                                    CLIDetialLevel(level.LODs[0]);
                                    break;
                                case 'M':
                                    CLIDetialLevel(level.LODs[0]);
                                    break;
                                case 'L':
                                    CLIDetialLevel(level.LODs[0]);
                                    break;
                                case 'T':
                                    CLIDetialLevel(level.LODs[0]);
                                    break;
                            }
                            continue;
                        }
                }
            }
        }

        static void CLIMain(FileStream file, CustomiserConfig customiserConfig, long? CustomizerOffset)
        {
            file.Seek(customiserConfig.Offset, SeekOrigin.Begin);
            ToolConfig toolConfig = new();

            if (!toolConfig.Read(file))
            {
                Console.WriteLine($"Failed to read tool config!");
                return;
            }

            while (true)
            {
                string[] topLevelCommands = new[] { "exit", "save", "debug", "final" };
                string command = Sharprompt.Prompt.Select("Commands", topLevelCommands);
                switch (command[0])
                {
                    case 'e':
                        return;
                    case 's':
                        Console.WriteLine("Saving configuration!");
                        //string newFileName = Sharprompt.Prompt.Input("File name", )

                        CustomizerOffset ??= file.Length; // default to EOF
                        file.Seek(CustomizerOffset.Value, SeekOrigin.Begin);
                        customiserConfig.Write(file);

                        file.Seek(customiserConfig.Offset, SeekOrigin.Begin);
                        toolConfig.Write(file);

                        Console.WriteLine("Done!");
                        break;
                    case 'd':
                        CLIQuality(toolConfig.DebugQuality, "Debug");
                        break;
                    case 'f':
                        CLIQuality(toolConfig.FinalQuality, "Final");
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string fileName = args[0];
                if (IsPEFile(fileName))
                {
                    using (FileStream file = File.Open(fileName, FileMode.Open))
                    {
                        CustomiserConfig customiserConfig = new();
                        // read customizer config if any
                        long? customizerOffset = StreamSearch.Find(file, CustomiserConfig.GuidArray);
                        if (customizerOffset is not null && 
                            file.Seek(customizerOffset.Value, SeekOrigin.Begin) == customizerOffset 
                            && customiserConfig.Read(file))
                        {
                            Console.WriteLine($"Read customizer config from \"{fileName}\"");
                            CLIMain(file, customiserConfig, customizerOffset);
                        } else
                        {
                            file.Seek(0, SeekOrigin.Begin); // reset
                            long? found = DefaultConfigScanner.Find(file);
                            if (found is null)
                            {
                                Console.WriteLine($"Can't find default config in \"{fileName}\" , are you sure this a HEK executabe?");
                            } else
                            {
                                Console.WriteLine($"Scanner found default configuration at {found.Value}");
                                customiserConfig.Offset = found.Value;
                                CLIMain(file, customiserConfig, null);
                            }
                        }
                    }
                } else
                {
                    Console.WriteLine($"\"{fileName}\" is not a PE file or doesn't exist");
                }
                 
            }
            else
            {
                Console.WriteLine(Process.GetCurrentProcess().MainModule.ModuleName + " <path to tool.exe>");
            }
        }
    }
}
