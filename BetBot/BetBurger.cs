﻿using System;
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
using System.Collections.ObjectModel;

namespace BetBot
{
    class BetBurger
    {
        public static IWebDriver burgerMidas = new FirefoxDriver();
        //public static EventFiringWebDriver burgerMidas = new EventFiringWebDriver(bMidas);
        List<dynamic> responses = new List<dynamic>();
        //List<string> fileWriteResponses = new List<string>();
        List<IWebElement> jsonArbs = new List<IWebElement>();
        public static ObservableCollection<BetList> betList = new ObservableCollection<BetList>();
        BetList simpleBet = new BetList();
        JToken j;        
        string url;
        string[] words;
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

        public ObservableCollection<BetList> GetArbsToJson()
        {
            jsonArbs = burgerMidas.FindElements(By.XPath("html/body/div[5]/div[2]/div/div[3]/div/div/div[1]/div/div/div/div/div/div/div/div[1]/ul/li/div/div[1]/div/div[2]/div/div/div/div/div[4]/div/div/a[3]")).ToList<IWebElement>();
            foreach (IWebElement jsonArb in jsonArbs)
            {
                url = jsonArb.GetAttribute("href");
                url = Uri.UnescapeDataString(url);
                url = url.Substring(url.IndexOf('#') + 1);
                j = JToken.Parse(url);
                response = JsonConvert.DeserializeObject<dynamic>(j.ToString());
                try
                {
                    for (int i = 0; i < response.bets.Count; i++)
                    {
                        //if (response.bets[i].bookmaker_id == "10")
                        //{
                            responses.Add(response.bets[i]);
                            //fileWriteResponses.Add(response.bets[i].home.ToString());
                            simpleBet.arbId = response.arb.id.ToString();
                            simpleBet.eventName = response.arb.event_name.ToString();                            
                            simpleBet.league = response.arb.league.ToString();
                            words = simpleBet.league.Split('.');
                            simpleBet.parentDiv = words[0];
                            simpleBet.childDiv = words[1];
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
                        //}
                    }
                }
                catch (Exception ex)
                {

                }
            }
            //File.WriteAllLines("fuck.txt",betList);
            return betList;            
        }
    }
}
