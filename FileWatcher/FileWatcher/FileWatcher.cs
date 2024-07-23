using System;
using System.Collections;
using System.Threading;
using System.Globalization;

//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;



namespace FileWatcher {
    class FileWatcher {

        static ClassLibrary.CommonLibrary lib = new ClassLibrary.CommonLibrary();
        

        public static class Global {
            public static string pattern = lib.LoadINI("SearchPattern", "\\FileWatcher.ini");
            public static string ExecuteCode = lib.LoadINI("ExecutePath", "\\FileWatcher.ini");
            public static string filePath = @"D:\temp\LostArk_Client_20230814.zip";
            public static string srcPath= lib.LoadINI("SourcePath", "\\FileWatcher.ini");
        }



        static void Main(string[] args) {
            ClassLibrary.CommonSetting.LogFilePath = string.Concat(Directory.GetCurrentDirectory(), "\\FileWatcher");

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            
            Check();


            ClassLibrary.CommonSetting.LogFilePath = string.Concat(Directory.GetCurrentDirectory(), "\\FileWatchLog");
            ClassLibrary.CommonSetting.DebugFlag = true;

            string INIPath = "\\FileWatcher.ini";


            //lib.CheckUpdate("FileWatcher");

            string Obj = lib.LoadINI("ObjectKind", INIPath);
            int ObjectKind = -1;

            try {
                ObjectKind = Int32.Parse(Obj);
            }
            catch (Exception e) {
                ArrayList al = new ArrayList();
                al.Add("DIR / File flag is not setted!!!");
                al.Add(e.Message);
                lib.Terminate(al);
            }

            string[] ListArray;
            ListArray = GetListReturnArray(ObjectKind, Global.srcPath);

            DoWatch(ObjectKind, Global.srcPath);

            Console.WriteLine("FIN");
            Console.ReadLine();
        }
        //The process cannot access the file 'D:\temp\dvd.iso' because it is being used by another process.

        static void Check() {
            

            do {

                try {
                    using (FileStream fs = new FileStream(Global.filePath, FileMode.Open, FileAccess.Read, FileShare.None)) {
                        FileInfo fi = new FileInfo(Global.filePath);
                        Console.Write(DateTime.Now + "\t");
                        Console.WriteLine("File is not locked. \t" + lib.CastToGiga(fi.Length));
                    }

                }
                catch (FileNotFoundException) {
                    Console.Write(DateTime.Now + "\t");
                    Console.WriteLine("NOT FOUND!");
                }
                catch (Exception e) {
                    //Message	"The process cannot access the file 'D:\\temp\\LostArk_Client_20230814.zip' because it is being used by another process."
                    if (e.Message.Contains("because it is being used by another process")){
                        FileInfo fi = new FileInfo(Global.filePath);
                        Console.Write(DateTime.Now + "\t");
                        Console.WriteLine("LOCKED! \t" + lib.CastToGiga(fi.Length));
                    } else {
                        Console.WriteLine("Exception!");
                        Console.WriteLine(e.Message);
                    }
                }
                System.Threading.Thread.Sleep(1000);
            } while (true);


            Console.WriteLine("FIN");
            Console.ReadLine();
        }


        /// <summary>
        /// Get current file/dir list
        /// </summary>
        /// <param name="kind">1 dir 2 file</param>
        /// <param name="path">path</param>
        static string[] GetListReturnArray(int kind, string path) {

            ArrayList al = new ArrayList();
            DirectoryInfo di = new DirectoryInfo(path);

            switch (kind) {
                case 1:
                    foreach (var d in di.GetDirectories())
                        al.Add(d.ToString());
                    break;

                case 2:
                    foreach (var d in di.GetFiles())
                        al.Add(d.ToString());
                    break;
            }
            string[] ReturlArray = new string[al.Count];
            for (int i = 0; i <= al.Count - 1; i++)
                ReturlArray[i] = al[i].ToString();
            return ReturlArray;
        }

        static void DoWatch(int kind, string path) {
            if (Directory.Exists(path) == false)
                lib.Terminate("TargetDir: " + path + " is not Exist!!!");

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;


            watcher.Created += OnCreated;
            watcher.Deleted += OnDelete;
            watcher.Renamed += OnRenamed;

            watcher.EnableRaisingEvents = true;

            Console.Title = "Waiting change: " + Global.srcPath;
            Console.ReadLine();
        }

        static void OnCreated(object sender, FileSystemEventArgs e) {
            lib.WriteLog(1, "Created: " + e.FullPath);
            CheckSourceFolder(e.Name, e.FullPath);
        }

        static void OnDelete(object sender, FileSystemEventArgs e) {
            lib.WriteLog(1, "Deleted: " + e.FullPath);
        }

        static void OnRenamed(object sender, RenamedEventArgs e) {
            lib.WriteLog(1, "Renamed: " + e.OldName + " => " + e.Name);
            CheckSourceFolder(e.Name, e.FullPath);

        }

        static void CheckSourceFolder(string NewName, string fullpath) {
            if (Regex.IsMatch(NewName, Global.pattern)) {
                lib.WriteLog(0, "Match! " + NewName);
                Console.WriteLine("Waiting no change 10sec...");

                if (WaitSize(1, fullpath) == true) {
                    //TODO: 실행할 때 파라메터 던지기
                    int result = lib.ExecuteShellReturnExitCode(Global.ExecuteCode + " " + NewName);
                    lib.WriteLog(0, "Executed result: " + result);
                } else {
                    lib.WriteLog(2, "CheckSourceFolder(" + NewName + "," + fullpath + ") ==false");
                    lib.Terminate("waitsize err!!");
                }
            } else {
                lib.WriteLog(0, "CheckSourceFolder(" + NewName + ") is not Matched.");
            }

        }


        /// <summary>
        /// wait size is not change anymore
        /// </summary>
        /// <param name="kind">1 dir 2 file</param>
        /// <param name="path"></param>
        /// <returns></returns>
        static bool WaitSize(int kind, string path) {
            int cnt = 0;
            long BeforeSize = 0;

            while (cnt <= 10) {

                switch (kind) {
                    case 1:

                        DirectoryInfo di = new DirectoryInfo(path);

                        long FolderSize;
                        try {
                            FolderSize = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);
                            if (FolderSize > BeforeSize) {
                                BeforeSize = FolderSize;
                                cnt = 0;
                            } else {
                                cnt++;
                            }
                        }
                        catch (Exception e) {
                            cnt++;
                            if (cnt <= 10) {

                                lib.WriteLog(2, "waitsize(" + kind + "," + path + ")\n" + e.HelpLink);
                                lib.WriteLog(2, "waitsize(" + kind + "," + path + ")\n" + e.Message);
                                return false;
                            }
                        }
                        break;

                    case 2:
                        do {
                            try {
                                using (FileStream fs = new FileStream(Global.filePath, FileMode.Open, FileAccess.Read, FileShare.None)) {
                                    Console.Write(DateTime.Now + "\t");
                                    Console.WriteLine("File is not locked.");
                                }
                            }
                            catch (FileNotFoundException) {
                                Console.Write(DateTime.Now + "\t");
                                Console.WriteLine("NOT FOUND!");
                            }
                            catch (Exception e) {
                                Console.Write(DateTime.Now + "\t");
                                Console.WriteLine("LOCKED!");
                                Console.WriteLine(e.Message);
                            }
                            System.Threading.Thread.Sleep(1000);
                        } while (true) ;
                        break;

            }
            System.Threading.Thread.Sleep(1000 * 10);
            lib.WriteLog(0, "cnt: " + cnt);
        }
            return true;
        }


    static long GetFileSize(string filePath) {
        if (File.Exists(filePath)) {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }


        return 0;
    }


}
}
