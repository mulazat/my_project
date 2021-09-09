using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
namespace readXML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private DataTable CreateTable()
        {
            //создаём таблицу
            DataTable dt = new DataTable("Friends");

            //создаём три колонки
            DataColumn colID = new DataColumn("Id", typeof(String));
            DataColumn colName = new DataColumn("Name", typeof(String));
            DataColumn colAge = new DataColumn("Property", typeof(String));

            //добавляем колонки в таблицу
            dt.Columns.Add(colID);
            dt.Columns.Add(colName);
            dt.Columns.Add(colAge);

            return dt;
        }

        private DataTable ReadXml()
        {
            DataTable dt = null;

            try
            {
                //загружаем xml файл
                XDocument xDoc = XDocument.Load(@"../../XMLFile1.xml");

                //создаём таблицу
                dt = CreateTable();

                DataRow newRow = null;

                //получаем все узлы в xml файле
                foreach (XElement elm in xDoc.Descendants("Item"))
                //foreach (XElement elm2 in xDoc.Descendants("Property"))
                {
                    //создаём новую запись
                    newRow = dt.NewRow();

                    //проверяем наличие атрибутов (если требуется)
                    if (elm.HasAttributes)
                    {
                        //проверяем наличие атрибута id
                        if (elm.Attribute("Id") != null)
                        {
                            //получаем значение атрибута
                            newRow["id"] = elm.Attribute("Id").Value;
                        }

                        if (elm.Attribute("Name") != null)
                        {
                            //получаем значение атрибута
                            newRow["Name"] = elm.Attribute("Name").Value;
                        }

                        if (elm.Element("Property") != null)
                        {
                            newRow["Property"] = elm.Element("Id").Value;
                        }

                        //if (elm.Element("Properties") != null)
                        //{
                        //    foreach (XElement elm2 in elm.Descendants("Property"))
                        //   {
                        //       if (elm2.HasAttributes)
                        //       {
                        //проверяем наличие атрибута id
                        //          if (elm2.Attribute("Value") != null)
                        //          {
                        //получаем значение атрибута
                        //             newRow["Property"] = elm2.Attribute("Value").Value;
                        //        }
                        //        else
                        //        {
                        //           newRow["Property"] = "11";
                        //      }
                        //   }
                        // }
                        // }

                    }



                    //проверяем наличие xml элемента name
                    // if (elm.Element("Properties") != null)
                    //  {
                    //     foreach (XElement elm2 in xDoc.Descendants("Property"))
                    //получаем значения элемента name
                    //        newRow["Property"] = elm2.Attribute("Value").Value;

                    // }

                    //if (elm.Element("age") != null)
                    //{
                    //  newRow["age"] = int.Parse(elm.Element("age").Value);
                    //}

                    //добавляем новую запись в таблицу
                    dt.Rows.Add(newRow);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ReadXml();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            String name = "";
            String id = "";
            String subname = "";

            String subid = "";
            String subid101 = "";
            String subid5000 = "";
            String value = "";
            String value101="";
            String value5000 = "";
            try
            {
                //загружаем xml файл
                XDocument xdoc = XDocument.Load(@"../../XMLFile1.xml");
               var Items = xdoc.Descendants("Item");
            foreach (var Item in Items)
            {
                name = Item.Attribute("Name").Value;
                id = Item.Attribute("Id").Value;
                var Properties = Item.Element("Properties").Elements();
                //categories.Add("<li><a class='rubr' href='"+url+"'>"+name+"</a></li>");
                //richTextBox1.Text = richTextBox1.Text + name + id + "\n";
                foreach (var Property in Properties)
                {
                                        
                    subid = Property.Attribute("Id").Value;
                    //value = Property.Attribute("Value").Value;
                    
                    
                        if (subid.Contains("101"))
                        {
                            value101 = Property.Attribute("Value").Value;
                            subid101 = Property.Attribute("Id").Value;
                        }
                        
                        if (subid.Contains("5000"))
                        {
                            value5000 = Property.Attribute("Value").Value;
                            subid5000= Property.Attribute("Id").Value;
                        }
                        subname = name + "." + name;

                    //categories.Add("<li><a class='podkat' href='" + subUrl + "'>" + subName + "</a></li>");
                    //richTextBox1.Text = richTextBox1.Text + subid + value +  "\n";
                }
                richTextBox1.Text = richTextBox1.Text + name + ";"+subname+";"+ id + ";" + subid101 + ";" + value101 + ";" +  subid5000 + ";"+value5000 + "\n";
            }
            //richTextBox1.Text = richTextBox1.Text + name + id + subid+value+"\n";
                /*foreach (XElement phoneElement in xdoc.Descendants("Item"))
                {
                    XAttribute nameAttribute = phoneElement.Attribute("Name");
                    XAttribute IdAttribute = phoneElement.Attribute("Id");
                    XElement companyElement = phoneElement.Element("Property");
                    //XElement priceElement = phoneElement.Element("price");

                    if (nameAttribute != null )
                    {
                        Name = nameAttribute.Value;
                        Id = IdAttribute.Value;
                        ValueProp = companyElement.Attribute("Value").Value;
                    }

                    richTextBox1.Text = richTextBox1.Text + Id + Name + ValueProp + "\n";
                */

                    /*
                     //получаем все узлы в xml файле
                     foreach (XElement elm in xDoc.Descendants("Item"))
                     //foreach (XElement elm2 in xDoc.Descendants("Property"))
                     {
                   

                         //проверяем наличие атрибутов (если требуется)
                         if (elm.HasAttributes)
                         {
                             //проверяем наличие атрибута id
                             if (elm.Attribute("Id") != null)
                             {
                                 //получаем значение атрибута
                                Id = elm.Attribute("Id").Value;
                             }

                             if (elm.Attribute("Name") != null)
                             {
                                 //получаем значение атрибута
                                 Name = elm.Attribute("Name").Value;
                             }

                             if (elm.Element("Properties") != null)
                             {

                                  var elm2 = elm.Descendants("Properties");
                            
                                 if (elm2.Single() != null)
                                 {
                                     ValueProp = elm2.Single().ToString(); 
                                 }
                             }
                     */

                    // if (elm.Attribute("Property") != null)
                    // {

                    //  ValueProp = elm.Attribute("Value").Value;





                    //XElement elm2 = xDoc.Descendants("Property").SingleOrDefault();


                    //if (elm2.Element("Property") != null)
                    // {

                    //получаем значение атрибута
                    //ValueProp = elm2.Attribute("Value").Value;
                    // ValueProp = (elm.Descendants("Property").SingleOrDefault()).Value;
                    // }
                    // }

                    /*if (elm.Element("Properties") != null)
                    {
                        foreach (XElement elm2 in elm.Descendants("Property"))
                       {
                           if (elm2.HasAttributes)
                           {
                     //проверяем наличие атрибута id
                              if (elm2.Attribute("Value") != null)
                              {
                    //получаем значение атрибута
                                 ValueProp = elm2.Attribute("Value").Value;
                            }
                            else
                            {
                                ValueProp = "1";
                          }
                       }
                     }
                     }*/





                    //проверяем наличие xml элемента name
                    // if (elm.Element("Properties") != null)
                    //  {
                    //     foreach (XElement elm2 in xDoc.Descendants("Property"))
                    //получаем значения элемента name
                    //        newRow["Property"] = elm2.Attribute("Value").Value;

                    // }

                    //if (elm.Element("age") != null)
                    //{
                    //  newRow["age"] = int.Parse(elm.Element("age").Value);
                    //}

                    //добавляем новую запись в таблицу
                    //richTextBox1.Text = richTextBox1.Text + Id + Name + ValueProp+"\n";


                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

    }
}


