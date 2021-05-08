using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Luminal.OpenGL;

namespace Luminal.Core
{
    public static class WinAPIUtilities
    {
        public static IntPtr GetHwnd()
        {
            SDL_version h;
            SDL_VERSION(out h);
            SDL_SysWMinfo t = new();
            t.version = h;
            SDL_GetWindowWMInfo(Engine.Window, ref t);
            return t.info.win.window;
        }

        public static Rectangle WorkArea()
        {
            var t = Screen.FromHandle(GetHwnd()).WorkingArea;
            var lr = new Rectangle()
            {
                x = t.X,
                y = t.Y,
                w = t.Width,
                h = t.Height
            };
            Logging.Log.Debug($"{lr.x} {lr.y} - {lr.w} {lr.h}");
            return lr;
        }

        public const int FILE_ATTRIBUTE_NORMAL = 0x80;
        public const int SHGFI_TYPENAME = 0x400;
        public const int SHGFI_ICON = 0x100;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            int dwFileAttributes,
            ref SHFILEINFO shinfo,
            uint cbfileInfo,
            int uFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }

            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        private static Dictionary<string, string> fileInfoCache = new();
        private static Dictionary<string, GLTexture> iconCache = new();

        public static string GetFileType(string filename)
        {
            var ext = Path.GetExtension(filename);
            if (fileInfoCache.ContainsKey(ext))
            {
                return fileInfoCache[ext];
            }

            var shfi = new SHFILEINFO(true);

            const int flags = FILE_ATTRIBUTE_NORMAL | SHGFI_TYPENAME;

            if (SHGetFileInfo(filename, FILE_ATTRIBUTE_NORMAL, ref shfi, (uint)Marshal.SizeOf(shfi), flags) == IntPtr.Zero)
            {
                var bad = $"{ext[1..].ToUpper()} File";

                fileInfoCache[ext] = bad;

                return bad;
            }

            fileInfoCache[ext] = shfi.szTypeName;

            return shfi.szTypeName;
        }

        public static GLTexture GetIcon(string filename)
        {
            var ext = Path.GetExtension(filename);
            if (iconCache.ContainsKey(ext))
            {
                return iconCache[ext];
            }

            var shfi = new SHFILEINFO(true);

            const int flags = FILE_ATTRIBUTE_NORMAL | SHGFI_ICON | SHGFI_TYPENAME;
            if (SHGetFileInfo(filename, FILE_ATTRIBUTE_NORMAL, ref shfi, (uint)Marshal.SizeOf(shfi), flags) == IntPtr.Zero)
            {
                // Oops
                return null;
            }

            var icon = Bitmap.FromHicon(shfi.hIcon);
            var rect = new System.Drawing.Rectangle(0, 0, icon.Width, icon.Height);
            var pf = PixelFormat.Format32bppArgb;
            var nicon = icon.Clone(rect, pf);

            var tex = new GLTexture($"Icon: {shfi.szTypeName}", nicon);

            iconCache[ext] = tex;

            return tex;
        }

        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0 " + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }
    }

}
