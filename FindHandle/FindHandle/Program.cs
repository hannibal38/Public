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
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        //送信するためのメソッド(数値)
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern Int32 SendMessage(Int32 hWnd, Int32 Msg, Int32 wParam,  string lParam);

        //送信するためのメソッド(文字も可能)
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern Int32 SendMessage(Int32 hWnd, Int32 Msg, Int32 wParam, Int32 lParam);

        //送信するためのメソッド(数値)
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern Int32 PostMessage(Int32 hWnd, Int32 Msg, Int32 wParam, string lParam);

        //送信するためのメソッド(文字も可能)
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern Int32 PostMessage(Int32 hWnd, Int32 Msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetDlgCtrlID(IntPtr hwndCtl);


        static void Main(string[] args) {

            Process p = Process.GetProcessesByName("FileWatcher")[0];

            Console.WriteLine("pid: " + p.Id);
            IntPtr MainHandle = p.MainWindowHandle;
            Console.WriteLine("Main handle:" + MainHandle.ToString("X8"));

            int EditHandle = GetDlgCtrlID(MainHandle);
            
            Console.WriteLine("Edit handle:" + EditHandle.ToString("X8"));

            Console.WriteLine("FIN");
            Console.ReadLine();

        }
    }
}
