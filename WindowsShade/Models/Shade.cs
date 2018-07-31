using System;
using System.Runtime.InteropServices;

namespace WindowsShade.Models
{
    /// <summary>
    /// 参考：https://blog.csdn.net/dobzhansky/article/details/4464224
    /// </summary>
    public class Shade
    {
        // 窗口过程  
        static int SplashWindowProcedure(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
            case WM_DESTROY:
                PostQuitMessage(0);
                break;
            }
            return DefWindowProc(hwnd, msg, wParam, lParam);
        }
        [Flags]
        public enum EX_STYLE : long
        {
            WS_EX_TOPMOST = 0x00000008L,
            WS_EX_TOOLWINDOW = 0x00000080L
        }
        [Flags]
        public enum WS_STYLE : long
        {
            WS_OVERLAPPED = 0x00000000L,
            WS_CAPTION = 0x00C00000L,    /* WS_BORDER | WS_DLGFRAME  */
            WS_SYSMENU = 0x00080000L,
            WS_THICKFRAME = 0x00040000L,

            WS_MINIMIZEBOX = 0x00020000L,
            WS_MAXIMIZEBOX = 0x00010000L,

            WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED |
                             WS_CAPTION |
                             WS_SYSMENU |
                             WS_THICKFRAME |
                             WS_MINIMIZEBOX |
                             WS_MAXIMIZEBOX),
        }
        public const int WM_DESTROY = 2;
        public const int WM_CLOSE = 0x10;
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string moduleName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterClass(WNDCLASS wc);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, string lpszClassName, string lpszWindowName, int style, int x, int y, int width, int height, IntPtr hWndParent, IntPtr hMenu, IntPtr hInst, [MarshalAs(UnmanagedType.AsAny)] object pvParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void PostQuitMessage(int nExitCode);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(IntPtr Handle, int crKey, byte bAlpha, int dwFlags);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class WNDCLASS
        {
            public int style = 0;
            public WNDPROC lpfnWndProc = null;
            public int cbClsExtra = 0;
            public int cbWndExtra = 0;
            public IntPtr hInstance = IntPtr.Zero;
            public IntPtr hIcon = IntPtr.Zero;
            public IntPtr hCursor = IntPtr.Zero;
            public IntPtr hbrBackground = IntPtr.Zero;
            public string lpszMenuName = null;
            public string lpszClassName = null;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public int time;
            public int pt_x;
            public int pt_y;
        }
        public delegate int WNDPROC(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x20;
        const int WS_EX_LAYERED = 0x80000;
        const int LWA_ALPHA = 2;

        const int HWND_TOP = 0;
        const int HWND_BOTTOM = 1;
        const int HWND_TOPMOST = -1;
        const int HWND_NOTOPMOST = -2;

        private static IntPtr hwnd;
        public Shade()
        {
            if (hwnd != default(IntPtr))
                return;

            var wc = new WNDCLASS()
            {
                style = 0,
                lpfnWndProc = SplashWindowProcedure,
                hInstance = GetModuleHandle(null),
                hbrBackground = (IntPtr)1,
                lpszClassName = "CSharpWindow",
                cbClsExtra = 0,
                cbWndExtra = 0,
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                lpszMenuName = null,
            };
            RegisterClass(wc); // 注册窗口类  

            // 创建并显示窗口  
            hwnd = CreateWindowEx((int)EX_STYLE.WS_EX_TOPMOST | (int)EX_STYLE.WS_EX_TOOLWINDOW, "CSharpWindow", "Shade", (int)WS_STYLE.WS_OVERLAPPEDWINDOW, -1, -1, 400, 300, IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), null);
        }

        public void Show(bool display = true)
        {
            ShowWindow(hwnd, display ? 1 : 0);
        }

        public void Brightness(byte alpha)
        {
            SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            SetLayeredWindowAttributes(hwnd, 0, alpha, LWA_ALPHA);
        }
    }
}
