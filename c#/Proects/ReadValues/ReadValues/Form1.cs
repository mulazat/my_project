using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Collections;
using Opc;
using OpcCom;
using OpcEnumLib;
//using OPCAutomation;


namespace ReadValues
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static ArrayList readVal(ArrayList arrtags, Opc.Da.Server server)
        {

            ArrayList value = new ArrayList();
            Opc.Da.Item[] TMP = new Opc.Da.Item[arrtags.Count];
            for (int i = 0; i < arrtags.Count; i++)
            {
                TMP[i] = new Opc.Da.Item(new Opc.ItemIdentifier(arrtags[i].ToString()));
            }
            Opc.Da.ItemValueResult[] vTMP = server.Read(TMP);
            for (int i = 0; i < vTMP.Length; i++)
            {
                    value.Add(vTMP[i].Value);
            }
            return value;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string MasterIP = "localhost";
            string SlaveIP = "localhost";
            string OPCServer = "Infinity.OPCServer";
            //Создаем массив для записи
            ArrayList arrTagVal = new ArrayList();
            ArrayList arrTagDesc = new ArrayList();
            
            Opc.URL url = new Opc.URL("opcda://" + MasterIP + "/" + OPCServer);
            Opc.Da.Server server = new Opc.Da.Server(new OpcCom.Factory(), url);
           

            
            //Соединяемся с сервером
            try
            {
                server.Connect();
                label4.Text = "Связь по осн каналу";
            }
            catch
            {
                url = new Opc.URL("opcda://" + SlaveIP + "/" + OPCServer);
                server = new Opc.Da.Server(new OpcCom.Factory(), url);
                try
                {
                    server.Connect();
                    label4.Text = "Связь по рез каналу";
                }
                catch
                {
                }
            }
            System.Threading.Thread.Sleep(1500); // время необходимое для подключения к источнику данных; ОБЯЗАТЕЛЬНЫЙ ПАРАМЕТР! Параметр в скобках задается в \\миллисекундах;


            richTextBox1.Clear();
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
                return;
            StreamReader streamReader = System.IO.File.OpenText(openFileDialog1.FileName);
            string tmp = "";
            string TagVal = "";
            string TagDesc = "";
            //Пишем, пока в файле есть строки
            while (tmp != null)
            {
                tmp = streamReader.ReadLine();
                if (tmp != null)

                    TagVal = tmp;
                    arrTagVal.Add(TagVal+textBox1.Text);
               
                    TagDesc = tmp + ".description";
                    arrTagDesc.Add(TagDesc);
            }

            streamReader.Close();

            ArrayList value = readVal(arrTagVal, server);
            ArrayList description = readVal(arrTagDesc, server);
           
            for (int i = 0; i < value.Count; i++)
            {
                richTextBox1.Text = richTextBox1.Text +  arrTagVal[i] +";" + description[i] +";" +value[i] +  "\n";
                Application.DoEvents();

            }

            string lines = richTextBox1.Text;
            System.IO.File.WriteAllText(openFileDialog1.FileName + ".csv", lines);
           

                MessageBox.Show("Готово!");
                try
                {
                    server.Disconnect();
                }
                catch
                {
                }
            }


        }
    }
