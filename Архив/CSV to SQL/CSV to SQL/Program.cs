using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;

namespace CSV_to_SQL
{
    class Program
    {
        static void Main(string[] args)
        {
            string sqlServer = String.Empty;
            string sqlLogin = String.Empty;
            string sqlPassword = String.Empty;
            StreamReader sw = new StreamReader("C:\\1\\kumw.csv");
            List<string> list = new List<string>();
            string DisplayName, mail, Title, mailnickname, Company, samaccountname, Telephonenumber;
            string Department = String.Empty;
            string temp = sw.ReadLine();
            int i=2;
            while (temp!=null)
            {
                list.Add(temp.Replace("\"",""));
                temp = sw.ReadLine();
            }

            while (i < list.Count)
            {
                //DisplayName,"mail","title","mailnickname","company","samaccountname","telephonenumber"
                DisplayName = list[i].Substring(0, list[i].IndexOf(","));
                list[i]=list[i].Substring(list[i].IndexOf(",")+1);

                mail = list[i].Substring(0, list[i].IndexOf(","));
                list[i] = list[i].Substring(list[i].IndexOf(",")+1);

                Title = list[i].Substring(0, list[i].IndexOf(","));
                list[i] = list[i].Substring(list[i].IndexOf(",") + 1);

                mailnickname = list[i].Substring(0, list[i].IndexOf(","));
                list[i] = list[i].Substring(list[i].IndexOf(",") + 1);

                Company = list[i].Substring(0, list[i].IndexOf(","));
                list[i] = list[i].Substring(list[i].IndexOf(",") + 1);

                samaccountname = list[i].Substring(0, list[i].IndexOf(","));
                list[i] = list[i].Substring(list[i].IndexOf(",") + 1);

                Telephonenumber = list[i];

                using (SqlConnection connection = new SqlConnection("Data Source="+sqlServer+";Initial Catalog=PCLIST;User Id="+sqlLogin+";Password ="+sqlPassword+";"))
                {
                    string queryString = @"
             --Обновление данных о пользователях

                       
                        MERGE [pclist].[dbo].[UserInfo] trgt
                        USING (SELECT * FROM (
                        VALUES('" + Company +
                "','" + DisplayName +
                "','" + mail +
                "','" + Title +
                "','" + Department +
                "','" + Telephonenumber + @"'
                        ,'1'
                     )
                        ) s ([Company],[DisplayName], [mail], [Title],[Department],[Telephonenumber],[Enabled])) src
                        ON src.[mail] = trgt.[mail]
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
                            INSERT ([DisplayName],
                                   [mail],
                                   [Title],
                                   [Department],
                                   [Telephonenumber],
                                   [Company],
                                   [Enabled]
                                   )
                            VALUES (src.[DisplayName],
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
                }
                i++;
            }
        }
    }
}
