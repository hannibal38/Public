using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;







namespace FileWatcher {
    class Program {
        readonly ClassLibrary.CommonLibrary lib = new ClassLibrary.CommonLibrary();


       
        /* src경로에 폴더/파일의 변화가 있으면
         * 특정 명령어를 실행
         * SearchPattern은 정규식으로
         */
        

        public static class Global {
            public static string srcDirectory = LoadINI("src");
            public static string dstDirectory = LoadINI("dst");
            public static string FileName1 = LoadINI("FileName1");
            public static string FileName2 = LoadINI("FileName2");
            public static string FileName3 = LoadINI("FileName3");
            public static string SearchPattern = LoadINI("SearchPattern");
            public static string SearchPatternRegex = LoadINI("SearchPatternRegex");
            
            
        }
        



        static void Main(string[] args) {
           
            

            Console.ReadKey();



            string pattern = "700.2022.0[1-2]{2}";
            DirectoryInfo di=new DirectoryInfo(@"D:\HDD-C\Desktop\Desktop\title\MU\ShopScript");
            ArrayList al = new ArrayList();

            
            foreach (var d in di.GetDirectories()) {
                al.Add(d.ToString());
            }
            
                                 

            Regex regex=new Regex(pattern);

            foreach (string d in al)
                if (regex.IsMatch(d))
                    Console.WriteLine(d);




            Console.WriteLine("END");
            Console.ReadLine();


            //             700.2022.021

            




            // 이상 정규식테스트

            WriteLog(1, "INIT");
            //string result = GetLast(Global.srcDirectory);

            DoWatch(Global.srcDirectory);
                      
        }


        static string GetLast(string srcDirectory) {

            if (Directory.Exists(srcDirectory) == true) {
                string[] dirList = Directory.GetDirectories(srcDirectory);
                string ReturnStr = dirList[dirList.Length - 1];
                ReturnStr = ReturnStr.Replace(srcDirectory, "");
                ReturnStr = ReturnStr.Replace("\\", "");


                if (Global.SearchPattern != null){
                    foreach(string dirname in dirList) {

                    }

                }

                DirectoryInfo di = new DirectoryInfo(srcDirectory);
              


                if (Global.SearchPattern == null) {
                    
                }else {
                    
                }

                return dirList[0];
            }
            else {
                return null;
            }
        }

            

        static void DoWatch(string srcDirectory) {
            //Console.WriteLine(CopyToTarget(@"C:\C9Patches", @"C:\dev\temp")); ;

            
            if (Directory.Exists(srcDirectory) == true) {
                System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher();
                watcher.Path = srcDirectory;
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;

                watcher.Created += OnCreated;
                watcher.Deleted += OnDeleted;
                watcher.Renamed += OnRenamed;
                watcher.EnableRaisingEvents = true;

                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();

            }
            else {
                Console.WriteLine("targetdir:" + srcDirectory + " is not exist.");
            }
        }

        private static void OnCreated(object sender, FileSystemEventArgs e) {
            WriteLog(1, "Created: " + e.FullPath);
            CheckSourceFolder(e.Name);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) {
            WriteLog(1, "Deleted: " + e.FullPath);
        }

        private static void OnRenamed(object sender, RenamedEventArgs e) {
            WriteLog(1,"Renamed: " + e.OldName + " => " + e.Name);
            CheckSourceFolder(e.Name);
        }

        static int CheckSourceFolder(string foldername) {
            if (foldername.Contains(Global.SearchPattern)) {
                for(int i = 0; i <= 30; i++) {
                    string[] files = Directory.GetFiles(Global.srcDirectory + "\\" + foldername);
                    WriteLog(1, "Copy " + foldername + " to " + Global.dstDirectory);
                    int result = 0;
                    try {
                        result = CopyToTarget(Global.srcDirectory + "\\" + foldername, Global.dstDirectory + "\\" + foldername);
                        WriteLog(1, "CopyToTarget Result: " + result);
                    }
                    catch(Exception e){
                        WriteLog(1, "Ex in CheckSourceFolder(" + foldername + ")");
                        WriteLog(1, e.Message);
                        return result;
                    }
                    return result;
                }
                Console.WriteLine(foldername + " is not contain expected files.");
                return 0;
            }
            else {
                Console.WriteLine(foldername + " is not expected folder.");
                return 0;
            }
        }

        static int CopyToTarget(string src, string dst) {
            WriteLog(0, "Execute: " + "robocopy /e " + src + " " + dst);
            return ExecuteShellReturnExitCode("robocopy /e " + src + " " + dst);
        }

        /// <summary>
        /// (대리)쉘을 실행하고 ExitCode를 리턴
        /// </summary>
        /// <param name="Args">실행명령어</param
        /// <returns>int ExitCode리턴</returns>
         static int ExecuteShellReturnExitCode(string Args) {

            //7zip -> exit code
            //psexec -> exit code
            //service start ->  exit code
            //sc query [svc-name] -> result strings

            int ExitCode;
            string ReturnArray;

            ExecuteShellRoutine(Args, out ExitCode, out ReturnArray);

            return ExitCode;


        }

        static string LoadINI(string Key, string FilePath = @"\settings.ini") {


            FilePath = Directory.GetCurrentDirectory().ToString() + FilePath;

            
            try {
                using (StreamReader sr = new StreamReader(FilePath)) {
                    string line;
                    while ((line = sr.ReadLine()) != null) {

                        if (line.Contains(Key)) {
                            string[] ReturnString = line.Split(new char[] { '=' });
                            WriteLog(0, "LoadINI key:" + Key + " => " + ReturnString[1]); 
                            return ReturnString[1];
                        }
                    }
                }
            }
            catch (Exception e) {
                WriteLog(2, "Loadini threw EX!!! args: " + Key + ", Path:" + FilePath);
                WriteLog(0, e.Message);
            }

            WriteLog(2, "Loadini arg Is Not determine! : " + Key + ", Path:" + FilePath);
            return null;
        }

        /// <summary>
        /// 실제로 명령어를 받아 실행
        /// </summary>
        /// <param name="args">실행 명령어</param>
        /// <param name="ExitCode">리턴int ExitCode</param>
        /// <param name="ResultArray">리턴str ResultArray</param>
        static void ExecuteShellRoutine
            (string Args, out int ExitCode, out string ResultArray) {

            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = @"/c " + Args;

            try {
                p.Start();
            }
            catch {
                Console.WriteLine("Exception threw!!!");
                Console.WriteLine("args: " + Args);
                ExitCode = -1;
            }
            ResultArray = p.StandardOutput.ReadToEnd();
            ExitCode = p.ExitCode;

            p.WaitForExit();
            p.Close();

        }

        /// <summary>
        /// (대리)쉘을 실행하고 StringArray를 리턴
        /// </summary>
        /// <param name="Args">실행 명령어</param>
        /// <returns>string[] 실행결과를 리턴</returns>
        static string[] ExecuteShellReturnStringArray(string Args) {

            int ExitCode;
            string ReturnArray;

            ExecuteShellRoutine(Args, out ExitCode, out ReturnArray);

            string[] ExecuteResult = ReturnArray.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return ExecuteResult;
        }

        static void WriteLog(int level, string message) {
            string LogFilePath = Directory.GetCurrentDirectory().ToString();
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

    }
}