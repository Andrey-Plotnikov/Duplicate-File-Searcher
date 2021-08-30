using System;
using System.Security.Cryptography;

namespace DuplicateFileSearcher
{
    public class FileInfo
    {
        public String Path { get; }
        public String Name { get { return Path.Substring(Path.LastIndexOf('\\') + 1); } }
        public DateTime CreatDateTime { get; }
        public DateTime ModifDateTime { get; }

        public FileInfo(String path)
        {
            Path = path;
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(Path);
            CreatDateTime = fileInfo.CreationTime;
            ModifDateTime = fileInfo.LastWriteTime;
        }

        public Boolean IsStillExsists() { return System.IO.Directory.Exists(Path); }

        public Byte[] GetCurrentHash()
        {
            MD5 md5 = MD5.Create();
            Byte[] hash = null;
            using (var stream = System.IO.File.OpenRead(Path)) { hash = md5.ComputeHash(stream); stream.Close(); }
            return hash;
        }

        public Int64 GetCurrentSize() { return new System.IO.FileInfo(Path).Length; }
        public DateTime GetCurrentModifDateTime() { return new System.IO.FileInfo(Path).LastWriteTime; }
    }
}
