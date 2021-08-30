using System;
using System.Collections.Generic;

namespace DuplicateFileSearcher
{
    public class FileGroup
    {
        public Int64 FileSize { get; }
        public Int32 FileCount { get { return this.Files.Count; } }
        public Int64 TotalSize { get { return this.Files.Count * FileSize; } }
        public Byte[] FileHash { get; }
        public List<FileInfo> Files;

        public FileGroup(Int64 fileSize, Byte[] fileHash)
        {
            FileSize = fileSize;
            FileHash = fileHash;
            Files = new List<FileInfo>(0);
        }

        public void AddFile(FileInfo file)
        {
            Files.Add(file);
        }
    }
}
