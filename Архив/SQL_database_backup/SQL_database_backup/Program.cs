using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_database_backup
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Data Source=10.29.1.180\\SQLEXPRESS;User Id=script_logon;Password =  ;");
            string queryString = @"
             --Бекап баз данных
         BACKUP DATABASE pclist
         TO DISK = 'E:\Backup\pclist.Bak'  
            WITH FORMAT,  
              MEDIANAME = 'Z_SQLServerBackups',  
              NAME = 'Full Backup of pclist';  

         BACKUP DATABASE whelped
         TO DISK = 'E:\Backup\whelped.Bak'  
            WITH FORMAT,  
              MEDIANAME = 'Z_SQLServerBackups',  
              NAME = 'Full Backup of whelped';  
                        
                                   ";
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
            }
        }
    }
}
