using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace project
{
   
    class Program
    {
        
        static void Main(string[] args)
        {
            
            IWebDriver driver = new ChromeDriver();
            driver.Url = @"http://nalog.ru";
            driver.FindElement(By.Id("tb_main_search_f")).SendKeys("личный кабинет");
            driver.FindElement(By.Id("bt_main_search_f")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//a[@href='https://lkfl2.nalog.ru/lkfl/']")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//a[@href='/lkfl-demo']")).Click();
            Thread.Sleep(1000);
            var summa=driver.FindElement(By.ClassName("main-page_title_sum")).GetAttribute("textContent");
            var summa1=(summa.Replace(" ", "")).Replace(".", ",");
            var image=driver.FindElement(By.ClassName("src-modules-App-components-UserInfo-UserInfo-module__photo"));
            int image_width=image.Size.Width;
            int image_height=image.Size.Height;
            if (image_width==31 && image_height==31)
            {
                Console.WriteLine("Image - PASS");
            }
            else
            {
                Console.WriteLine("Image - FAIL!!!");
            }

            //Console.WriteLine(summa1);

            if (float.Parse(summa1) < 200000)
            {    
                Console.WriteLine("Summa - PASS");
            }
            else
            {
                Console.WriteLine("Summa - FAIL!!!");
            }
            driver.Quit();

        }
    }
}
