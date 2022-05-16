using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; //dll import
using System.Diagnostics;



namespace FindHandle
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern int FindWindow(string ipClassName, string IpWindowName);
        [DllImport("user32")]
        public static extern int GetWindow(int hwnd, int wCmd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32")]
        public static extern int GetClassName(int hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll",SetLastError =true, CharSet = CharSet.Unicode)]
        public static extern Int32 SendMessage(IntPtr hWnd, Int32 uMsg, int WParam, IntPtr LParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SendMessageA(int hWnd, int wMsg, int wParam, string lParam);
        
         [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern int GetWindowTextLength(IntPtr hWnd);


        static void Main(string[] args) {

            Process[] processes = Process.GetProcessesByName("MakeUpdateFile");

            IntPtr windowHandle=new IntPtr();

            foreach (Process p in processes) {
                windowHandle = p.MainWindowHandle;
                
                Console.WriteLine(string.Format("#windowHandle: {0:X8}", windowHandle.ToInt64()));
                // do something with windowHandle
            
            }

            int WM_SETTEXT = 0xC;
            int WM_GETTEXTLENGTH = 0xE;
            int WM_GETTEXT = 0x000D;
            int GW_HWNDNEXT = 2;

            //"MakeUpdateFile 1.2.4.0"

            IntPtr hw = new IntPtr();
            hw=GetWindow(windowHandle, 5);
            Console.WriteLine(string.Format("hw1: {0:X8}", hw.ToInt64()));
                       
            hw = GetWindow(hw, 2);
            Console.WriteLine(string.Format("hw2: {0:X8}", hw.ToInt64()));

            hw = GetWindow(hw, 2);
            var length = GetWindowTextLength(hw);
            Console.WriteLine("length: " + length);
            var title = new StringBuilder(length+1);
            Console.WriteLine("title: " + title);
            Console.WriteLine(string.Format("hw3: {0:X8}", hw.ToInt64()));
            Int32 textLength=SendMessage(hw,WM_GETTEXTLENGTH,0,IntPtr.Zero);
            IntPtr textPtr = Marshal.AllocHGlobal(textLength);
            Console.WriteLine(string.Format("textptr: {0:X8}", textPtr.ToInt64()));
            SendMessage(hw, WM_GETTEXT, 260, textPtr);
            Console.Write(string.Format("hw4: {0:X8}", hw.ToInt64()));
            Console.WriteLine(" caption: " + Marshal.PtrToStringAuto(textPtr, textLength));
            
            GetWindowText(hw, title, 255);


            //Console.WriteLine(string.Format("hw1: {0:X4}", hw1));
            //GW_HWNDFIRST	 0	 최상위 Window를 찾는다.
            //GW_HWNDLAST	 1	 최하위 Window를 찾는다.
            //GW_HWNDNEXT	 2	 하위 Window를 찾는다.
            //GW_HWNDPREV	 3	 상위 Window를 찾는다.
            //GW_OWNER	 4	 부모 Window를 찾는다.
            //GW_CHILD	 5	 자식 Window를 찾는다.
            //int hw2 = GetWindow(hw1,5);
            //Console.WriteLine(string.Format("hw2: {0:X4}", hw2));

            //int hw3 = GetWindow(hw2, 2);
            //Console.WriteLine(string.Format("hw3: {0:X4}", hw3));

            //int hw4 = GetWindow(hw3, 2);
            //Console.WriteLine(string.Format("hw3: {0:X4}", hw4));


            //StringBuilder sClass = new StringBuilder(100);
            //GetClassName(hw4, sClass, 100);

            //Console.WriteLine(string.Format("hw4 classname: " + sClass.ToString()));


            //// 텍스트박스1 의 텍스트 길이를 가져온다.
            //Int32 textLength = SendMessage(new IntPtr(hw2), WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);

            //// 260 바이트 만큼 메모리 공간을 할당한다.
            //IntPtr textPtr = Marshal.AllocHGlobal(260);

            //// 텍스트박스1 의 텍스트를 textPtr 에 저장한다.
            //SendMessage(new IntPtr(hw2), WM_GETTEXT, new IntPtr(260), textPtr);
            //Console.WriteLine("txtPtr caption" + textPtr.ToString());
            //// 포인터를 유니코드 문자열로 변환한다. 
            //String text1 = Marshal.PtrToStringUni(textPtr, 260);

            //// 텍스트박스2 에 텍스트박스1 의 텍스트를 입력한다.
            ////SendMessage(textBox2.Handle, WM_SETTEXT, IntPtr.Zero, textPtr);

            //// 사용이 끝난 포인터는 메모리에서 해제해준다.
            //Marshal.FreeHGlobal(textPtr);


            ////SendMessage(hw4, WM_SETTEXT, IntPtr.Zero, "testSendmsg");

            //SendMessageA(hw4, WM_SETTEXT, 0, "testSendmsg");






            Console.WriteLine("FIN");
            Console.ReadLine();

        }




    }


}