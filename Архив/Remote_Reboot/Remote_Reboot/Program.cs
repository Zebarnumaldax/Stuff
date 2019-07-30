using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace Remote_Reboot
{
    static class Program
    {
    static void Main()
        {
            string[] rebootList = new string[] {"10.29.2.108",
"10.29.2.107",
"10.29.2.197",
"10.29.2.119",
"10.29.2.147",
"10.29.2.103",
"10.29.2.166",
"10.29.2.167",
"10.29.2.155",
"10.29.2.160",
"10.29.2.137",
"10.29.2.110",
"10.29.2.190",
"10.29.2.136",
"10.29.2.175",
"10.29.2.146",
"10.29.2.195",
"10.29.2.104",
"10.29.2.114",
"10.29.2.117",
"10.29.2.111",
"10.29.2.192",
"10.29.2.102",
"10.29.2.112",
"10.29.2.120",
"10.29.2.162",
"10.29.2.149",
"10.29.2.100"
            };
            int i = 0;
            while (i< rebootList.Length)
            {
                Process.Start("shutdown", "-r -m " + @"\\" + rebootList[i] + " -t 00");
                System.Threading.Thread.Sleep(1000);
                i = i+1;
            }
        }
    }
}
