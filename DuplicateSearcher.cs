using System;
using System.IO;
using System.Collections.Generic;

namespace DuplicateFileSearcher
{
    class DuplicateSearchResult
    {
        public Int32 totalGroup { get; set; }
        public Int32 totalFiles { get; set; }
        public Int64 totalSize { get; set; }
        public Int32 filesMissed { get; set; }
        public Int32 foldersMissed { get; set; }

        public DuplicateSearchResult()
        {
            totalGroup = 0;
            totalFiles = 0;
            totalSize = 0;
            filesMissed = 0;
            foldersMissed = 0;
        }
    }
    class DuplicateSearcher
    {
        private String searchDirectory;
        private Boolean isSubDir;
        public List<FileGroup> DuplicateGroupList = null;
        public DuplicateSearchResult searchResult = null;

        public DuplicateSearcher(String srchDir, Boolean isSub)
        {
            searchDirectory = srchDir;
            isSubDir = isSub;
        }

        public List<FileGroup> StartSearch()
        {
            SortedList<Int64, List<String>> sameSizeFiles = GetSameSizeFiles();
            searchResult = new DuplicateSearchResult();
            DuplicateGroupList = FindDuplicates(sameSizeFiles);
            return DuplicateGroupList;
        }

        private List<FileGroup> FindDuplicates(SortedList<Int64, List<String>> sameSizeFiles)
        {
            List<FileGroup> duplicatesList = new List<FileGroup>(0);

            for (Int32 i = 0; i < sameSizeFiles.Count; i++)
                duplicatesList.AddRange(CompareFilesByHash(sameSizeFiles.Values[i], sameSizeFiles.Keys[i]));

            for (Int32 i = 0; i < duplicatesList.Count; i++)
            {
                if (duplicatesList[i].FileCount == 1)
                {
                    duplicatesList.RemoveAt(i);
                    i--;
                }
            }
            searchResult.totalGroup = duplicatesList.Count;
            foreach (var g in duplicatesList)
            {
                searchResult.totalFiles += g.FileCount;
                searchResult.totalSize += g.TotalSize;
            }

            return duplicatesList;
        }


        private List<FileGroup> CompareFilesByHash(List<String> fileList, Int64 groupSize)
        {
            List<FileGroup> duplicatesList = new List<FileGroup>(0);
            List<FileInfo> files = new List<FileInfo>(0);
            List<Byte[]> fileHashList = new List<Byte[]>(0);

            for (Int32 i = 0; i < fileList.Count; i++)
            {
                files.Add(new FileInfo(fileList[i]));
                fileHashList.Add(files[i].GetCurrentHash());
            }

            Int32 fileGroupNum = 0;
            for (Int32 i = 0; i < fileHashList.Count; i++)
            {
                duplicatesList.Add(new FileGroup(groupSize, fileHashList[i]));
                duplicatesList[fileGroupNum].AddFile(files[i]);

                for (Int32 j = i + 1; j < fileHashList.Count; j++)
                {
                    if (СompareHashes(fileHashList[i], fileHashList[j]) == true)
                    {
                        duplicatesList[fileGroupNum].AddFile(files[j]);
                        fileHashList.RemoveAt(j);
                        files.RemoveAt(j);
                        j--;
                    }
                }
                fileHashList.RemoveAt(i);
                files.RemoveAt(i);
                i--;
                fileGroupNum++;
            }

            return duplicatesList;
        }



        private Boolean СompareHashes(Byte[] hash1, Byte[] hash2)
        {
            for (Int32 i = 0; i < hash1.Length; i++) if (hash1[i] != hash2[i]) return false;
            return true;
        }

        private SortedList<Int64, List<String>> GetSameSizeFiles()
        {
            SortedList<Int64, List<String>> sameSizeFiles = new SortedList<Int64, List<String>>(0);
            List<String> files = GetFiles(searchDirectory, "*");

            foreach (String file in files)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
                Int64 fileSize = fileInfo.Length;

                if (fileSize > 0)
                {
                    if (sameSizeFiles.GetValueOrDefault(fileSize) == null) sameSizeFiles.Add(fileSize, new List<String>(0));
                    sameSizeFiles[fileSize].Add(file);
                }
            }

            for (Int32 i = 0; i < sameSizeFiles.Count; i++)
            {
                if (sameSizeFiles.Values[i].Count < 2)
                {
                    sameSizeFiles.RemoveAt(i);
                    i--;
                }
            }

            return sameSizeFiles;
        }

        private List<String> GetFiles(String path, String pattern)
        {
            var files = new List<String>(0);

            try
            {
                files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                if (isSubDir == true) foreach (var directory in Directory.GetDirectories(path)) files.AddRange(GetFiles(directory, pattern));
            }
            catch (UnauthorizedAccessException) { }

            return files;
        }
    }
}
