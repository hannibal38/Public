using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;


namespace FindHandle {

    class Program {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        public static string GetWindowTitle(IntPtr hWnd) {
            var length = GetWindowTextLength(hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

        //ウィンドウを探す用のメソッド
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        ////送信するためのメソッド(数値)
        //[DllImport("User32.dll", EntryPoint = "SendMessage")]
        //public static extern Int32 SendMessage(Int32 hWnd, Int32 Msg, Int32 wParam,  string lParam);

        ////送信するためのメソッド(文字も可能)
        //[DllImport("User32.dll", EntryPoint = "SendMessage")]
        //public static extern Int32 SendMessage(Int32 hWnd, Int32 Msg, Int32 wParam, Int32 lParam);

        //送信するためのメソッド(数値)
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern Int32 PostMessage(Int32 hWnd, Int32 Msg, Int32 wParam, string lParam);

        //送信するためのメソッド(文字も可能)
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern Int32 PostMessage(Int32 hWnd, Int32 Msg, Int32 wParam, Int32 lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wparam, string lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetDlgCtrlID(IntPtr hwndCtl);
        const int WM_GETTEXT = 0x000D;
        const int WM_GETTEXTLENGTH = 0x000E;


        static void Main(string[] args) {

            Process p = Process.GetProcessesByName("FileWatcher")[0];

            Console.WriteLine("pid: " + p.Id);
            IntPtr MainHandle = p.MainWindowHandle;
            Console.WriteLine("Main handle: " + string.Format("0x{0:8X}",MainHandle));
            IntPtr SubHandle = p.Handle;
            Console.WriteLine("Sub handle: " + string.Format("0x{0:8X}", SubHandle));

            int EditHandle = GetDlgCtrlID(MainHandle);
            
            Console.WriteLine("Edit handle:" + EditHandle.ToString("X8"));


            IntPtr hwnd=FindWindow(null,p.MainWindowTitle);
            Console.WriteLine("Console handle:" + hwnd.ToString("X8"));

            // Get the size of the string required to hold the window title (including trailing null.) 
            Int32 titleSize = SendMessage(hwnd, WM_GETTEXTLENGTH, 0, "0");

            // If titleSize is 0, there is no title so return an empty string (or null)
                            

            StringBuilder title = new StringBuilder(titleSize + 1);

            SendMessage(hwnd, (int)WM_GETTEXT, title.Capacity, title.ToString());

            Console.WriteLine("Console caption: " + title);

            Console.WriteLine("FIN");
            Console.ReadLine();

        }
    }
}
