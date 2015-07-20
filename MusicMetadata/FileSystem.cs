using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MusicMetadata
{
    static class FileSystem
    {
        static public IEnumerable<DirectoryInfo> QueryLeafFoldersOf(string path)
        {
            Debug.WriteLine("QueryLeafFoldersOf({0}) on Thread {1}", path, Thread.CurrentThread.ManagedThreadId);
            var folder = new DirectoryInfo(path);
            var subFolders = folder.EnumerateDirectoriesSafe("*", SearchOption.AllDirectories);
            return subFolders.Except(subFolders.Select(x => x.Parent), new DirectoryInfoEqualityComparer());
        }

        static public IEnumerable<Metadata> QueryMetadataOf(string path)
        {
            Debug.WriteLine("QueryMetadataOf({0}) on Thread {1}", path, Thread.CurrentThread.ManagedThreadId);
            var folder = new DirectoryInfo(path);
            var metadataPaths = folder.EnumerateFiles("*.wav").Select(x => Regex.Replace(x.FullName, @"^(.*\.)wav$", "$1xml", RegexOptions.IgnoreCase));
            return MetadataFactory.MaterializeFor(metadataPaths);
        }

        static public void Save(Metadata metadataFile)
        {
            MetadataStore.Write(metadataFile);
        }
    }

    static class FileSystemExtensions
    {
        static public IEnumerable<DirectoryInfo> EnumerateDirectoriesSafe(this DirectoryInfo directoryInfo, string searchPattern, SearchOption searchOption)
        {
            if (directoryInfo == null)
                throw new ArgumentNullException("directoryInfo");

            try
            {
                if (directoryInfo.Exists)
                {
                    return directoryInfo.EnumerateDirectories(searchPattern, searchOption);
                }
                else
                {
                    return Enumerable.Empty<DirectoryInfo>();
                }
            }
            catch (SecurityException)
            {
                return Enumerable.Empty<DirectoryInfo>();
            }
        }
    }

    class DirectoryInfoEqualityComparer : IEqualityComparer<DirectoryInfo>
    {

        bool IEqualityComparer<DirectoryInfo>.Equals(DirectoryInfo x, DirectoryInfo y)
        {
            return x != null && y != null && x.FullName == y.FullName;
        }

        int IEqualityComparer<DirectoryInfo>.GetHashCode(DirectoryInfo obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}
