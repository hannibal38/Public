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

            Console.SetWindowSize(120, 40);


            //0 ip
            //1 user


            string Account = "";
            string pwd = "";
            string IP="";
            string cmd = "";

            #region inputargs check
            if (args.Length >= 1) {
                try {
                    Account = args[0];
                    pwd = args[1];
                    IP = args[2];
                    cmd = args[3];
                }
                catch {

                }
            }
            else {
                Console.WriteLine("Input arguments");
                Console.WriteLine("[0] AD account with domain");
                Console.WriteLine("[1] password");
                Console.WriteLine("[2] IP address");
                Console.WriteLine("[3] command");
                Environment.Exit(-1);
            }
            #endregion


            //check update
            string UpdatePath = lib.LoadINI("UpdatePath", @"\RemoteExec.ini");
            if (UpdatePath != null)
                if (lib.CheckUpdate("RemoteExec", UpdatePath))
                    lib.DoUpdate("RemoteExec", UpdatePath);


            string PsExecPath = lib.LoadINI("PsExecPath", @"\RemoteExec.ini");
            //get session id
            int sessionNo = GetRdpNo(IP, Account, pwd,PsExecPath);
            if (sessionNo == -1) {
                lib.Terminate("Get rdp session id has failed!");
            }
            else {
                lib.WriteLog(1, "RDP session ID : " + sessionNo);
            }

            string BasicExec = PsExecPath + " \\\\" + IP + " -u " +Account + " -p " + pwd + " -i " +sessionNo 
                + " -nobanner cmd /c start " ;

            string[] ExecuteResult= lib.ExecuteShellReturnStringArray(BasicExec + cmd);

            foreach (string str in ExecuteResult)
                Console.WriteLine(str);
            

        }


        static int GetRdpNo(string IP,string Account, string pwd,string PsExecPath) {
            int sessionNo;
            
            
            string[] Result = lib.ExecuteShellReturnStringArray(PsExecPath + " \\\\" + IP + " -u " +
                Account + " -p " + pwd + " -nobanner query session");

            lib.WriteLog(0,"Query session Result Len: " + Result.Length);

            if(Result.Length != 0) {

                for(int i = 0; i <= Result.Length - 1; i++) {
                    if (Result[i].Contains("Active")) {
                        sessionNo=SplitSessionResult(Result[i]);
                        return sessionNo;
                    }
                }
                return -1;
            }     
            else {
                lib.Terminate("Get rdp session id result is null!");
                return -1;
            }
        }


        static int SplitSessionResult(string Result) {
            string[] Line;
            Line = Result.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int sessionno = 0;

            try {
                sessionno=int.Parse(Line[2]);
                return sessionno;
            } catch {
                return -1;
            }
        }







    }
}
