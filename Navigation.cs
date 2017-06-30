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
    class Navigation
    {

        List<IWebElement> elements = new List<IWebElement>();
        public void closeAllOpenDivs()
        {
            elements = MainWindow.midas.FindElements(By.CssSelector(".sm-Market_HeaderOpen")).ToList<IWebElement>();
            foreach (IWebElement element in elements)
            {
                try
                {
                    element.Click();
                }
                catch (InvalidOperationException)
                {
                    //element.Click();

                }
            }
        }
    }
}
