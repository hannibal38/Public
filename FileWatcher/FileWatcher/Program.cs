using System;
using System.Collections;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;



namespace FileWatcher {
    class Program {

        static ClassLibrary.CommonLibrary lib = new ClassLibrary.CommonLibrary();

        public static class Global{
            public static string pattern = lib.LoadINI("SearchPattern", "\\FileWatcher.ini");
            public static string ExecuteCode = lib.LoadINI("ExecutePath", "\\FileWatcher.ini");
        }



        static void Main(string[] args) {

            

            ClassLibrary.CommonSetting.LogFilePath = string.Concat(Directory.GetCurrentDirectory(),"\\FileWatchLog");
            ClassLibrary.CommonSetting.DebugFlag = true;

            string INIPath = "\\FileWatcher.ini";

           
            string srcPath = lib.LoadINI("SourcePath", INIPath);
            
            string Obj= lib.LoadINI("ObjectKind", INIPath);
            int ObjectKind=-1;

            try {
                ObjectKind= Int32.Parse(Obj);
            }
            catch (Exception e) {
                ArrayList al = new ArrayList();
                al.Add("DIR / File flag is not setted!!!");
                al.Add(e.Message);
                lib.Terminate(al);
            }                    

            string[] ListArray;
            ListArray = GetListReturnArray(ObjectKind, srcPath);

            DoWatch(ObjectKind, srcPath);

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

            switch (kind){
                case 1:
                    foreach (var d in di.GetDirectories())
                        al.Add(d.ToString());
                    break;

                case 2:
                    foreach (var d in di.GetFiles())
                        al.Add(d.ToString());
                    break;
            }
            string[] ReturlArray=new string[al.Count];
            for (int i = 0; i <= al.Count - 1; i++)
                ReturlArray[i] = al[i].ToString();
            return ReturlArray;
        }

        static void DoWatch(int kind,string path) {
            if (Directory.Exists(path)==false)
                lib.Terminate("TargetDir: " + path + " is not Exist!!!");

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;

            watcher.Created += OnCreated;
            watcher.Deleted += OnDelete;
            watcher.Renamed += OnRenamed;
           
            watcher.EnableRaisingEvents = true;

            Console.Title = "Waiting folder/files be changed";
            Console.ReadLine();
        }

        static void OnCreated(object sender,FileSystemEventArgs e) {
            lib.WriteLog(1, "Created: " + e.FullPath);
            CheckSourceFolder(e.Name, e.FullPath);
        }

        static void OnDelete(object sender, FileSystemEventArgs e) {
            lib.WriteLog(1, "Deleted: " + e.FullPath);
        }

        static void OnRenamed(object sender, RenamedEventArgs e) {
            lib.WriteLog(1, "Renamed: " + e.OldName + " => " + e.Name);
            CheckSourceFolder(e.Name,e.FullPath);
            
        }

        static void CheckSourceFolder(string NewName, string fullpath) {
            if (Regex.IsMatch(NewName, Global.pattern)) {
                lib.WriteLog(0, "Match! " + NewName);
                Console.Write("Waiting no change 10sec:");
                WaitSrc(fullpath);
                int result = lib.ExecuteShellReturnExitCode(Global.ExecuteCode + " " + NewName);
                lib.WriteLog(0, "Executed result: " + result);
            }
            else {
                lib.WriteLog(0, "CheckSourceFolder(" + NewName + ") is not Matched.");
            }

        }

        static void WaitSrc(string path) {
            int cnt = 0;
            long BeforeFolderSize = 0;
            while (cnt <= 10) {
                DirectoryInfo di = new DirectoryInfo(path);

                long FolderSize=di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);
                if(FolderSize > BeforeFolderSize) {
                    BeforeFolderSize = FolderSize;
                    cnt = 0;
                } else {
                    cnt++;
                }
                System.Threading.Thread.Sleep(1000);
                Console.Write(" " + cnt);
            }
            Console.Write("\r\n");
        }    
    }
}
