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
using System.Reflection;

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
            BetBurger.betTypesList.Add("1X2-1()");
            BetBurger.betTypesList.Add("1X2-X()");
            BetBurger.betTypesList.Add("1X2-2()");
            // BindingOperations.EnableCollectionSynchronization(betList,lockObject);           
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            //dispatcherTimer.Start();
            this.DataContext = this;
            //driver.Manage().Window.Maximize();
            midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            dictionary = File.ReadLines(@"logs/LOG.txt").Select(line => line.Split(','));
            foreach (string[] e in dictionary)
            {
                BetBurger.historyList.Add(new BetList(e[0], e[1], e[2], e[3], e[4], e[5], e[6], e[7], e[8], e[9], e[10], e[11], e[12], e[13], Convert.ToBoolean(e[14]), Convert.ToBoolean(e[15]), Convert.ToBoolean(e[16]), Convert.ToInt32(e[17])));
            }
            
            //BetBurger.betList.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedEvent);
        }



        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            watch = Stopwatch.StartNew();
            //threading
            btnFetchBurger.IsEnabled = false;
            betList = await Task.Factory.StartNew(() => burger.GetArbsToJson()).ConfigureAwait(true);
            //await slowBurgerScrapper;
            var slowBetClicker = Task.Factory.StartNew(() => SiteClicker()).ConfigureAwait(true);
            await slowBetClicker;
            listViewBetList.ItemsSource = null;
            listViewBetList.ItemsSource = betList;
            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            performace.Content = elapsedMs.ToString() + "ms";
            ITERATIONS++;
            errorLabel.Content = ITERATIONS.ToString();
            dispatcherTimer.Start();
        }

        private void SiteClicker()
        {
            
            for (int index = 0; index < betList.Count; index++)
            {
                if (!betList[index].thrown && !betList[index].coefChanged && betList[index].faultCounter <= 2 /*&& !BetBurger.historyList[index].thrown && !BetBurger.historyList[index].coefChanged && BetBurger.historyList[index].faultCounter <= 2*/)
                {
                    clickResponse(betList[index].sportName, "html/body/div[1]/div/div[2]/div[1]/div/div[1]/div/div/div", leftNav);
                    nav.closeAllOpenDivs(".sm-Market_HeaderOpen");
                    clickResponse(betList[index].parentDiv, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div", leftNav);
                    clickResponse(betList[index].childDiv, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div/div[2]/div/div", leftNav);
                    bool eventFound = clickResponse(betList[index].eventName, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div/div[2]/div/div[1]/div/div/div", leftNav);
                    nav.closeAllOpenDivs(".gl-MarketGroup_Open");
                    clickResponse("Full Time Result", "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div/div[1]", leftNav);
                    //Koef(betList[index].koef, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div[3]/div[2]/div/div/div/span[2]", leftNav);
                    //placeMaxBet();
                    if (eventFound)
                    {
                        if (BetMap(betList[index].betType, betList[index].koef))
                        {
                            //if (placeMaxBet())
                            //{
                            //    betList[index].thrown = true;
                            //}

                            if (placeFifty())
                            {
                                betList[index].thrown = true;
                            }
                        }
                        else
                        {
                            betList[index].coefChanged = true;
                            betList[index].faultCounter++;
                        }
                    }
                    CreateCSVFromGenericList(betList, "C:\\Users\\John\\Documents\\Visual Studio 2017\\Projects\\BetBot\\BetBot\\bin\\Debug\\logs\\LOG.txt", betList[index]);
                }
            }            
        }

        private bool BetMap(string betType, string koef)
        {
            bool betThrown = false;
            switch (betType)
            {
                case "1X2-1()":
                    betThrown = Koef(koef, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div[3]/div[2]/div/div/div[1]/span[2]", leftNav);
                    return (betThrown) ? true : false;
                case "1X2-X()":
                    betThrown = Koef(koef, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div[3]/div[2]/div/div/div[2]/span[2]", leftNav);
                    return (betThrown) ? true : false;

                case "1X2-2()":
                    betThrown = Koef(koef, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div[3]/div[2]/div/div/div[3]/span[2]", leftNav);
                    return (betThrown) ? true : false;

                //case "1X2-2()":
                //    clickResponse("Goals", "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div[1]/div/div[2]/div/div[3]", leftNav);
                //    Koef(koef, "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div[3]/div[2]/div/div/div[3]/span[2]", leftNav);
                //    break;
                default:
                    return false;
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

            //Connect to 365 and login!
            midas.Navigate().GoToUrl("https://www.bet365.gr/en/");
            IWebElement element = midas.FindElement(By.Id("TopPromotionButton"));
            element.Click();
            System.Threading.Thread.Sleep(1000);
            IWebElement Odds = midas.FindElement(By.XPath("html/body/div[1]/div/div[1]/div/div[2]/div[2]/div[3]/a"));
            Odds.Click();
            IWebElement hiddenList = midas.FindElement(By.XPath("html/body/div[1]/div/div[1]/div/div[2]/div[2]/div[3]/div/div/a[2]"));
            hiddenList.Click();
            //login
            IWebElement UserNameText = midas.FindElement(By.CssSelector(".hm-Login_InputField"));
            IWebElement PasswordText = midas.FindElement(By.XPath("html/body/div[1]/div/div[1]/div/div[1]/div[2]/div/div[2]/input[1]"));
            UserNameText.SendKeys(userName365TextBox.Text);
            PasswordText.Click();
            IWebElement PasswordText2 = midas.FindElement(By.XPath("html/body/div[1]/div/div[1]/div/div[1]/div[2]/div/div[2]/input[2]"));
            PasswordText2.SendKeys(password365TextBox.Text);
            IWebElement element2 = midas.FindElement(By.CssSelector(".hm-Login_LoginBtn"));
            element2.Click();

            //re-Enable button!
            btnNavigate.IsEnabled = true;
        }

        private bool clickResponse(string clickName, string path, divNav leftNav)
        {
            if (!leftNav.fetchLeftNav(clickName, path))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool Koef(string koef, string path, divNav leftNav)
        {
            if (!leftNav.KoefToDouble(koef, path))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool placeMaxBet()
        {
            midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(300);
            IWebElement iframe;
            IWebElement element;
            try
            {
                iframe = midas.FindElement(By.TagName("iframe"));
                midas.SwitchTo().Frame(iframe);
                element = midas.FindElement(By.XPath("html/body/div[1]/div/ul/li[3]/ul/li/div[3]/div[2]/span"));
                if (element.Displayed)
                {
                    element.Click();
                }

                element = midas.FindElement(By.XPath("html/body/div[1]/div/ul/li[8]/a[2]/div"));
                if (element.Displayed && !(element.Text == "Accept Changes"))
                {
                    element.Click();
                    midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                    midas.SwitchTo().ParentFrame();
                    return true;
                }
                else
                {
                    midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                    midas.SwitchTo().ParentFrame();
                    return false;
                }

            }
            catch (Exception)
            {
                midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                midas.SwitchTo().ParentFrame();
                return false;
            }

        }

        private bool placeFifty()
        {
            midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(300);
            IWebElement iframe;
            IWebElement element;
            try
            {
                iframe = midas.FindElement(By.TagName("iframe"));
                midas.SwitchTo().Frame(iframe);
                element = midas.FindElement(By.XPath("html/body/div[1]/div/ul/li[3]/ul/li/div[3]/div[1]/div[1]/input"));
                if (element.Displayed)
                {
                    element.SendKeys("0.50");
                }

                element = midas.FindElement(By.XPath("html/body/div[1]/div/ul/li[8]/a[2]/div"));
                if (element.Displayed && !(element.Text == "Accept Changes"))
                {
                    element.Click();
                    midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                    midas.SwitchTo().ParentFrame();
                    return true;
                }
                else
                {
                    midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                    midas.SwitchTo().ParentFrame();
                    return false;
                }

            }
            catch (Exception)
            {
                midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                midas.SwitchTo().ParentFrame();
                return false;
            }

        }

        private async void BurgerClick(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Start();
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BetBurger.betList.Clear();
        }

        public static void CreateCSVFromGenericList<BetList>(ObservableCollection<BetList> list, string csvCompletePath,BetList item)
        {
            if (list == null || list.Count == 0) return;

            //get type from 0th member
            Type t = list[0].GetType();
            string newLine = Environment.NewLine;

            if (!Directory.Exists(System.IO.Path.GetDirectoryName(csvCompletePath))) Directory.CreateDirectory(System.IO.Path.GetDirectoryName(csvCompletePath));

           // if (!File.Exists(csvCompletePath)) File.Create(csvCompletePath);

            using (var sw = new StreamWriter(csvCompletePath,true))
            {
                //make a new instance of the class name we figured out to get its props
                object o = Activator.CreateInstance(t);
                //gets all properties
                PropertyInfo[] props = o.GetType().GetProperties();

                //foreach of the properties in class above, write out properties
                //this is the header row
                //sw.Write(string.Join(",", props.Select(d => d.Name).ToArray()) + newLine);

                //this acts as datarow
                //foreach (BetList item in list)
                //{
                    //this acts as datacolumn
                    var row = string.Join(",", props.Select(d => item.GetType()
                                                                    .GetProperty(d.Name)
                                                                    .GetValue(item, null)
                                                                    .ToString())
                                                            .ToArray());
                    sw.Write(row + newLine);

               // }
            }
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




