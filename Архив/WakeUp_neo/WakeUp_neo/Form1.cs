using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Net;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WakeUp_neo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("Введите IP адресс компьютера:");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                String ip = textBox1.Text;
                String conStr = "Data Source=10.29.1.180\\SQLEXPRESS;Initial Catalog=PCLIST;User Id=script_logon;Password = ;";
                SqlConnection connection = new SqlConnection(conStr);
                string queryString = @"SELECT TOP (1) 
                                   [MAC] 
                                   FROM [pclist].[dbo].[PC] 
                                   where [IP] = '" + ip + "' ";
                SqlConnection sqlConnection = new SqlConnection();
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                object Get = command.ExecuteScalar();
                sqlConnection.Close();
                sqlConnection.Dispose();
                string MAC = Convert.ToString(Get).Replace(":", "");
                Console.WriteLine("MAC адресс машины:" + MAC + ". Идёт запуск...");


                int counter = 0;
                byte[] bytes = new byte[1024];
                //Первые 6 бит 0xFF
                for (int y = 0; y < 6; y++)
                    bytes[counter++] = 0xFF;
                //Повторим MAC адрес 16 раз
                for (int y = 0; y < 16; y++)
                {
                    int i = 0;
                    for (int z = 0; z < 6; z++)
                    {

                        bytes[counter++] = byte.Parse(MAC.Substring(i, 2), NumberStyles.HexNumber);
                        i += 2;
                    }
                }

                UdpClient client = new UdpClient();
                IPEndPoint ips = new IPEndPoint(IPAddress.Broadcast, 15000);
                client.Send(bytes, bytes.Length, ips);
                client.Close();
                if (MAC != null)
                {
                    MessageBox.Show("Идёт включение...");
                }
                else
                {
                    MessageBox.Show("Ошибка: МАК-адресс компьютера " + textBox1.Text + " отсутствует в базе.");
                }

            }
            catch (Exception err)
            {
                MessageBox.Show("Ошибка: МАК-адресс компьютера "+textBox1.Text+" отсутствует в базе.");
            }
        }

            

        private void label1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
