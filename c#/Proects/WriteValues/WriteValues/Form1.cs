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

namespace WriteValues
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string MasterIP = "localhost";
            string SlaveIP = "localhost";
            string OPCServer = "Infinity.OPCServer";
            ArrayList tags = new ArrayList();//Массив с тегами
            ArrayList values = new ArrayList();//Массив со значениями
            string strtags = "";
            string strvalues = "";


            Opc.URL url = new Opc.URL("opcda://" + MasterIP + "/" + OPCServer);
            Opc.Da.Server server = new Opc.Da.Server(new OpcCom.Factory(), url);


            richTextBox1.Clear();
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
                return;
            StreamReader streamReader = System.IO.File.OpenText(openFileDialog1.FileName);
            //Вспомогательная переменная - строка
            string tmp = "";
            //Вспомогательная переменная для индекса
            int ind = 0;
            
            //Пишем, пока в файле есть строки
            while (!streamReader.EndOfStream)
            {
                tmp = streamReader.ReadLine();
                //Вырезаем числовое значение из строки
                ind = tmp.IndexOf(";");
                strtags = tmp.Substring(0,ind);
                strvalues = tmp.Substring(ind + 1);
                //Помещаем в массив
                tags.Add(strtags);
                values.Add(strvalues);
              
            }


            streamReader.Close();

            //Соединяемся с сервером
            try
            {
                server.Connect();
                label1.Text = "Связь по осн каналу";
            }
            catch
            {
                url = new Opc.URL("opcda://" + SlaveIP + "/" + OPCServer);
                server = new Opc.Da.Server(new OpcCom.Factory(), url);
                try
                {
                    server.Connect();
                    label1.Text = "Связь по рез каналу";
                }
                catch
                {
                }
            }
            System.Threading.Thread.Sleep(1500); // время необходимое для подключения к источнику данных; ОБЯЗАТЕЛЬНЫЙ ПАРАМЕТР! Параметр в скобках задается в \\миллисекундах;
            
            //Формируем массив для записи
            Opc.Da.ItemValue[] TMP = new Opc.Da.ItemValue[tags.Count];
            for (int i = 0; i < tags.Count; i++)
            {
                //Указываем теги     
                TMP[i] = new Opc.Da.ItemValue(new Opc.ItemIdentifier(System.Convert.ToString(tags[i])));
                //Устанавливаем значения     
                TMP[i].Value = values[i];
                //Устанавливаем хорошее качество     
                TMP[i].Quality = Opc.Da.Quality.Good;
                
            }
            //Записываем в сервер значения
            server.Write(TMP);


            for (int i = 0; i < tags.Count; i++)
            {
                richTextBox1.Text = richTextBox1.Text + tags[i] + "-"  + values[i] + "\n";
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
