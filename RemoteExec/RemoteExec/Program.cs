using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteExec
{
    class Program
    {
        /* D:\SysinternalsSuite_20181210>psexec \\172.21.150.190 -nobanner cmd /c query session | findstr Active
        
 SESSIONNAME       USERNAME ID  STATE TYPE        DEVICE
>services                                    0  Disc
 console                                     1  Conn
                   muoadmin_tg               2  Disc
 rdp-tcp#2         muoadmin_tg               3  Active
 rdp-tcp                                 65536  Listen
cmd exited on 172.21.150.190 with error code 1.

D:\SysinternalsSuite_20181210>

        */


        static void Main(string[] args) {
        }







    }
}
