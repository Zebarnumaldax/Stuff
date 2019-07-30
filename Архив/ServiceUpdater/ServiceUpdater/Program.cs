using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using System.Runtime.InteropServices;



namespace ServiceUpdater
{

    class Program
    {
       
        public static String conStr = "Data Source=10.29.1.180\\SQLEXPRESS;Initial Catalog=PCLIST;User Id=script_logon;Password = ;";
        static void Main()
        {
            string[] dirs = Directory.GetFiles(@"C:\Windows\Microsoft.NET\", "InstallUtil.exe", SearchOption.AllDirectories);
            var installUtil = new List<String>();
            foreach (string dir in dirs)
            {
                if (dir.Contains("v4."))
                {
                    installUtil.Add(dir);                   
                }
            }
            String FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String fileName = (FilePath + "\\SQL_client\\SQL_client.txt");
            System.IO.Directory.CreateDirectory(FilePath + "\\SQL_client");
            SqlConnection connection = new SqlConnection(conStr);
            string queryString = @"SELECT TOP (1) 
                                   [version] 
                                   FROM [pclist].[dbo].[Param] 
                                   where [version] is not null";
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            object Ver = command.ExecuteScalar();
            sqlConnection.Close();
            sqlConnection.Dispose();

            string currVer = Convert.ToString(Ver);
            try
            {   
                StreamReader file = new StreamReader(fileName);
                string line = file.ReadLine();
                file.Close();
                if (line != currVer)
                {
                    //Copy new .exe to app folder.
                    File.Copy(Path.Combine("\\\\10.29.2.3\\ps", "SQL_client.exe"), Path.Combine(FilePath+"\\SQL_client", "SQL_client.exe"), true);
                    //Uninstall old service.
                    ProcessStartInfo info = new ProcessStartInfo(installUtil[0]);
                    info.UseShellExecute = true;
                    info.Arguments = ("/u "+FilePath+ @"\SQL_client.exe ");
                    info.Verb = "runas";
                    Process.Start(info);
                    //Wait till done.
                    System.Threading.Thread.Sleep(5000);
                    //Install new app.
                    ProcessStartInfo installClinet = new ProcessStartInfo(installUtil[0]);
                    info.UseShellExecute = true;
                    info.Arguments = (FilePath + @"\SQL_client.exe ");
                    info.Verb = "runas";
                    Process.Start(installClinet);
                    //Update version info on local pc.
                    using (StreamWriter update =
                    new StreamWriter(fileName))
                    {
                        update.WriteLine(currVer);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                using (StreamWriter writer =
            new StreamWriter(fileName))
                {
                    writer.WriteLine("First lunch");

                }
            }
        }
    }
}    
    



