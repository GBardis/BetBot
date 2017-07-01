using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using System.Collections;

namespace BetBot
{
    class BetBurger
    {
        IWebDriver burgerMidas = new FirefoxDriver();
        private List<IWebElement> arbs = new List<IWebElement>();
        private List<IWebElement> divisions = new List<IWebElement>();
        private List<IWebElement> games = new List<IWebElement>();
        private List<IWebElement> categories = new List<IWebElement>();
        private List<IWebElement> tree = new List<IWebElement>();
        private List<IWebElement> betcompanies;
        private Dictionary<int, string[]> bets = new Dictionary<int, string[]>();
        private IWebElement arb;
        private IWebElement prematch;
        public void scrapBurger()
        {
            burgerMidas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            burgerMidas.Navigate().GoToUrl("https://www.betburger.com/gr/");
            arb = burgerMidas.FindElement(By.XPath("html/body/div[1]/div/a[1]"));
            System.Threading.Thread.Sleep(1000);
            arb.Click();
            prematch = burgerMidas.FindElement(By.XPath("html/body/header/div/nav/ul/li[2]/a"));
            prematch.Click();
            getdata();
        }
        public void getdata()
        {
            games = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div/div/div[3]/div/div/div/div/div[1]/div/a")).ToList<IWebElement>();
            divisions = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div/div/div[3]/div/div/div/div/div[1]/small")).ToList<IWebElement>();
            arbs = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div/div/div[4]/div/div/a")).ToList<IWebElement>();
            categories = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div[2]/div/div/*")).ToList<IWebElement>();
            betcompanies = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div[2]/div/div[1]/div/div/div/div/div/a")).ToList<IWebElement>();

            int i = 0;
            foreach (IWebElement betcompany in betcompanies)
            {
                foreach (IWebElement game in games)
                {
                    foreach (IWebElement division in divisions)
                    {
                        foreach (IWebElement arbs in arbs)
                        {
                            // if (betcompany.Text == "Bet365" || betcompany.Text == "Pinnacle"))
                            //   {
                            bets.Add(i, new[] { betcompany.Text.ToString(), game.Text.ToString(), division.Text.ToString(), arbs.Text.ToString() });
                            i++;
                            //  }
                            break;
                        }
                        break;
                    }
                    break;
                }
            }
        }
    }
}
