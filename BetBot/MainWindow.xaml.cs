using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support;
using System.IO;
using OpenQA.Selenium.Support.Events;

namespace BetBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IWebDriver midas = new FirefoxDriver();
        //public static EventFiringWebDriver firingMidas = new EventFiringWebDriver(midas);
        Dictionary<string, string> dict = new Dictionary<string, string>();
        Navigation nav = new Navigation();
        private readonly IEnumerable<string[]> dictionary;
        BetBurger burger = new BetBurger();
        List<BetList> betList = new List<BetList>();
        //placedBets functionality

        public MainWindow()
        {
            InitializeComponent();
            //driver.Manage().Window.Maximize();
            midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            midas.Navigate().GoToUrl("https://www.bet365.gr/en/");
            IWebElement element = midas.FindElement(By.Id("TopPromotionButton"));
            element.Click();
            dictionary = File.ReadLines(@"Football.csv").Select(line => line.Split(','));
            foreach (string[] e in dictionary)
            {
                dict.Add(e[0].ToString(), e[1].ToString());
            }
            //BetBurger.burgerMidas.ScriptExecuted += new EventHandler<WebDriverScriptEventArgs>(firingDriver_ScriptExecuted);
            //BetBurger.burgerMidas.ElementClicked += new EventHandler<WebElementEventArgs>(firingDriver_ButtonClicked);
        }
        //private void firingDriver_ScriptExecuted(object sender, WebDriverScriptEventArgs e)
        //{
        //    // do action required to handle what happens after clicking button you have mentioned.
        //    errorLabel.Content = "BOOM";
        //}
        //private void firingDriver_ButtonClicked(object sender, WebElementEventArgs e)
        //{
        //    // do action required to handle what happens after clicking button you have mentioned.
        //    errorLabel.Content = "Zoom";
        //}
        private void Navigate_Click(object sender, RoutedEventArgs e)
        {

            burger.ScrapBurger();
            divNav leftNav = new divNav();
            List<string> betcat = new List<string>();

            //betcat = betcompanydivisions;
            betList = burger.GetArbsToJson();

            foreach (var bet in betList)
            {
                // navigate to betcategories
                clickResponse(bet.sportName, "html/body/div[1]/div/div[2]/div[1]/div/div[1]/div/div/div", leftNav);
                // close all divs inside a category 
                nav.closeAllOpenDivs();
                // Find Country
                clickResponse("Ην. Βασίλειο", "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div", leftNav);
                // Find Division 
                clickResponse("Αγγλία - Πρέμιερ Λιγκ", "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div[3]/div[2]/div", leftNav);
                // Select Match
                clickResponse(bet.eventName, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div[1]/div[2]/div/div[1]/div/div/div", leftNav);

            }
        }

        private void clickResponse(string clickName, string path, divNav leftNav)
        {
            if (!leftNav.fetchLeftNav(clickName, path))
            {
                //errorLabel.Content = clickName + " Not Found!";
            }
            else
            {
                // errorLabel.Content = clickName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            betList = burger.GetArbsToJson();
            BetBurger.betList.Clear();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            burger.DummyClick();
        }
    }
}
