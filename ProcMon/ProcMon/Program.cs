using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Security;
using System.Collections;



namespace ProcMon
{
    class Program
    {
        static ClassLibrary.CommonLibrary lib = new ClassLibrary.CommonLibrary();

        static void Main(string[] args) {
            Console.Title = "";
            Boolean LocalFlag = false;
            //Local();
            //Console.WriteLine("FIN");
            //Console.ReadLine();

            #region inputargs check
            if (args.Length >= 1) {
                if (args[0].ToLower() == "local") {
                    LocalFlag = true;
                }
                else {
                    if (args.Length != 3) {
                        Console.WriteLine("Input arguments");
                        Console.WriteLine("[0] AD domain");
                        Console.WriteLine("[1] AD account");
                        Console.WriteLine("[2] password");
                        Environment.Exit(-1);
                    }
                }
            } else {
                Console.WriteLine("Input arguments");
                Console.WriteLine("[0] AD domain");
                Console.WriteLine("[1] AD account");
                Console.WriteLine("[2] password");
                Environment.Exit(-1);
            }
            #endregion

            ClassLibrary.CommonSetting.LogFilePath = string.Concat(Directory.GetCurrentDirectory(), "\\Procmon");

            //[servername][hostname]
            string[,] ServerList = GetServerList();
            int ServerListLength = ServerList.Length / ServerList.Rank;

            string ProcessName = lib.LoadINI("processname", "\\Procmon.ini");
            string PortNo = lib.LoadINI("portno", "\\Procmon.ini");
            string Mailto = lib.LoadINI("Mailto", "\\Procmon.ini");
            if (Mailto == null) {
                lib.Terminate("MailTo is not assigned!!!");
            }
            #region inputargs process
            string DomainName = "";
            string account = "";
            string plainPassText="";

            if (LocalFlag == false) {
                DomainName = args[0];
                account = args[1];
                //プレーンテキストをストリングにセット
                plainPassText = args[2];
            }
            
            //SecureStringオブジェクトを作成
            System.Security.SecureString secStr = new System.Security.SecureString();
            //AppendChareで一文字づつ追加
            foreach (char c in plainPassText)
                secStr.AppendChar(c);
            #endregion

            long[] LastEventIdx = new long[ServerListLength];

            // first: get LastEventIdx
            if (LocalFlag == false) {
                #region 
                for (int j = 0; j <= ServerListLength - 1; j++) {
                    try {
                        EventLogSession session = new EventLogSession(ServerList[j, 1], DomainName, account, secStr, SessionAuthentication.Default);
                        EventLogQuery query = new EventLogQuery("Application", PathType.LogName, "*");
                        query.ReverseDirection = true;
                        query.Session = session;
                        EventLogReader logReader = new EventLogReader(query);
                        EventRecord eventdetail = logReader.ReadEvent();
                        LastEventIdx[j] = long.Parse(eventdetail.RecordId.ToString());
                        lib.WriteLog(0, ServerList[j, 0] + " LastIdx:" + LastEventIdx[j]);
                    }
                    catch (Exception e) {
                        lib.WriteLog(2, "Ex in getLastEventCheck: " + ServerList[j, 0]);
                        lib.WriteLog(0, e.Message);
                    }
                }
                #endregion
            }
            else {
                EventLogQuery query = new EventLogQuery("Application", PathType.LogName, "*");
                query.ReverseDirection = true;
                EventLogReader logReader = new EventLogReader(query);
                EventRecord eventdetail = logReader.ReadEvent();
                LastEventIdx[0] = long.Parse(eventdetail.RecordId.ToString());
                lib.WriteLog(0, ServerList[0, 0] + " LastIdx:" + LastEventIdx[0]);
            }
                

            while (true) {
                
                for (int j = 0; j <= ServerListLength - 1; j++) {
                    try {
                        //string query = "*[System/EventID=1000]";
                        string queryString = "";
                        queryString = "*[System[(EventRecordID > " + LastEventIdx[j] + ")]]";
                        

                        EventLogSession session = new EventLogSession(ServerList[j, 1], DomainName, account, secStr, SessionAuthentication.Default);
                        //secStr.Dispose();

                        EventLogQuery query = new EventLogQuery("Application", PathType.LogName, queryString);
                        if (LocalFlag==false)
                            query.Session = session;

                        EventLogReader logReader = new EventLogReader(query);

                        for (EventRecord eventdetail = logReader.ReadEvent(); eventdetail != null; eventdetail = logReader.ReadEvent()) {

                            if (eventdetail.RecordId != LastEventIdx[j]) {
                                Console.Title = ServerList[j,0] + " BeforeIdx:" + LastEventIdx[j] + "\t" + DateTime.Now.ToString("HH:mm:ss");
                                // Read Event details
                                lib.WriteLog(0,ServerList[j,0] + "\t" + eventdetail.RecordId);
                                foreach (var detail in eventdetail.Properties)
                                    //processname found!
                                    if (detail.Value.ToString().Contains(ProcessName)==true) {
                                        ArrayList al = new ArrayList();
                                        al.Add(ServerList[j, 0] + " Application crashed");
                                        al.Add(eventdetail.MachineName);
                                        al.Add("EventRecordID: " + eventdetail.RecordId);
                                        al.Add("EventName: " + eventdetail.ProviderName);
                                        foreach (var dt in eventdetail.Properties)
                                            al.Add(dt.Value.ToString());
                                        lib.WriteLog(1, "EventLog found: " + ServerList[j, 0] + " Idx: " + eventdetail.RecordId);
                                        lib.WriteLog(1, detail.Value.ToString());
                                        WriteFile(al, "mail.txt");
                                                                                
                                        lib.WriteLog(1, "SendMail Result: " + SendMail(Mailto,"mail.txt"));
                                    }
                                    else {
                                        //lib.WriteLog(0, "eventdetail.Properties: " + detail.Value.ToString());
                                    }
                                Console.Title = string.Concat(Console.Title, "(+", long.Parse(eventdetail.RecordId.ToString()) - LastEventIdx[j], ")");
                                LastEventIdx[j] = long.Parse(eventdetail.RecordId.ToString());
                            }
                            else {
                                Console.Title = ServerList[j, 0] + " LastIdx:" + LastEventIdx + " No update " + DateTime.Now.ToString("HH:mm:ss");
                            }

                        }
                    }
                    catch (EventLogException e) {
                        lib.WriteLog(2, "Could not query the remote computer! " + ServerList[j, 0]);
                        lib.WriteLog(0,e.Message);
                        return;
                    }
                    System.Threading.Thread.Sleep(1000 * 3);
                }
                string consolestr = Console.Title;
                for(int i = 0; i<= 30; i++){
                    System.Threading.Thread.Sleep(1000 * 1);
                    Console.Title = consolestr + " " + i + "/30 waiting";
                }
            }
        }
            
        static String[,] GetServerList(string FilePath = "\\serverlist.ini") {
            FilePath = Directory.GetCurrentDirectory() + FilePath;
            ArrayList ServerName = new ArrayList();
            ArrayList HostName = new ArrayList();
            

            try {
                using(StreamReader sr=new StreamReader(FilePath)) {
                    string line;
                    while((line=sr.ReadLine()) != null) {
                        string[] split = line.Split(new char[] { ',' });
                        ServerName.Add(split[0]);
                        HostName.Add(split[1]);
                       
                    }
                }
            }
            catch (Exception e) {
                lib.WriteLog(2, "Serverlist cannot loaded!!!");
                lib.WriteLog(0, e.Message);
                return null;
            }

            string[,] ServerList= new string[ServerName.Count, 2];
            for(int j = 0; j <= ServerName.Count-1; j++) {
                ServerList[j, 0] = ServerName[j].ToString();
                ServerList[j, 1] = HostName[j].ToString();
            }
            lib.WriteLog(1, "Serverlist loaded cnt:" + ServerName.Count);
            return ServerList;
        }

        static void Local() {
            long LastEventIdx = 75990;
            //75990
            while (true) {
                Console.Title = "";

                string query = "*EventRecordID>" + LastEventIdx;
                query = "*[System[(EventRecordID > " + LastEventIdx + ")]]";

                // + ") and EventID=1000]]";
                //query = "*[System[(EventRecordID >= " + LastEventIdx + ") and ProviderName=Application Error]]";


                EventLogQuery eventsQuery = new EventLogQuery("Application", PathType.LogName, query);
                EventLogReader logReader = new EventLogReader(eventsQuery);

                for (EventRecord eventdetail = logReader.ReadEvent(); eventdetail != null; eventdetail = logReader.ReadEvent()) {
                    //&& eventdetail.ProviderName == "Application Error"
                    if (eventdetail.RecordId != LastEventIdx ) {
                        
                        Console.Title = "BeforeIdx:" + LastEventIdx;
                        // Read Event details
                        Console.WriteLine(DateTime.Now + "\t" + eventdetail.RecordId + "\t" + eventdetail.TimeCreated.ToString());
                        foreach (var detail in eventdetail.Properties)
                            Console.WriteLine(detail.Value.ToString());
                        Console.Title = string.Concat(Console.Title, "(+", long.Parse(eventdetail.RecordId.ToString()) - LastEventIdx, ")");
                        LastEventIdx = long.Parse(eventdetail.RecordId.ToString());
                    }
                    else {
                        Console.Title = "LastIdx:" + LastEventIdx + " No update " + DateTime.Now.ToShortTimeString();
                    }

                }
                logReader.Dispose();

                System.Threading.Thread.Sleep(1000 * 10);
            }

        }

        static int WriteFile(ArrayList al,String OutFileName) {
            string FilePath = Directory.GetCurrentDirectory() + "\\" + OutFileName;
            try {
                using (FileStream fs = File.Open(OutFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)) {
                    foreach(string line in al) {
                        byte[] b = new UTF8Encoding(true).GetBytes(line + "\r\n");
                        UTF8Encoding temp = new UTF8Encoding(true);
                        fs.Write(b, 0, b.Length);
                    }
                }
                return 1;
            } catch (Exception e) {
                lib.WriteLog(2,"EX in WriteFile: " + OutFileName);
                lib.WriteLog(0, e.Message);
                return 0;
            }
        }
        
        static int SendMail(string addr,string FileName) {
            return lib.ExecuteShellReturnExitCode(Directory.GetCurrentDirectory() + "\\smtp.exe " + addr + " " + FileName);
        }
    }
}
