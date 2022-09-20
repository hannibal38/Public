using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteExec
{
    class Program
    {
    
        static ClassLibrary.CommonLibrary lib = new ClassLibrary.CommonLibrary();


        static void Main(string[] args) {

            ClassLibrary.CommonSetting.LogFilePath = string.Concat(Directory.GetCurrentDirectory(), "\\Log");
            ClassLibrary.CommonSetting.DebugFlag = true;

            //string PsExecPath = lib.LoadINI("PsExecPath", @"\RemoteExec.ini");
            //if (PsExecPath == null)
            //    lib.Terminate("No argument(PsExec)!!!");
            //if (!File.Exists(PsExecPath))
            //    lib.Terminate("PsExec is not exist!!!");

            string UpdatePath = lib.LoadINI("UpdatePath", @"\RemoteExec.ini");
            if (UpdatePath != null)
                if (lib.CheckUpdate("RemoteExec", UpdatePath))
                    lib.DoUpdate("RemoteExec", UpdatePath);


            //string[] Result = lib.ExecuteShellReturnStringArray(PsExecPath + " \\ - nobanner cmd / c query session | findstr Active");

            //Console.WriteLine("Result Len: " + Result.Length);
            //foreach (string str in Result)
            //    Console.WriteLine(str);


        }
        

        static ArrayList SplitSessionResult(string Result) {
            ArrayList AL = new ArrayList();

            return AL;
            
        }







    }
}
