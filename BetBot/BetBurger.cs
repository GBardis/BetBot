using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Windows;


namespace BetBot
{
    class BetBurger
    {
        public static IWebDriver burgerMidas = new FirefoxDriver();
        //public static EventFiringWebDriver burgerMidas = new EventFiringWebDriver(bMidas);
        List<dynamic> responses = new List<dynamic>();
        //List<string> fileWriteResponses = new List<string>();
        List<IWebElement> jsonArbs = new List<IWebElement>();
        List<IWebElement> jsonArbsTemp = new List<IWebElement>();
        public static ObservableCollection<BetList> betList = new ObservableCollection<BetList>();
        BetList simpleBet = new BetList();
        JToken j;
        string url;
        dynamic response;
        private Dictionary<int, string[]> bets = new Dictionary<int, string[]>();
        private IWebElement okCookieClick, prematchLinkClick;
        List<IWebElement> divisions = new List<IWebElement>();
        List<IWebElement> betCompanies = new List<IWebElement>();
        List<int> betCompanyIndexes = new List<int>();
        List<string> betCompanyDivisions = new List<string>();
        List<string> divisionsList = new List<string>();

        public void ScrapBurger()
        {
            burgerMidas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            burgerMidas.Navigate().GoToUrl("https://www.betburger.com/gr/");
            okCookieClick = burgerMidas.FindElement(By.XPath("html/body/div[1]/div/a[1]"));
            System.Threading.Thread.Sleep(1000);
            okCookieClick.Click();
            prematchLinkClick = burgerMidas.FindElement(By.XPath("html/body/header/div/nav/ul/li[2]/a"));
            prematchLinkClick.Click();
            //LOGIN!
            //prematchLinkClick = burgerMidas.FindElement(By.XPath("html/body/header/div/nav/ul/li[6]/a"));
            //prematchLinkClick.Click();
            //prematchLinkClick = burgerMidas.FindElement(By.XPath("html/body/div[1]/div[2]/div/div[1]/div/div/div/form/div[1]/input"));
            //prematchLinkClick.SendKeys("con.kokkinis@gmail.com");
            //prematchLinkClick = burgerMidas.FindElement(By.XPath("html/body/div[1]/div[2]/div/div[1]/div/div/div/form/div[2]/input"));
            //prematchLinkClick.SendKeys("6983868418");
            //prematchLinkClick = burgerMidas.FindElement(By.XPath("html/body/div[1]/div[2]/div/div[1]/div/div/div/form/div[4]/button"));
            //prematchLinkClick.Click();
            //prematchLinkClick = burgerMidas.FindElement(By.XPath("html/body/header/div/nav/ul/li[2]/a"));
            //prematchLinkClick.Click();
        }

        public void DummyClick()
        {
            prematchLinkClick = burgerMidas.FindElement(By.XPath("html/body/nav/div/div[2]/ul/li[2]/a"));
            prematchLinkClick.Click();
        }

        public ObservableCollection<BetList> GetArbsToJson()
        {
            betList.Clear();
            responses.Clear();
            jsonArbs = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div[1]/div/div[2]/div/div/div/div/div[4]/div/div/a[3]")).ToList<IWebElement>();

            divisionsList.Clear();
            divisionsList = FindBetDivision();
            List<string[]> words = new List<string[]>();
            int jj = 0;
            foreach (IWebElement jsonArb in jsonArbs)
            {
                url = jsonArb.GetAttribute("href");
                url = Uri.UnescapeDataString(url);
                url = url.Substring(url.IndexOf('#') + 1);
                j = JToken.Parse(url);
                response = JsonConvert.DeserializeObject<dynamic>(j.ToString());

                words.Clear();
                for (int i = 0; i < response.bets.Count; i++)
                {
                    if (response.bets[i].bookmaker_id == "10")
                    {
                        responses.Add(response.bets[i]);
                        //fileWriteResponses.Add(response.bets[i].home.ToString());
                        simpleBet.arbId = response.arb.id.ToString();
                        // simpleBet.league = "Sweden.Sweden 1.div Norra";
                        simpleBet.league = divisionsList[jj];
                        words.Add(divisionsList[jj].Split('.'));

                        if (words.First().ElementAt(0) == "Sweden")
                        {
                            words.Clear();
                            string str = divisionsList[jj];
                            words.Add(str.Split(new[] { "." }, 2, StringSplitOptions.None));

                        }
                        else
                        {
                            words.Add(divisionsList[jj].Split('.'));
                        }


                        simpleBet.parentDiv = words.First().ElementAt(0);
                        if ((words.First().ElementAt(1) == words.First().ElementAt(0)))
                        {
                            simpleBet.childDiv = words.First().ElementAt(2);
                            jj++;
                        }
                        else
                        {
                            simpleBet.childDiv = words.First().ElementAt(1).Substring(1, words.First().ElementAt(1).Length - 1);
                            jj++;
                        }
                        simpleBet.sportId = response.arb.sport_id.ToString();
                        simpleBet.sportName = response.arb.sport.name.ToString();
                        simpleBet.countryId = response.arb.country_id;
                        simpleBet.betId = response.bets[i].id.ToString();
                        simpleBet.koef = response.bets[i].koef.ToString();
                        simpleBet.home = response.bets[i].home.ToString();
                        simpleBet.away = response.bets[i].away.ToString();
                        simpleBet.eventName = simpleBet.home + " v " + simpleBet.away;
                        simpleBet.betType = response.bets[i].bet_combination.title.ToString();
                        simpleBet.bookmakerId = response.bets[i].bookmaker_id;

                        betList.Add(new BetList(simpleBet.arbId, simpleBet.eventName, simpleBet.league, simpleBet.countryId, simpleBet.betId, simpleBet.bookmakerId, simpleBet.parentDiv, simpleBet.childDiv, simpleBet.sportId, simpleBet.sportName, simpleBet.home, simpleBet.away, simpleBet.koef, simpleBet.betType));

                    }
                }
            }
            jsonArbsTemp = jsonArbs;
            //File.WriteAllLines("fuck.txt",betList);
            return betList;
        }

        public List<string> FindBetDivision()
        {
            betCompanies = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div/div/div[1]/div/div/div/div/div/a")).ToList<IWebElement>();
            divisions = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div/div/div[3]/div/div/div/div/div[1]/small")).ToList<IWebElement>();

            for (int i = 0; i < betCompanies.Count; i++)
            {
                if (betCompanies[i].Text == "Bet365")
                {
                    try
                    {
                        betCompanyDivisions.Add(divisions[i].Text);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("ERROR");
                    }
                }
            }
            return betCompanyDivisions;
        }
    }
}
