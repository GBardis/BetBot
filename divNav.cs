using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;


namespace BetBot
{
    class divNav : Navigation
    {
        List<IWebElement> elements = new List<IWebElement>();
        WebDriverWait wait = new WebDriverWait(MainWindow.midas, TimeSpan.FromSeconds(10));

        public bool fetchLeftNav(string clickName,string path)
        {
            //elements = MainWindow.driver.FindElements(By.XPath(""));
            //string path = "html/body/div[1]/div/div[2]/div[1]/div/div[1]/div/div/div";
            elements = MainWindow.midas.FindElements(By.XPath(path)).ToList<IWebElement>();
            if (elements.Count > 0)
            {
                foreach (IWebElement element in elements)
                {
                    if (element.Text == clickName)
                    {
                        element.Click();
                        break;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
