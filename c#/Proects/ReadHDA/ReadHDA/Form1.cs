using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Opc;
using Opc.Da;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;


namespace ReadHDA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string MasterIP = "localhost";//Расположение основного сервера истории
            string SlaveIP = "localhost";//Расположение дублирующего сервера истории
            string OPCServer = "OPCAEDualSource.OPCAEServer.1";//Идентификатор сервера
DateTime startDate = new DateTime(2018, 2, 9, 10, 0, 0);//Начало интервала выборки
DateTime endDate = new DateTime(2018, 3, 9, 15, 0, 0);//Конец интервала выборки
DateTime curDate = startDate;//Переменная для запоминания текущей метки времени выборки
DataSet result = new DataSet();
DataTable table = new DataTable("ScriptData");
//Объявление таблицы для хранения данных
table.Columns.Add("Teg", typeof(System.String));
table.Columns.Add("Value", typeof(System.Double));
table.Columns.Add("TimeStamp", typeof(System.String));
table.Columns.Add("Quality", typeof(System.String));
result.Tables.Add(table);

Opc.URL url = new Opc.URL("opcae://" +MasterIP + "/" + OPCServer);
Opc.Ae.Server server = new Opc.Ae.Server(new OpcCom.Factory(), url);
//Объявляем строковый массив с тегами
string[] TEMP = new string[2];
TEMP[0] = "AK.USMN.R_Nurl.LU_BerKro.KP_10.P1";
TEMP[1] = "AK.USMN.R_Nurl.LU_BerKro.KP_10.P2";

//Соединение с сервером
try
{
    server.Connect();
    label1.Text="Канал 1";
}
catch
{
    url = new Opc.URL("opcae://" + SlaveIP + "/" + OPCServer);
    server = new Opc.Ae.Server(new OpcCom.Factory(), url);
    try
    {
        server.Connect();
        label1.Text = "Канал 2";
    }
    catch (System.Exception ex)
    {
        MessageBox.Show(ex.Message);
    } 
    //label1.Text = "Error"; }
}

            System.Threading.Thread.Sleep (1500); // время необходимое для подключения к \\источнику данных; ОБЯЗАТЕЛЬНЫЙ ПАРАМЕТР! Параметр в скобках задается в \\миллисекундах;
            string[] identifiers = new string[] { "" };//Задаем массив тегов




/*
            
            Opc.ItemIdentifier[] identifiers = new Opc.ItemIdentifier[TEMP.Length];

            for (int i = 0; i < TEMP.Length; i++)
{
    identifiers[i] = new Opc.ItemIdentifier(TEMP[i]);
}
Opc.IdentifiedResult[] items = server.CreateItems(identifiers);
Opc.Hda.Time startTime = new Opc.Hda.Time(startDate);
Opc.Hda.Time endTime = new Opc.Hda.Time(endDate);
int maxValues = 100;//Эта переменная нужна для того, чтобы читать не все записи сразу за заданный интервал, а частями
int index = 0;
//Начинаем читать данные из истории порциями, пока не встретился конец интервала
while(curDate < endDate)
{
Opc.Hda.Time curTime = new Opc.Hda.Time(curDate);
Opc.Hda.ItemValueCollection[] values = server.ReadRaw(curTime, endTime, maxValues, true, items);
for (int i = 0; i < values.Length; i++)
{
    for (int j = 0 + index; j < values[i].Count + index; j++)
    {
        DataRow row = table.NewRow();
        table.Rows.Add(row);
        curDate = System.Convert.ToDateTime(values[i][j - index].Timestamp);
        row[0] = values[i].ItemName;
        row[1] = values[i][j - index] == null ? System.Convert.ToDouble(DBNull.Value) : System.Convert.ToDouble(values[i][j - index].Value);
        row[2] = System.Convert.ToString(values[i][j - index].Timestamp);
        row[3] = values[i][j - index].Quality.GetCode();
        richTextBox1.Text = richTextBox1.Text + row[0]+row[1]+ "\n";   
    }


}
    index+=maxValues;
}
try
{
    server.Disconnect();
}
catch
{
}
*/
        }
    }
}
