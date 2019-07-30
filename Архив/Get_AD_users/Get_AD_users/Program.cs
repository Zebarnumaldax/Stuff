using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace Get_AD_users
{
    class Program
    {
        static void Main(string[] args)
        {
            string sqlServer = String.Empty;
            string sqlLogin = String.Empty;
            string sqlPassword = String.Empty;
            string path = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                String domain = "";
                string ADSI = "ADSI";
                string LDAP = "LDAP://alupro.domain/dc=alupro,dc=domain";
                String IP = PowerShell.Create().AddScript("((ipconfig | findstr [0-9].\\.)[0]).Split()[-1]").Invoke()[0].ToString();
                if (IP.Contains("10.29.1."))
                {
                    LDAP = "LDAP://alupro.domain/dc=alupro,dc=domain";
                    ADSI = "ADSI";
                    domain = "DC = alupro,DC = domain";
                    string alupro = "УК \"Алюминиевые продукты\"";
                    SqlConnection connection = new SqlConnection("Data Source=" + sqlServer + ";Initial Catalog=PCLIST;User Id=" + sqlLogin + ";Password =" + sqlPassword + ";");
                    string queryString = @"UPDATE [pclist].[dbo].[UserInfo]
                                       SET [Enabled] = '0'
                                       WHERE Company ='" + alupro + @"'";
                    SqlConnection sqlConnection = new SqlConnection();
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
                if (IP.Contains("10.29.2.")||IP.Contains("10.29.3."))
                {
                    LDAP = "LDAP://10.29.2.2/dc=scovo,dc=domain";
                    ADSI = "Scovo_ADSI";
                    domain = "DC = scovo,DC = domain";
                    string Scovo = "ООО \"Сково\"";
                    string Demid = "ЗАО «Завод» Демидовский»";
                    string TD = "ООО «ТД «Сково»";
                    SqlConnection connection = new SqlConnection("Data Source=" + sqlServer + ";Initial Catalog=PCLIST;User Id=" + sqlLogin + ";Password =" + sqlPassword + ";");
                    string queryString = @"UPDATE [pclist].[dbo].[UserInfo]
                                       SET [Enabled] = '0'
                                       WHERE Company ='" + Scovo + @"' or Company ='" + Demid + @"' or Company ='" + TD + @"'";
                    SqlConnection sqlConnection = new SqlConnection();
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    sqlConnection.Close();
                    sqlConnection.Dispose();

                }

                String logonname = "Get-ADUser -searchbase \"" + domain + "\" -Filter * -Properties * | where {$_.mail -ne $null -and $_.Telephonenumber -ne $null -and ($_.Enabled -eq $true) -and ($_.Company -ne $null)} | select DisplayName, mail, Title, Telephonenumber,samaccountname, Mailnickname, Company, department, givenName ";

                using (PowerShell powershell = PowerShell.Create().AddScript(@"import -module ActiveDirectory").AddScript(logonname))
                {

                    foreach (PSObject result in powershell.Invoke())
                    {
                        string str = result.ToString();
                        string[] res = str.Split(';');
                        string DisplayName = res[0].Substring(res[0].ToString().IndexOf("=") + 1);
                        string SAM = res[4].Substring(res[4].ToString().IndexOf("=") + 1);
                        string mail = res[1].Substring(res[1].ToString().IndexOf("=") + 1);
                        string Title = res[2].Substring(res[2].ToString().IndexOf("=") + 1);
                        string Telephonenumber = res[3].Substring(res[3].ToString().IndexOf("=") + 1);
                        string Company = res[6].Substring(res[6].ToString().IndexOf("=") + 1);
                        string Department = res[7].Substring(res[7].ToString().IndexOf("=") + 1);

                        SqlConnection connection = new SqlConnection("Data Source=" + sqlServer + ";Initial Catalog=PCLIST;User Id=" + sqlLogin + ";Password =" + sqlPassword + ";");
                        string queryString = @"
             --Обновление данных о пользователях

                        WITH cte AS (SELECT [sid].objectsid  FROM OPENQUERY(" + ADSI + @", 'SELECT objectsid
                        FROM ''" + LDAP + @"'' 
                        WHERE sAMAccountName = ''" + SAM + @"''
                        ') [sid])
                        MERGE [pclist].[dbo].[UserInfo] trgt
                        USING (SELECT * FROM (
                        VALUES('" + Company +
                        "','" + DisplayName +
                        "','" + mail +
                        "','" + Title +
                        "','" + Department +
                        "','" + Telephonenumber + @"'
                        , (SELECT TOP 1 [objectsid] FROM cte),'1'
                     )
                        ) s ([Company],[DisplayName], [mail], [Title],[Department],[Telephonenumber],[SID],[Enabled])) src
                        ON src.[SID] = trgt.[SID]
                        WHEN MATCHED
                          THEN
                            UPDATE
                              SET
                                trgt.[Title] = src.[Title],
                                trgt.[Department] = src.[Department],
                                trgt.[DisplayName] = src.[DisplayName],  
                                trgt.[mail] = src.[mail],
                                trgt.[Telephonenumber]= src.[Telephonenumber],
                                trgt.[Company]= src.[Company],
                                trgt.[Enabled]=src.[Enabled]
                                

        
                        WHEN NOT MATCHED
                          THEN
                            INSERT ([SID],
                                   [DisplayName],
                                   [mail],
                                   [Title],
                                   [Department],
                                   [Telephonenumber],
                                   [Company],
                                   [Enabled]
                                   )
                            VALUES (src.[SID],
                                   src.[DisplayName],
                                   src.[mail],
                                   src.[Title],
                                   src.[Department],
                                   src.[Telephonenumber],
                                   [Company],
                                   '1'
                                   );";
                        SqlConnection sqlConnection = new SqlConnection();
                        SqlCommand command = new SqlCommand(queryString, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        sqlConnection.Close();
                        sqlConnection.Dispose();

                    }

                }
            }
            catch(Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path+@"exception.txt"))
            {
                file.WriteLine(e.Message);
            }
            }
        }
    }
}
