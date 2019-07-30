using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Net;
using System.Management.Automation;
using System.ServiceProcess;
namespace Install_Deployer_exe
{
    class Program
    {
        //Метод который проверяет установлен ли указанный сервис
        private static bool IsServiceInstalled(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == serviceName) return true;
            }

            return false;
        }

        static void Main(string[] args)
        {
            string serviceName = String.Empty;
            string ftpLogin = String.Empty;
            string ftpPassword = String.Empty;
            string ftpPath = String.Empty;
            try
            {
                //Если сервис установлен, то не делает ничего
                if (IsServiceInstalled(serviceName))
                {
                    return;
                }
                //Если нет, то определяет параметры для установки и устанавливает
                String fileName, FilePath, installUtil, OS;
                OS = PowerShell.Create().AddScript("(Get-WmiObject Win32_OperatingSystem).caption + ' ' +(Get-WmiObject Win32_OperatingSystem).OSArchitecture + ' SP'+(Get-WmiObject Win32_OperatingSystem).ServicePackMajorVersion").Invoke()[0].ToString();
                string[] dirs = Directory.GetFiles(@"C:\Windows\Microsoft.NET\", "InstallUtil.exe", SearchOption.AllDirectories);
                var installUtil_d = new List<String>();

                foreach (string dir in dirs)
                {
                    if (dir.Contains("v4."))
                    {
                       installUtil_d.Add(dir);
                    }
                }

                installUtil = Convert.ToString(installUtil_d[0]);
                FilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                fileName = (FilePath + "\\SQL_client\\SQL_client.txt");
                Directory.CreateDirectory(FilePath + "\\SQL_client");
                //Качает нужные файлы с ФТП
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpPath);
                ftpRequest.Credentials = new NetworkCredential(ftpLogin, ftpPassword);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                List<string> directories = new List<string>();
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                      directories.Add(line);
                      line = streamReader.ReadLine();
                }
                streamReader.Close();


                using (WebClient ftpClient = new WebClient())
                {
                      ftpClient.Credentials = new System.Net.NetworkCredential(ftpLogin,ftpPassword);
                    for (int i = 0; i <= directories.Count - 1; i++)
                    {
                        if (directories[i].Contains("."))
                        {
                            string path = ftpPath + directories[i].ToString();
                            string trnsfrpth = FilePath + @"\SQL_client\" + directories[i].ToString();
                            ftpClient.DownloadFile(path, trnsfrpth);
                        }
                    }
                }
                //Устанавливает
                ProcessStartInfo info = new ProcessStartInfo(installUtil);
                info.UseShellExecute = true;
                info.Arguments = (" " + '"' + FilePath + @"\SQL_client\ServiceDeployer.exe" + '"');
                if (OS.Contains("XP"))
                {
                    info.Verb = "";
                }
                else
                {
                    info.Verb = "runas";
                }
                Process.Start(info);
                System.Threading.Thread.Sleep(5000);
            }
            catch (Exception err)
            {
            }
            try
            {
                ServiceController service = new ServiceController("ServiceDeployer");
                TimeSpan timeout = TimeSpan.FromMilliseconds(5000);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception eh)
            {

            }

        }
    }
}
