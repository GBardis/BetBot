using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support;

namespace BetBot
{
    class BetBurger
    {
        IWebDriver burgerMidas = new FirefoxDriver();
        List<IWebElement> arbs = new List<IWebElement>();
        IWebElement arb;

        public void scrapBurger()
        {
            burgerMidas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            burgerMidas.Navigate().GoToUrl("https://www.betburger.com/arbs");
            arb = burgerMidas.FindElement(By.XPath("html/body/div[1]/div/a[1]"));
            System.Threading.Thread.Sleep(1000);
            arb.Click();
            burgerMidas.Navigate().Refresh();
            //arbs = burgerMidas.FindElements(By.TagName("a")).ToList<IWebElement>();
            arbs = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div/div/div[3]/div/div/div/div/div[1]/div/a")).ToList<IWebElement>();
        }
    }
}
