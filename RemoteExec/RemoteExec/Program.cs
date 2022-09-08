﻿using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteExec
{
    class Program
    {
    /* D:\SysinternalsSuite_20181210>psexec \\172.21.150.190 -nobanner cmd /c query session | findstr Active
    Starting cmd on 172.21.150.190... 172.21.150.190...
     rdp-tcp#2         muoadmin_tg               3  Active
    cmd exited on 172.21.150.190 with error code 1.
    */

        static ClassLibrary.CommonLibrary lib = new ClassLibrary.CommonLibrary();


        static void Main(string[] args) {
            string PsExecPath = lib.LoadINI("PsExecPath", @"\RemoteExec.ini");
            if (PsExecPath == null)
                lib.Terminate("No argument(PsExec)!!!");
            if (!File.Exists(PsExecPath))
                lib.Terminate("PsExec is not exist!!!");

            string UpdatePath = lib.LoadINI("UpdatePath", @"\RemoteExec.ini");
            if (UpdatePath != null)
                if (lib.CheckUpdate("RemoteExec", UpdatePath))
                    lib.DoUpdate("RemoteExec", UpdatePath);









        }


        static ArrayList SplitSessionResult(string Result) {

            
        }
    }
}