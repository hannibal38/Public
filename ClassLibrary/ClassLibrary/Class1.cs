using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ClassLibrary {

    //string variable for Log output path
    /// <summary>
    /// set commonlib's var
    /// </summary>
    public struct CommonSetting {
        /// <summary>
        /// set log filename
        /// </summary>
        public static string LogFilePath;
        /// <summary>
        /// DebugFlag. default:false
        /// </summary>
        public static Boolean DebugFlag = false;
    }


    /// <summary>
    /// 
    /// </summary>
    public class CommonLibrary {


        /// <summary>
        /// Get value from ini by Key. default filepath=settings.ini
        /// 20220426 Added: Skip comment line
        /// </summary>
        /// <param name="Key">Search Key</param>
        /// <param name="FilePath">Location of settingfile @"\settings.ini"</param>
        /// <returns>key-value or null</returns>
        public string LoadINI(string Key, string FilePath = @"\settings.ini") {


            FilePath = Directory.GetCurrentDirectory() + FilePath;

            try {
                using (StreamReader sr = new StreamReader(FilePath)) {
                    string line;
                    while ((line = sr.ReadLine()) != null) {
                        if (line.Substring(0, 2) == "//") {

                        }
                        else {
                            if (line.ToLower().Contains(Key.ToLower())) {
                                string[] ReturnString = line.Split(new char[] { '=' });
                                WriteLog(0, "LoadINI(" + Key + ") => " + ReturnString[1]);
                                return ReturnString[1];
                            }
                        }
                    }
                }
            }
            catch (Exception e) {
                WriteLog(2, "Loadini threw EX!!! args: " + Key + "\t(Filename:" + FilePath + ")");
                WriteLog(0, e.Message);
            }
            // not determin don't console output
            //WriteLog(2, "Loadini arg Is Not determine! : " + Key + "\t(Filename:" + FilePath + ")");
            return null;
        }

        /// <summary>
        /// byte to KB,MB,GB
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public string CastToGiga(long args) {

            decimal Lower = args / 1024;

            if (Lower < 1024) {
                return Math.Round(Lower, 2, MidpointRounding.AwayFromZero) + "KB";
            }

            Lower /= 1024;

            if (Lower < 1024) {
                return Math.Round(Lower, 2, MidpointRounding.AwayFromZero) + "MB";
            }

            Lower /= 1024;

            return Math.Round(Lower, 2, MidpointRounding.AwayFromZero) + "GB";

        }



        /// <summary>
        /// write log to file
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void WriteLog(int level, string message) {
            string LogFilePath = CommonSetting.LogFilePath;
            string LogLevel = "";

            switch (level) {
                case 0:
                    LogLevel = "Info";
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case 1:
                    LogLevel = "Action";
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case 2:
                    LogLevel = "Error";
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
            }

            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "\t" + LogLevel + "\t" + message);

            if (Directory.Exists(LogFilePath) == false) {
                Directory.CreateDirectory(LogFilePath);
            }
            //Append로 파일을 열면 필요없음
            //if(File.Exists(LogFilePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log") == false) {
            //    using (FileStream fs = File.Create(LogFilePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log")) {
            //        Byte[] info = new UTF8Encoding(true).GetBytes(DateTime.Now.ToString("HH:mm:ss") + "--- New date Log line---");

            //        // Add some information to the file
            //        fs.Write(info, 0, info.Length);
            //    }
            //}

            try {
                using (FileStream fs = File.Open(LogFilePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite)) {
                    byte[] b = new UTF8Encoding(true).GetBytes(DateTime.Now.ToString("HH:mm:ss") + "\t" + LogLevel + "\t" + message + "\r\n");
                    UTF8Encoding temp = new UTF8Encoding(true);

                    fs.Write(b, 0, b.Length);
                }
            }
            catch (Exception e) {
                Console.WriteLine("Writelog Error!!!");
                Console.WriteLine("LogPath: " + LogFilePath);
                Console.WriteLine(e.Message);
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }



        



        /// <summary>
        /// terminate application
        /// </summary>
        /// <param name="msg"></param>
        public void Terminate(string msg) {
            WriteLog(1, "Terminate: " + msg);
            Environment.Exit(0);
        }

        /// <summary>
        /// Terminate application with exitcode
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exitcode"></param>
        public void Terminate(string msg, int exitcode) {
            WriteLog(1, "Terminate: " + msg);
            Environment.Exit(exitcode);
        }

        /// <summary>
        /// Terminate application with ArrayList
        /// </summary>
        /// <param name="al">Msg Array</param>
        public void Terminate(ArrayList al) {
            WriteLog(1, "Terminate with ArrayList");
            int lines = 0;
            foreach (var d in al)
                WriteLog(0, ++lines +  ": " + d.ToString());
            Environment.Exit(0);
        }


        /// <summary>
        /// (대리)쉘을 실행하고 ExitCode를 리턴
        /// </summary>
        /// <param name="Args">실행명령어</param>
        /// <returns>int ExitCode리턴</returns>
        public int ExecuteShellReturnExitCode(string Args) {

            //7zip -> exit code
            //psexec -> exit code
            //service start ->  exit code
            //sc query [svc-name] -> result strings

            int ExitCode;
            string ReturnArray;

            ExecuteShellRoutine(Args, out ExitCode, out ReturnArray);

            return ExitCode;

            //string[] ExecuteResult = ReturnArray.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //* Code Meaning 
            //*0 No error 
            //*1 Warning (Non fatal error(s)). For example, one or more files were locked by some other application, so they were not compressed. 
            //*2 Fatal error 
            //*7 Command line error 
            //*8 Not enough memory for operation 
            //*255 User stopped the process 
            //*/

            //string ReturningResult = "";

            //for(int i = 0; i <= results.Length; i++) {
            //    if (results.Contains("STATE")){
            //        ReturningResult = results;
            //        }
            //}

            ////Console.WriteLine("Result: " + results);
            //Console.WriteLine("Result: " + ReturningResult);

        }



        /// <summary>
        /// 실제로 명령어를 받아 실행
        /// </summary>
        /// <param name="Args">실행 명령어</param>
        /// <param name="ExitCode">리턴int ExitCode</param>
        /// <param name="ResultArray">리턴str ResultArray</param>
        public void ExecuteShellRoutine
            (string Args, out int ExitCode, out string ResultArray) {
            Boolean DebugFlag = CommonSetting.DebugFlag;
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = @"/c " + Args;
            if (DebugFlag == true)
                WriteLog(0, "Execute args: " + p.StartInfo.Arguments);
            try {
                p.Start();
            }
            catch {
                Console.WriteLine("Exception threw!!!");
                Console.WriteLine("args: " + Args);
                ExitCode = -1;
            }
            ResultArray = p.StandardOutput.ReadToEnd();
            if (DebugFlag == true)
                WriteLog(0, "Output: " + ResultArray);

            ExitCode = p.ExitCode;

            p.WaitForExit();
            p.Close();

        }

        /// <summary>
        /// (대리)쉘을 실행하고 StringArray를 리턴
        /// </summary>
        /// <param name="Args">실행 명령어</param>
        /// <returns>string[] 실행결과를 리턴</returns>
        public string[] ExecuteShellReturnStringArray(string Args) {

            int ExitCode;
            string ReturnArray;

            ExecuteShellRoutine(Args, out ExitCode, out ReturnArray);

            string[] ExecuteResult = ReturnArray.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return ExecuteResult;
        }


        
        //string UploadedPath = @"C:\Users\sehychoi\Downloads\7z2107-x64.exe";
        /// <summary>
        /// set path
        /// </summary>
        /// <param name="UploadedPath">new version path</param>
        /// <returns>0 no update, 1 updated, -1 error</returns>
        public int setPath(string UploadedPath) {
            if (!File.Exists(UploadedPath)) {
                Console.WriteLine("File is not exist: " + UploadedPath);
                return -1;
            }
            Console.WriteLine(File.GetLastAccessTime(UploadedPath));
            return 0;
        }



        /// <summary>
        /// Check update available
        /// </summary>
        /// <param name="AppName">Curruntly executing AppName for create batch</param>
        /// <param name="NewVerPath">New version path</param>
        /// <returns></returns>
        public Boolean CheckUpdate(string AppName, string NewVerPath) {   
            if (File.Exists(NewVerPath)) {
                FileInfo fi1;
                FileInfo fi2;

                //currently executing file's date
                try {
                    fi1 = new FileInfo(Directory.GetCurrentDirectory() +"\\" + AppName + ".exe");
                } catch (Exception e){
                    WriteLog(2, "Cannot get FileInfo: " + Directory.GetCurrentDirectory() + "\\" + AppName + ".exe");
                    WriteLog(0, e.Message);
                    return false;
                }
                //new version's date
                try {
                    fi2 = new FileInfo(NewVerPath);
                } catch (Exception e) {
                    WriteLog(2, "Cannot get FileInfo: " + Directory.GetCurrentDirectory() + "\\" + AppName + ".exe");
                    WriteLog(0, e.Message);
                    return false;
                }
                if (fi1.LastWriteTime != fi2.LastWriteTime) {
                    DoUpdate(AppName, NewVerPath);
                }
            }
            else {
                WriteLog(2, "Newversion path is not exist: " + NewVerPath);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Create batch and execute update
        /// </summary>
        /// <param name="AppName">AppName</param>
        /// <param name="NewVerPath">NewVersionPath</param>
        public void DoUpdate(string AppName,string NewVerPath) {
            string BatchPath = Directory.GetCurrentDirectory() + "\\" + "Update.bat";
            if (File.Exists(BatchPath))
                File.Delete(BatchPath);
                       
            ArrayList Batch = new ArrayList();
            Batch.Add("ping 8.8.8.8 2>nul");
            Batch.Add("taskkill /f /im " + AppName);
            Batch.Add("echo date time >> update.log");
            Batch.Add("copy /y " + NewVerPath);
            Batch.Add("ping 8.8.8.8 2>nul");
            Batch.Add("start " + Directory.GetCurrentDirectory() + "\\" + AppName + ".exe");

            StreamWriter sw = new StreamWriter(BatchPath);
            {
                foreach(string line in Batch) {
                    sw.WriteLine(line + "\n");
                }
            }
            sw.Close();

            ExecuteShellReturnExitCode(BatchPath);
        }
      
    }
}
