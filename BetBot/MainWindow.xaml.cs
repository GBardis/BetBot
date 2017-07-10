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
using System.Windows.Threading;

namespace BetBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public static IWebDriver midas  = new FirefoxDriver();
        //public static EventFiringWebDriver firingMidas = new EventFiringWebDriver(midas);
        Dictionary<string, string> dict = new Dictionary<string, string>();
        Navigation nav = new Navigation();
        private readonly IEnumerable<string[]> dictionary;
        BetBurger burger = new BetBurger();
        ObservableCollection<BetList> betList = new ObservableCollection<BetList>();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        divNav leftNav = new divNav();
        private Stopwatch watch;

        int ITERATIONS = 0;
        private object lockObject;

        //placedBets functionality

        public MainWindow()
        {
            //FirefoxProfile p = new FirefoxProfile();
            //p.SetPreference("javascript.enabled", false);

            InitializeComponent();
           // BindingOperations.EnableCollectionSynchronization(betList,lockObject);
            
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            //dispatcherTimer.Start();
            this.DataContext = this;
            //driver.Manage().Window.Maximize();
            midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            midas.Navigate().GoToUrl("https://www.bet365.gr/en/");
            IWebElement element = midas.FindElement(By.Id("TopPromotionButton"));
            element.Click();
            dictionary = File.ReadLines(@"Football.csv").Select(line => line.Split(','));
            foreach (string[] e in dictionary)
            {
                dict.Add(e[0].ToString(), e[1].ToString());
            }

            //BetBurger.betList.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedEvent);
           
        }

        private async void  dispatcherTimer_Tick(object sender,EventArgs e)
        {
            dispatcherTimer.Stop();
            //threading
            btnFetchBurger.IsEnabled = false;
            betList = await Task.Factory.StartNew(() => burger.GetArbsToJson()).ConfigureAwait(true);
            //await slowBurgerScrapper;
            var slowBetClicker = Task.Factory.StartNew(() => SiteClicker()).ConfigureAwait(true);
            await slowBetClicker;
            listViewBetList.ItemsSource = null;
            listViewBetList.ItemsSource = betList;
            
            ITERATIONS++;
            errorLabel.Content = ITERATIONS.ToString();
            dispatcherTimer.Start();
        }

        private void SiteClicker()
        {
            for (int index = 0; index < betList.Count; index++)
            {
                if (!betList[index].thrown)
                {
                    clickResponse(betList[index].sportName, "html/body/div[1]/div/div[2]/div[1]/div/div[1]/div/div/div", leftNav);
                    nav.closeAllOpenDivs(".sm-Market_HeaderOpen");
                    clickResponse(betList[index].parentDiv, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div", leftNav);
                    clickResponse(betList[index].childDiv, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div/div[2]/div/div", leftNav);
                    clickResponse(betList[index].eventName, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div/div[2]/div/div[1]/div/div/div", leftNav);
                    betList[index].thrown = true;
                }

            }
        }

        private void CollectionChangedEvent(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                MessageBox.Show("Added value");
            }
        }

        private async void Navigate_Click(object sender, RoutedEventArgs e)
        {
            btnNavigate.IsEnabled = false;
            //threading
            var slowNavigation = Task.Factory.StartNew(() => burger.ScrapBurger());
            // burger.ScrapBurger();                      
            await slowNavigation;
            btnNavigate.IsEnabled = true;
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
        private void Koef(string koef, string path, divNav leftNav)
        {
            if (!leftNav.KoefToDouble(koef, path))
            {
                errorLabel.Content = koef + " Not Found!";
            }
            else
            {
                errorLabel.Content = koef;
            }

        }

        private async void BurgerClick(object sender, RoutedEventArgs e)
        {
            watch = Stopwatch.StartNew();
            dispatcherTimer.Start();
            //   THIS CODE RUNS IN THE dispatcherTimer_Tick METHOD!
            //betList = burger.GetArbsToJson();
            //for (int index = 0; index < betList.Count; index++)
            //{
            //    if (!betList[index].thrown)
            //    {
            //        clickResponse(betList[index].sportName, "html/body/div[1]/div/div[2]/div[1]/div/div[1]/div/div/div", leftNav);
            //        nav.closeAllOpenDivs(".sm-Market_HeaderOpen");
            //        clickResponse(betList[index].parentDiv, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div", leftNav);
            //        clickResponse(betList[index].childDiv, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div/div[2]/div/div", leftNav);
            //        clickResponse(betList[index].eventName, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div/div[2]/div/div[1]/div/div/div", leftNav);
            //        betList[index].thrown = true;
            //    }         
            //}
            //listViewBetList.ItemsSource = betList;
            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            performace.Content = ($"{elapsedMs.ToString()} ms");
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BetBurger.betList.Clear();
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




