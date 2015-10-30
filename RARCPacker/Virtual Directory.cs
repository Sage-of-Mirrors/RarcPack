using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RarcPack
{
    public class VirtualFolder
    {
        public string Name;

        public string NodeName;

        public List<VirtualFolder> Subdirs = new List<VirtualFolder>();

        public List<FileData> Files = new List<FileData>();
    }

    public class FileData
    {
        public string Name;

        public byte[] Data;
    }
}
