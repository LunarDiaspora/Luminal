using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Luminal.Core
{
    public static class Filesystem
    {
        public static string DataRoot = "";
    }

    public class FileSystemObject
    {
        public string Path;
        public string AbsolutePath => System.IO.Path.GetFullPath(Path);
        public string Name => System.IO.Path.GetFileName(Path);
    }

    public class FileFilter
    {
        public List<string> ExtensionsToMatch = new();
        public string Name = "";
        public bool MatchEverything = false;

        public static List<FileFilter> Parse(string pattern)
        {
            // Patterns:
            // Project files,.luminal/.lpr;All files,*

            var pat = new List<FileFilter>();

            var chunks = pattern.Split(";");
            foreach (var ch in chunks)
            {
                var sp = ch.Split(",");
                if (sp.Length != 2) throw new Exception("Malformed pattern.");
                var name = sp[0];
                var matches = sp[1];

                var fil = new FileFilter();
                fil.Name = name;
                fil.MatchEverything = matches == "*";
                fil.ExtensionsToMatch = matches.Split("/").ToList();

                pat.Add(fil);
            }

            return pat;
        }

        public bool Matches(string path)
        {
            return MatchEverything ||
                ExtensionsToMatch.Any(j => Path.GetExtension(path) == j);
        }

        public override string ToString()
        {
            return $"{Name} ({(MatchEverything ? "*.*" : string.Join(", ", ExtensionsToMatch))})";
        }
    }

    public class FSDirectory : FileSystemObject
    {
        List<FSDirectory> _Children = new();

        public List<FSDirectory> Children
        {
            get
            {
                if (!loaded)
                {
                    Find();
                }

                return _Children;
            }
        }

        public List<FSFile> Files = new();
        public List<FileFilter> Filters = null;
        bool loaded = false;

        public FSDirectory(string where, List<FileFilter> filter = null, bool recurse = true)
        {
            Path = where;

            Filters = filter;

            if (recurse) Find();
        }

        public static bool CanAccess(string path)
        {
            try
            {
                Directory.GetLastAccessTime(path);
                return true;
            } catch(UnauthorizedAccessException)
            {
                return false;
            }
        }

        public void Find()
        {
            if (!CanAccess(Path))
                return;

            IEnumerable<string> subdirectories = new List<string>();
            try
            {
                var di = new DirectoryInfo(Path);
                subdirectories = di.GetDirectories("*", new EnumerationOptions()
                {
                    IgnoreInaccessible = true,
                    AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
                    ReturnSpecialDirectories = false
                }).Select(e => e.FullName); // holy shit this is bad
            } catch(UnauthorizedAccessException)
            {
                // WINDOWS
            }
            // Subdirs

            foreach (var sd in subdirectories)
            {
                var nd = new FSDirectory(System.IO.Path.Combine(Path, sd), null, false);
                _Children.Add(nd);
            }

            var files = Directory.GetFiles(Path);
            // Files

            foreach (var fp in files)
            {
                var f = new FSFile(System.IO.Path.Combine(Path, fp), this);

                if (Filters != null)
                {
                    if (Filters.All(h => !h.Matches(f.Path)))
                        continue;
                }

                Files.Add(f);
            }

            loaded = true;
        }
    }

    public class FSFile : FileSystemObject
    {
        public string FileName => System.IO.Path.GetFileName(Path);
        public long FileSize => new FileInfo(Path).Length;
        public string HumanFileSize => WinAPIUtilities.BytesToString(FileSize);

        FileStream _Stream = null;

        public FSDirectory Parent;

        public FileStream Stream
        {
            get
            {
                if (_Stream == null)
                {
                    return OpenStream();
                }

                return _Stream;
            }
        }

        public FileStream OpenStream()
        {
            var strm = File.Open(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);

            if (_Stream == null)
            {
                _Stream = strm;
            }

            return strm;
        }

        public void CloseStream()
        {
            if (_Stream != null)
                _Stream.Dispose();
        }

        public FSFile(string where, FSDirectory parent = null)
        {
            Path = where;
            Parent = parent;

            if (Parent == null)
            {
                Parent = new FSDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(where)));
            }
        }
    }
}
