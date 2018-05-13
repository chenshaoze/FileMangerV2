using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FileMangerV2
{
    public class SystemInfoIcon
    {

        [DllImport("Shell32.dll")]
        private static extern IntPtr SHGetFileInfo
        (
            string pszPath,
            uint dwFileAttributes,
            out SHFILEINFO psfi,
            uint cbfileInfo,
            SHGFI uFlags
        );


        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero; iIcon = 0; dwAttributes = 0; szDisplayName = ""; szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            public string szTypeName;
        };

        private enum SHGFI
        {
            SmallIcon = 0x00000001,
            LargeIcon = 0x00000000,
            Icon = 0x00000100,
            DisplayName = 0x00000200,
            Typename = 0x00000400,
            SysIconIndex = 0x00004000,
            UseFileAttributes = 0x00000010
        }

        /// <summary>  
        /// 根据文件扩展名得到系统扩展名的图标  
        /// </summary>  
        /// <param name="fileName">文件名(如：win.rar;setup.exe;temp.txt)</param>  
        /// <param name="largeIcon">图标的大小</param>  
        /// <returns></returns>  
        public static Icon GetFileIcon(string fileName, bool largeIcon)
        {
            SHFILEINFO info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;
            if (largeIcon)
                flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;
            else
                flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
            IntPtr IconIntPtr = SHGetFileInfo(fileName, 256, out info, (uint)cbFileInfo, flags);
            if (IconIntPtr.Equals(IntPtr.Zero))
                return null;
            return Icon.FromHandle(info.hIcon);
        }

        /// <summary>    
        /// 获取文件夹图标  
        /// </summary>    
        /// <returns>图标</returns>    
        public static Icon GetDirectoryIcon(string dirName, bool largeIcon)
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();
            int cbFileInfo = Marshal.SizeOf(_SHFILEINFO);
            SHGFI flags;
            if (largeIcon)
                flags = SHGFI.Icon | SHGFI.LargeIcon;
            else
                flags = SHGFI.Icon | SHGFI.SmallIcon;

            IntPtr IconIntPtr = SHGetFileInfo(dirName, 0, out _SHFILEINFO, (uint)cbFileInfo, flags);
            if (IconIntPtr.Equals(IntPtr.Zero))
                return null;
            Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);
            return _Icon;
        }
    }
}
