using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Newtonsoft.Json;
using Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Support.Events;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BetBot
{
    class BetBurger
    {
        public static IWebDriver burgerMidas = new FirefoxDriver();
        //public static EventFiringWebDriver burgerMidas = new EventFiringWebDriver(bMidas);
        List<dynamic> responses = new List<dynamic>();
        //List<string> fileWriteResponses = new List<string>();
        List<IWebElement> jsonArbs = new List<IWebElement>();
        public static List<BetList> betList = new List<BetList>();
        BetList simpleBet = new BetList();
        JToken j;
        string url;
        dynamic response;
        private Dictionary<int, string[]> bets = new Dictionary<int, string[]>();
        private IWebElement okCookieClick, prematchLinkClick;

        public void ScrapBurger()
        {
            burgerMidas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            burgerMidas.Navigate().GoToUrl("https://www.betburger.com/gr/");
            okCookieClick = burgerMidas.FindElement(By.XPath("html/body/div[1]/div/a[1]"));
            System.Threading.Thread.Sleep(1000);
            okCookieClick.Click();
            prematchLinkClick = burgerMidas.FindElement(By.XPath("html/body/header/div/nav/ul/li[2]/a"));
            prematchLinkClick.Click();
        }

        public void DummyClick()
        {
            prematchLinkClick = burgerMidas.FindElement(By.XPath("html/body/nav/div/div[2]/ul/li[2]/a"));
            prematchLinkClick.Click();
        }

        public List<BetList> GetArbsToJson()
        {
            jsonArbs = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div[1]/div/div[2]/div/div/div/div/div[4]/div/div/a[3]")).ToList<IWebElement>();
            List<string> divisions = FindBetDivision();
            string[] words = new string[2];
            foreach (IWebElement jsonArb in jsonArbs)
            {
                url = jsonArb.GetAttribute("href");
                url = Uri.UnescapeDataString(url);
                url = url.Substring(url.IndexOf('#') + 1);
                j = JToken.Parse(url);
                response = JsonConvert.DeserializeObject<dynamic>(j.ToString());

                for (int i = 0; i < response.bets.Count; i++)
                {
                    if (response.bets[i].bookmaker_id == "10")
                    {
                        responses.Add(response.bets[i]);
                        //fileWriteResponses.Add(response.bets[i].home.ToString());
                        simpleBet.arbId = response.arb.id.ToString();
                        simpleBet.eventName = response.arb.event_name.ToString().Replace("-", "v");

                        foreach (string division in divisions)
                        {
                            simpleBet.league = division;
                            words = simpleBet.league.Split('.');
                            simpleBet.parentDiv = words[0];
                            simpleBet.childDiv = words[1];
                            break;
                        }
                        simpleBet.sportId = response.arb.sport_id.ToString();
                        simpleBet.sportName = response.arb.sport.name.ToString();
                        simpleBet.betId = response.bets[i].id.ToString();
                        simpleBet.koef = response.bets[i].koef.ToString();
                        simpleBet.home = response.bets[i].home.ToString();
                        simpleBet.away = response.bets[i].away.ToString();
                        simpleBet.betType = response.bets[i].bet_combination.title.ToString();
                        simpleBet.bookmakerId = response.bets[i].bookmaker_id;
                        simpleBet.countryId = response.arb.country_id;
                        betList.Add(simpleBet);
                    }
                }
            }
            //File.WriteAllLines("fuck.txt",betList);
            return betList;
        }
        public List<string> FindBetDivision()
        {
            string title;
            int index;
            List<IWebElement> divisions = new List<IWebElement>();
            List<IWebElement> betcompanies = new List<IWebElement>();
            List<int> betcompanyindexes = new List<int>();
            List<string> betcompanydivisions = new List<string>();

            betcompanies = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div/div/div[1]/div/div/div/div/div/a")).ToList<IWebElement>();
            divisions = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div/div/div[3]/div/div/div/div/div[1]/small")).ToList<IWebElement>();

            foreach (IWebElement betcompany in betcompanies)
            {
                title = betcompany.GetAttribute("title");
                if (title == "Bet365")
                {
                    index = betcompanies.IndexOf(betcompany) + 1;
                    betcompanyindexes.Add(index);
                }
                else
                {
                    betcompanyindexes.Add(0);
                }
            }
            int temp = 0;
            foreach (IWebElement division in divisions)
            {
                index = divisions.IndexOf(division) + 1;
                for (int i = temp; i <= betcompanyindexes.Count; i++)
                {
                    if (betcompanyindexes[i] == index)
                    {
                        betcompanydivisions.Add(division.Text);
                        temp = temp + 1;
                        break;
                    }
                    else
                    {
                        temp = temp + 1;
                        // betcompanydivisions.Add("");
                        break;
                    }

                }
                //  var newlist = betcompanydivisions.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();            
            }
            return betcompanydivisions;
        }
    }
}
