using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace troll_ui_app
{
    public static class SafeFileEnumerator
    {
        //public static IEnumerable<string> EnumerateDirectories(string parentDirectory, string searchPattern, SearchOption searchOpt)
        //{
        //    try
        //    {
        //        var directories = Enumerable.Empty<string>();
        //        if (searchOpt == SearchOption.AllDirectories)
        //        {
        //            directories = Directory.EnumerateDirectories(parentDirectory)
        //                .SelectMany(x => EnumerateDirectories(x, searchPattern, searchOpt));
        //        }
        //        return directories.Concat(Directory.EnumerateDirectories(parentDirectory, searchPattern));
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        return Enumerable.Empty<string>();
        //    }
        //}
        public static IEnumerable<string> EnumerateDirectories(string parentDirectory, string searchPattern, SearchOption searchOpt, Func<String, bool> directoryFilter=null)
        {
            try
            {
                var directories = Enumerable.Empty<string>();
                if ( directoryFilter!=null && !directoryFilter(parentDirectory))
                    return directories;
                if (searchOpt == SearchOption.AllDirectories)
                {
                    directories = Directory.EnumerateDirectories(parentDirectory)
                        .SelectMany(x => EnumerateDirectories(x, searchPattern, searchOpt, directoryFilter));
                }
                return directories.Concat(Directory.EnumerateDirectories(parentDirectory, searchPattern));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Enumerable.Empty<string>();
            }
            catch (Exception e)
            {
                return Enumerable.Empty<string>();
            }
        }

        //public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOpt)
        //{
        //    try
        //    {
        //        var dirFiles = Enumerable.Empty<string>();
        //        if (searchOpt == SearchOption.AllDirectories)
        //        {
        //            dirFiles = Directory.EnumerateDirectories(path)
        //                                .SelectMany(x => EnumerateFiles(x, searchPattern, searchOpt));
        //        }
        //        return dirFiles.Concat(Directory.EnumerateFiles(path, searchPattern));
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        return Enumerable.Empty<string>();
        //    }
        //}
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOpt, Func<String, bool> directoryFilter=null)
        {
            try
            {
                var dirFiles = Enumerable.Empty<string>();
                if ( directoryFilter!=null && !directoryFilter(path))
                    return dirFiles;
                if (searchOpt == SearchOption.AllDirectories)
                {
                    dirFiles = Directory.EnumerateDirectories(path)
                                        .SelectMany(x => EnumerateFiles(x, searchPattern, searchOpt, directoryFilter));
                }
                return dirFiles.Concat(Directory.EnumerateFiles(path, searchPattern));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Enumerable.Empty<string>();
            }
            catch(Exception e)
            {
                return Enumerable.Empty<string>();
            }
        }
        public static IEnumerable<FileInfo> EnumerateFileInfos(string path, string searchPattern, SearchOption searchOpt, Func<string, bool> directoryFilter=null)
        {
            try
            {
                var dirFiles = Enumerable.Empty<FileInfo>();
                if ( directoryFilter!=null && !directoryFilter(path))
                    return dirFiles;
                if (searchOpt == SearchOption.AllDirectories)
                {
                    dirFiles = Directory.EnumerateDirectories(path)
                                        .SelectMany(x => EnumerateFileInfos(x, searchPattern, searchOpt, directoryFilter));
                }
                DirectoryInfo pathinfo = new DirectoryInfo(path);
                return dirFiles.Concat(pathinfo.EnumerateFiles(searchPattern));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Enumerable.Empty<FileInfo>();
            }
            catch(Exception e)
            {
                return Enumerable.Empty<FileInfo>();
            }
        }
    }
}
