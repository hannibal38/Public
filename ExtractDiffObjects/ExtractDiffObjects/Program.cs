using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;


namespace ExtractDiffObjects
{
    class Program
    {
        static string DiffFolder;
        static string ObjectFolder;
        static string LogPath;
        static ArrayList ErrorAL = new ArrayList();
        static ArrayList ErrorMsgAL = new ArrayList();
        static ArrayList FileNameAL = new ArrayList();
        static int FileCount = 0;
        static int ProceedCnt = 0;
        static string TotalFiles;

        static ClassLibrary.CommonLibrary lib = new ClassLibrary.CommonLibrary();


        public static class Global
        {

            //public static string pattern = lib.LoadINI("SearchPattern", "\\FileWatcher.ini");
            //public static string ExecuteCode = lib.LoadINI("ExecutePath", "\\FileWatcher.ini");
        }

        static void Main(string[] args) {
            Console.BufferHeight = 8000;

            //ExceptionLogger el = new ExceptionLogger(ex);
            //System.Threading.Thread t = new System.Threading.Thread(el.DoLog);
            //t.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //t.Start();

            ClassLibrary.CommonSetting.LogFilePath = string.Concat(Directory.GetCurrentDirectory());
            ClassLibrary.CommonSetting.DebugFlag = true;

            /* 1 compared index.log
             * 2 source path
             * 3 extract path
             */

            for (int i = 0; i <= args.Length - 1; i++)
                Console.WriteLine("Input arg[" + i + "] : " + args[i]);
            
            if (args.Length < 3) {
                NoArgs();
            }
            else {
                LogPath = args[0];
                ObjectFolder = args[1];
                DiffFolder = args[2];
                CheckArgs();
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ReadDiffLog(LogPath);

            DirectoryInfo di = new DirectoryInfo(DiffFolder);
            long totalSize = di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            Console.WriteLine(DiffFolder + "Size: " + lib.CastToGiga(totalSize));


            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine("Elapsed time: " + ts.Minutes + "Min " + ts.Seconds + "Sec.");
            Console.WriteLine("Copied file: " + FileCount);
            Console.WriteLine("Error count:" + ErrorMsgAL.Count);
            if (ErrorAL.Count != 0) {
                try {
                    using (FileStream fs = File.Open(DiffFolder + ".log", FileMode.Create, FileAccess.Write, FileShare.ReadWrite)) {
                        UTF8Encoding temp = new UTF8Encoding(true);
                        for (int i =0; i <= ErrorAL.Count - 1; i++) {
                            byte[] b = new UTF8Encoding(true).GetBytes(ErrorAL[i] + "\t" + ErrorMsgAL[i] + "\n");
                            fs.Write(b, 0, b.Length);
                        }                       
                    }
                }
                catch (Exception e) {
                    Console.WriteLine("Write ErrorArray Error!!!");
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("ErrorArray ===>" + DiffFolder + ".log");
                Environment.Exit(1);
            }
            else {
                Console.WriteLine("No error occurred.");
                Environment.Exit(0);
            }

        }
        static void ReadDiffLog(string LogPath) {

            using (StreamReader sr = new StreamReader(LogPath)) {
                string lineStr = "";
                while ((lineStr = sr.ReadLine()) != null) {

                    switch (lineStr) {
                        case string a when a.Contains("MODIFIED"):
                            ProceedCnt += 1;
                            SplitLine(lineStr);
                            break;
                        case string a when a.Contains("count"):
                            Console.WriteLine(lineStr);
                            //TODO: 출력하기
                            break;
                        case string a when a.Contains("DELETED"):
                            ProceedCnt += 1;
                            Console.Title = "[" + ProceedCnt + "/" + TotalFiles + " " + DiffFolder;
                            break;
                        case string a when a.Contains("ADD"):
                            //TODO: 바로복사
                            ProceedCnt += 1;
                            SplitLine(lineStr);
                            break;
                        default:
                            break;

                    }

                }

            }


        }

        static Boolean AddError(string FileName, string ex) {
            foreach (string str in ErrorAL) {
                if (FileName == str) {
                    return false;
                }
            }
            
            ErrorAL.Add(FileName);
            ErrorMsgAL.Add(ex);
            return true;
        }

        static void CheckArgs() {
            if (!File.Exists(LogPath)) {
                Console.WriteLine("Not exist logfile!!!");
                Environment.Exit(-1);
            }

            if (!Directory.Exists(ObjectFolder)) {
                Console.WriteLine("Wrong ObjectFolder path!!!");
                Environment.Exit(-2);
            }
            if (!DiffFolder.Contains("\\"))
                DiffFolder = Directory.GetCurrentDirectory() + "\\" + DiffFolder;
            if (!Directory.Exists(DiffFolder))
                Directory.CreateDirectory(DiffFolder);
        }


        static void NoArgs() {
            Console.WriteLine("Input args");
            Console.WriteLine("1: compared index.log");
            Console.WriteLine("2: source path");
            Console.WriteLine("3: extract path");
            Environment.Exit(0);
        }

        static void SplitLine(string str) {
            str = str.Replace(" ", "/");
            do {
                str = str.Replace("//", "/");
            } while (str.Contains("//"));

            string[] LineArray = str.Split('/');
            LineArray[1] = LineArray[1].Replace("[", "");
            string LogMsg = "";
            TotalFiles = LineArray[2];
            
            ///* 0 \\
            // * 1 [1
            // * 2 265]
            // * 3 MODIFIED
            // * 4 17.98MB
            // * 5 cc22445b old
            // * 6 bin\win64\efengin.dll
            // * 7 147ac0a4 new
            // * 8 bin\win64\efeingin.dll
            // * /

            LogMsg = LogMsg + String.Format("{0,3}", LineArray[1] + " ");
            Console.Title = "[" + ProceedCnt + "/" +  TotalFiles + " " + DiffFolder;

            switch (LineArray.Length) {
                case 9: //modified
                    CopyRoutine(LineArray[7]);
                    Console.WriteLine(LineArray[6] + " ");
                    break;

                case 10: // modified with 0 column  MODIFIED 0 14.00B
                    CopyRoutine(LineArray[8]);
                    Console.WriteLine(LineArray[7] + " ");
                    break;

                case 7:
                    CopyRoutine(LineArray[5]);
                    Console.WriteLine(LineArray[6] + " ");
                    break;
            }
        }

        static string SplitFileName(string str) {
            //D:\dev\Heartbeat\.git\objects\14\7ac0a46bd8c2e97a354afedc4e556ce7366db9

            string[] LineArray = str.Split('\\');
            return LineArray[LineArray.Length - 1];
        }

        static Boolean CopyRoutine(string path) {
            string path1 = path.Substring(0, 2);
            string path2 = path.Substring(2, path.Length - 2);
            Console.Write(path1 + "\\" + path2 + " => ");
            string[] files = { "" };
            try {
                files = Directory.GetFiles(ObjectFolder + "\\" + path1, path2 + "*");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.GetType().ToString());
                AddError(path1 + "\\" + path2, ex.GetType().ToString());
                return false;
            }

            if (files.Length == 0) {
                Console.WriteLine("FileNotExist in Object Folder");
                AddError(path1 + "\\" + path2,"FileNotExist in Object Folder");
                return false;
            }
            else {
                string FileName = SplitFileName(files[0]);

                if (files.Length == 1) {
                    Console.Write(FileName + " ===> ");
                    try {
                        if (Directory.Exists(DiffFolder + "\\" + path1 + "\\") == false)
                            Directory.CreateDirectory(DiffFolder + "\\" + path1 + "\\");

                        if (File.Exists(DiffFolder + "\\" + path1 + "\\" + FileName) == true) {
                            Console.WriteLine("Alredy exist. Skip...");
                        }
                        else {
                            File.Copy(files[0], DiffFolder + "\\" + path1 + "\\" + FileName);
                            Console.WriteLine("Copied...");
                            FileCount += 1;
                        }
                    }
                    catch (Exception e) {
                        AddError(path1 + "\\" + path2, e.GetType().ToString());
                    }
                }
                else { //2++
                    foreach (string str in files) {
                        Console.WriteLine("Files[]: " + str);
                    }
                    Console.WriteLine("Error!!!");
                    AddError(path, "Directory.GetFiles(" + ObjectFolder + "\\" + path2 + " returned 2++");
                }
                return true;
            }



            //ErrorAL.Add(path);
            //ErrorMsgAL.Add(ex.Message);

        }
        //File.Copy(@"E:\server_root\games\lostark\git\.git\objects" + "\\" + path1 + "\\" + path2);

    }
    class ExceptionLogger
    {
        Exception _ex;

        public ExceptionLogger(Exception ex) {
            _ex = ex;
        }
        public string DoLog() {
            return _ex.GetType().ToString();
        }
    }

}
