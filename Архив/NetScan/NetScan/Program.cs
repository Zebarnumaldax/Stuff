using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Security;
using System.ServiceProcess;
using System.Threading;

namespace NetScan
{

    class Program
    {
        public static void ProcStat(string remoteSystem)
        {
            ServiceController sc = new ServiceController();
            string status = "Не возможно подключиться: ";
            string hostName = string.Empty;
            string procSearch = "SQL_Client";
            sc.MachineName = remoteSystem;
            sc.ServiceName = procSearch;
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(remoteSystem);
                hostName = hostEntry.HostName;
                if (hostName != string.Empty)
                {
                    System.Net.NetworkInformation.PingReply rep = p.Send(remoteSystem, 100);
                    try
                    {
                        if (sc.Status.Equals(ServiceControllerStatus.Running))
                        {
                            status = "SQL_Client Запущен: ";
                        }
                    }
                    catch (Exception e)
                    { }
                    if (rep.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        Console.WriteLine(status + hostName + " | " + remoteSystem);
                        using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + @"\SQL_Client_Status.txt"))
                        {
                            sw.WriteLine(status + hostName + " | " + remoteSystem);
                            sw.Close();
                        }
                    }

                }

            }
            catch { }
        }

        static void Main(string[] args)
        {
            string[] addresses = new string[255];
            Console.WriteLine("Введите подсеть в виде: 10.29.1.");
            string network = Console.ReadLine();
            for (int i = 0; i < 255; i++)
            {
                addresses[i] = network + i.ToString();
            }
            foreach (var el in addresses)
            {
                Task.Factory.StartNew(() =>
                {
                    ProcStat(el);
                });
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}

