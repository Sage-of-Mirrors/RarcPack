using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFormatReader.Common;

namespace RarcPack
{
    public class RARCHeader
    {
        public string Magic;
        public int FileSize;
        public int Unknown1;
        public int DataOffset;

        public int Unknown2;
        public int Unknown3;
        public int Unknown4;
        public int Unknown5;

        public int NodeCount;

        public int Unknown6;
        public int Unknown7;

        public int FileEntriesOffset;

        public int Unknown8;

        public int StringTableOffset;

        public short FileEntryCount;
        public byte UnknownBool1;
        public byte Padding;
        public int Unknown10;

        public void Write(EndianBinaryWriter writer)
        {
            writer.WriteFixedString(Magic, 4);
            writer.Write(FileSize);
            writer.Write(Unknown1);
            writer.Write(DataOffset);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(Unknown4);
            writer.Write(Unknown5);
            writer.Write(NodeCount);
            writer.Write(Unknown6);
            writer.Write(Unknown7);
            writer.Write(FileEntriesOffset);
            writer.Write(Unknown8);
            writer.Write(StringTableOffset);
            writer.Write(FileEntryCount);
            writer.Write(UnknownBool1);
            writer.Write(Padding);
            writer.Write(Unknown10);
        }
    }

    public class RARCNode
    {
        public string Type;
        public int NameOffset;

        public short NameHash;

        public short FileEntryCount;
        public int FirstFileEntryIndex;

        public void Write(EndianBinaryWriter writer)
        {
            writer.WriteFixedString(Type, 4);
            writer.Write(NameOffset);
            writer.Write(NameHash);
            writer.Write(FileEntryCount);
            writer.Write(FirstFileEntryIndex);
        }
    }

    public class RARCFileEntry
    {
        public short FileId;
        public short NameHash;
        public byte Type;
        public byte Padding;
        public short NameOffset;
        public int DataOffset;
        public int DataSize;
        public int Zero;

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(FileId);
            writer.Write(NameHash);
            writer.Write(Type);
            writer.Write(Padding);
            writer.Write(NameOffset);
            writer.Write(DataOffset);
            writer.Write(DataSize);
            writer.Write(Zero);
        }
    }
}
