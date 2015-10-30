using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GameFormatReader.Common;

namespace RarcPack
{
    public class RARCPacker
    {
        RARCHeader Header;

        List<RARCNode> Nodes;

        List<RARCFileEntry> Entries;

        List<Char> StringTable;

        List<byte> Data;

        int EntryCount;

        public void Pack(VirtualFolder root, EndianBinaryWriter writer)
        {
            Header = new RARCHeader();

            Header.Magic = "RARC";

            Header.Unknown1 = 0x20;

            Header.Unknown6 = 0x20;

            Nodes = new List<RARCNode>();

            Entries = new List<RARCFileEntry>();

            StringTable = new List<char>();

            Data = new List<byte>();

            StringTable.Add('.');

            StringTable.Add('\0');

            StringTable.Add('.');

            StringTable.Add('.');

            StringTable.Add('\0');

            RARCNode rootNode = new RARCNode();

            rootNode.Type = "ROOT";

            rootNode.NameOffset = 5;

            rootNode.NameHash = HashName(root.Name);

            foreach (char c in root.Name)
            {
                StringTable.Add(c);
            }

            StringTable.Add('\0');

            rootNode.FileEntryCount = (short)(root.Subdirs.Count + root.Files.Count + 2);

            rootNode.FirstFileEntryIndex = 0;

            Nodes.Add(rootNode);

            EntryCount = rootNode.FileEntryCount;

            foreach (VirtualFolder folder in root.Subdirs)
            {
                RecursiveDir(folder, rootNode);
            }

            foreach (FileData file in root.Files)
            {
                Entries.Add(AddFileFileEntry(file));
            }

            #region Add first two period entries. Not using the method I created because of course these two have a slightly different format....

            RARCFileEntry singlePeriod = new RARCFileEntry();

            singlePeriod.FileId = -1;

            singlePeriod.NameHash = 0x2E;

            singlePeriod.Type = 2;

            singlePeriod.Padding = 0;

            singlePeriod.NameOffset = 0;

            singlePeriod.DataOffset = 0;

            singlePeriod.DataSize = 0x10;

            singlePeriod.Zero = 0;

            Entries.Add(singlePeriod);

            RARCFileEntry doublePeriod = new RARCFileEntry();

            doublePeriod.FileId = -1;

            doublePeriod.NameHash = 0xB8;

            doublePeriod.Type = 2;

            doublePeriod.Padding = 0;

            doublePeriod.NameOffset = 2;

            doublePeriod.DataOffset = -1;

            doublePeriod.DataSize = 0x10;

            doublePeriod.Zero = 0;

            Entries.Add(doublePeriod);

            #endregion

            foreach (VirtualFolder folder in root.Subdirs)
            {
                RecursiveFile(folder);
            }

            Header.NodeCount = Nodes.Count;

            Header.Unknown2 = Data.Count;

            Header.Unknown3 = Data.Count;

            Header.Unknown7 = Entries.Count;

            Header.FileEntryCount = (short)Entries.Count;

            Header.UnknownBool1 = 1;

            int headerLength = 64;

            int unalignedData = (headerLength + (Nodes.Count * 16) + (Entries.Count * 20) + StringTable.Count);

            int alignedEntries = ((Entries.Count * 0x14) + 0x1F) & ~0x1F;

            int alignedNodes = ((Nodes.Count * 0x10) + 0x1F) & ~0x1F;

            int alignedTable = alignedNodes + alignedEntries + headerLength;

            int alignedStringTableSize = (StringTable.Count + 0x1F) & ~0x1F;

            int alignedData = (alignedTable + alignedStringTableSize + 0x1F) & ~0x1F;

            Header.Unknown8 = alignedStringTableSize;

            Header.FileEntriesOffset = alignedNodes + 64 - 0x20;

            Header.StringTableOffset = alignedTable - 0x20;

            Header.DataOffset = alignedData - 0x20;

            Header.FileSize = (alignedData + Data.Count + 0x1F) & ~0x1F;

            Write(writer);
        }

        short HashName(string name)
        {
            short hash = 0;

            short multiplier = 1;

            if (name.Length == 2)
            {
                multiplier = 2;
            }

            if (name.Length >= 3)
            {
                multiplier = 3;
            }

            foreach (char c in name)
            {
                hash = (short)(hash * multiplier);
                hash += (short)c;
            }

            return hash;
        }

        void AddPeriodEntries(int nodeIndex)
        {
            RARCFileEntry singlePeriod = new RARCFileEntry();

            singlePeriod.FileId = -1;

            singlePeriod.NameHash = 0x2E;

            singlePeriod.Type = 2;

            singlePeriod.Padding = 0;

            singlePeriod.NameOffset = 0;

            singlePeriod.DataOffset = nodeIndex;

            singlePeriod.DataSize = 0x10;

            singlePeriod.Zero = 0;

            Entries.Add(singlePeriod);

            RARCFileEntry doublePeriod = new RARCFileEntry();

            doublePeriod.FileId = -1;

            doublePeriod.NameHash = 0xB8;

            doublePeriod.Type = 2;

            doublePeriod.Padding = 0;

            doublePeriod.NameOffset = 2;

            doublePeriod.DataOffset = 0;

            doublePeriod.DataSize = 0x10;

            doublePeriod.Zero = 0;

            Entries.Add(doublePeriod);
        }

        RARCFileEntry AddFileFileEntry(FileData file)
        {
            RARCFileEntry entry = new RARCFileEntry();

            entry.FileId = (short)Entries.Count;

            entry.NameHash = HashName(file.Name);

            entry.Type = 0x11;

            entry.Padding = 0;

            entry.NameOffset = (short)StringTable.Count;

            foreach (char c in file.Name)
            {
                StringTable.Add(c);
            }

            StringTable.Add('\0');

            entry.DataOffset = Data.Count;

            Data.AddRange(file.Data);

            entry.DataSize = file.Data.Length;

            entry.Zero = 0;

            return entry;
        }

        void RecursiveDir(VirtualFolder folder, RARCNode rootNode)
        {
            RARCNode subdirNode = new RARCNode();

            subdirNode.Type = folder.NodeName;

            subdirNode.NameOffset = StringTable.Count;

            foreach (char c in folder.Name)
            {
                StringTable.Add(c);
            }

            subdirNode.NameHash = HashName(folder.Name);

            subdirNode.FirstFileEntryIndex = EntryCount;

            subdirNode.FileEntryCount = (short)(folder.Subdirs.Count + folder.Files.Count + 2);

            EntryCount += (folder.Subdirs.Count + folder.Files.Count + 2);

            StringTable.Add('\0');

            Nodes.Add(subdirNode);

            RARCFileEntry subdirEntry = new RARCFileEntry();

            subdirEntry.FileId = -1;

            subdirEntry.NameHash = HashName(folder.Name);

            subdirEntry.Type = 2;

            subdirEntry.Padding = 0;

            subdirEntry.NameOffset = (short)subdirNode.NameOffset;

            subdirEntry.DataOffset = Nodes.IndexOf(subdirNode);

            subdirEntry.DataSize = 0x10;

            subdirEntry.Zero = 0;

            Entries.Add(subdirEntry);

            foreach (VirtualFolder dir in folder.Subdirs)
            {
                RecursiveDir(dir, subdirNode);
            }
        }

        void RecursiveFile(VirtualFolder folder)
        {
            foreach (FileData file in folder.Files)
            {
                Entries.Add(AddFileFileEntry(file));
            }

            AddPeriodEntries(0);

            foreach (VirtualFolder dir in folder.Subdirs)
            {
                RecursiveFile(dir);
            }
        }

        void Write(EndianBinaryWriter writer)
        {
            Header.Write(writer);

            Pad32(writer);

            foreach (RARCNode node in Nodes)
            {
                node.Write(writer);
            }

            Pad32(writer);

            foreach (RARCFileEntry entry in Entries)
            {
                entry.Write(writer);
            }

            Pad32(writer);

            writer.Write(StringTable.ToArray());

            Pad32(writer);

            writer.Write(Data.ToArray());
        }

        void Pad32(EndianBinaryWriter writer)
        {
            // Pad up to a 32 byte alignment Formula:
            // (x + (n-1)) & ~(n-1)
            long nextAligned = (writer.BaseStream.Length + 0x1F) & ~0x1F;

            long delta = nextAligned - writer.BaseStream.Length;
            writer.BaseStream.Position = writer.BaseStream.Length;
            writer.Write(new byte[delta]);
        }
    }
}
