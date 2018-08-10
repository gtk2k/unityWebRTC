using System;
using System.Runtime.InteropServices;

class Win32Api
{
    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(SystemMetric smIndex);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteDC([In] IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, DIB_Color_Mode pila, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

    [DllImport("gdi32.dll")]
    public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject([In] IntPtr hObject);

    [DllImport("gdi32.dll")]
    public static extern int BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, TernaryRasterOperations dwRop);

    [DllImport("gdi32.dll")]
    public static extern bool StretchBlt(IntPtr hDCDest, int dstX, int dstY, int dstW, int dstH, IntPtr hDCSrc, int srcX, int srcY, int srcW, int srcH, TernaryRasterOperations dwRop);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public int Width
        {
            get
            {
                return Right - Left;
            }
        }
        public int Height
        {
            get
            {
                return Bottom - Top;
            }
        }
    }

    public enum SystemMetric
    {
        SM_CXSCREEN = 0,
        SM_CYSCREEN = 1
    }

    public enum DIB_Color_Mode : uint
    {
        DIB_RGB_COLORS = 0,
        DIB_PAL_COLORS = 1
    }

    public enum TernaryRasterOperations : uint
    {
        SRCCOPY = 0x00CC0020,
        CAPTUREBLT = 0x40000000 //only if WinVer >= 5.0.0 (see wingdi.h)
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BITMAPFILEHEADER
    {
        public ushort bfType;
        public uint bfSize;
        public ushort bfReserved1;
        public ushort bfReserved2;
        public uint bfOffBits;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        public RGBQUAD bmiColors;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
        public const int BI_RGB = 0;

        public void Init()
        {
            biSize = (uint)Marshal.SizeOf(this);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
    }

    static BITMAPINFO bmpInfo;
    static IntPtr desktopWnd;
    static IntPtr desktopDC;
    static IntPtr cmpDC;
    static IntPtr cmpBmp;
    static IntPtr oldBmp;
    static bool inited;
    public static IntPtr pPixelData;

    public static int ScreenWidth;
    public static int ScreenHeight;

    public static void BeginDesktopCapture(int width, int height)
    {
        ScreenWidth = GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        ScreenHeight = GetSystemMetrics(SystemMetric.SM_CYSCREEN);

        bmpInfo = new BITMAPINFO();
        bmpInfo.bmiHeader = new BITMAPINFOHEADER();
        bmpInfo.bmiHeader.Init();
        bmpInfo.bmiHeader.biBitCount = 32;
        bmpInfo.bmiHeader.biPlanes = 1;
        bmpInfo.bmiHeader.biWidth = width;
        bmpInfo.bmiHeader.biHeight = height;

        desktopWnd = GetDesktopWindow();
        desktopDC = GetDC(desktopWnd);
        cmpDC = CreateCompatibleDC(desktopDC);
        cmpBmp = CreateDIBSection(cmpDC, ref bmpInfo, DIB_Color_Mode.DIB_RGB_COLORS, out pPixelData, IntPtr.Zero, 0);
        oldBmp = SelectObject(cmpDC, cmpBmp);

        inited = true;
    }

    public static void DesktopCapture()
    {
        BitBlt(cmpDC, 0, 0, bmpInfo.bmiHeader.biWidth, bmpInfo.bmiHeader.biHeight, desktopDC, 0, 0, TernaryRasterOperations.SRCCOPY);
    }

    public static void StretchDesktopCapture()
    {
        StretchBlt(cmpDC, 0, 0, bmpInfo.bmiHeader.biWidth, bmpInfo.bmiHeader.biHeight, desktopDC, 0, 0, ScreenWidth, ScreenHeight, TernaryRasterOperations.SRCCOPY);
    }

    public static void EndDesktopCapture()
    {
        if (!inited) return;
        SelectObject(cmpDC, oldBmp);
        DeleteObject(cmpBmp);
        DeleteDC(cmpDC);
        ReleaseDC(desktopWnd, desktopDC);
        inited = false;
    }
}