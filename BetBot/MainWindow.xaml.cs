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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

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
        ObservableCollection<BetList> betList = new ObservableCollection<BetList>();

        divNav leftNav = new divNav();
        private Stopwatch watch;

        //placedBets functionality

        public MainWindow()
        {
            //FirefoxProfile p = new FirefoxProfile();
            //p.SetPreference("javascript.enabled", false);

            InitializeComponent();
            this.DataContext = this;
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

            //BetBurger.betList.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedEvent);
            // betList.Add(new BetList());
            //BetBurger.burgerMidas.ScriptExecuted += new EventHandler<WebDriverScriptEventArgs>(firingDriver_ScriptExecuted);
            //BetBurger.burgerMidas.ElementClicked += new EventHandler<WebElementEventArgs>(firingDriver_ButtonClicked);
        }

        private void CollectionChangedEvent(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //MessageBox.Show("Added value");
            }
        }

        private void Navigate_Click(object sender, RoutedEventArgs e)
        {

            burger.ScrapBurger();
            // betList = burger.GetArbsToJson();

            foreach (var bet in betList)
            {
                // navigate to betcategories
                //clickResponse(bet.sportName, "html/body/div[1]/div/div[2]/div[1]/div/div[1]/div/div/div", leftNav);
                // close all divs inside a category 
                //nav.closeAllOpenDivs();
                // Find Country
                //clickResponse("Ην. Βασίλειο", "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div", leftNav);
                // Find Division 
                //clickResponse("Αγγλία - Πρέμιερ Λιγκ", "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div[3]/div[2]/div", leftNav);
                // Select Match
                // clickResponse(bet.eventName, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div[1]/div[2]/div/div[1]/div/div/div", leftNav);

            }
        }

        private void clickResponse(string clickName, string path, divNav leftNav)
        {
            if (!leftNav.fetchLeftNav(clickName, path))
            {
                errorLabel.Content = clickName + " Not Found!";
            }
            else
            {
                errorLabel.Content = clickName;
            }
        }

        private void BurgerClick(object sender, RoutedEventArgs e)
        {
            watch = Stopwatch.StartNew();
            betList = burger.GetArbsToJson();
            //BetBurger.betList.Clear();
            for (int index = 0; index < betList.Count; index++)
            {
                clickResponse(betList[index].sportName, "html/body/div[1]/div/div[2]/div[1]/div/div[1]/div/div/div", leftNav);
                nav.closeAllOpenDivs(".sm-Market_HeaderOpen");
                clickResponse(betList[index].parentDiv, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div", leftNav);
                clickResponse(betList[index].childDiv, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div/div[2]/div/div", leftNav);
                clickResponse(betList[index].eventName, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div/div[2]/div/div[1]/div/div/div", leftNav);

                // html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div/div[2]/div/div[1]/div[4]/div[2]/div
                //nav.closeAllOpenDivs(".gl-MarketGroup_Open");
                //clickResponse("Goals Over/Under", "html /body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div/div[1]", leftNav);

            }
            listViewBetList.ItemsSource = betList;
            //BetBurger.betList.Clear();
            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            performace.Content = ($"{elapsedMs.ToString()} ms");
        }



        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }
    }
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




