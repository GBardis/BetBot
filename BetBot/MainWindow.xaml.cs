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




namespace BetBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IWebDriver midas = new FirefoxDriver();
        Dictionary<string, string> dict = new Dictionary<string, string>();
        Navigation nav = new Navigation();
        private readonly IEnumerable<string[]> dictionary;

        public MainWindow()
        {
            InitializeComponent();
            //driver.Manage().Window.Maximize();
            midas.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            midas.Navigate().GoToUrl("http://www.bet365.gr/#/HO");
            IWebElement element = midas.FindElement(By.Id("TopPromotionButton"));
            element.Click();
            dictionary = File.ReadLines(@"Football.csv").Select(line => line.Split(','));
            foreach (string[] e in dictionary)
            {
                dict.Add(e[0].ToString(), e[1].ToString());
            }
        }

        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            BetBurger burger = new BetBurger();
            burger.scrapBurger();
            divNav leftNav = new divNav();
            // navigate to betcategories
            clickResponse("Ποδόσφαιρο", "html/body/div[1]/div/div[2]/div[1]/div/div[1]/div/div/div", leftNav);
            // close all divs inside a category 
            nav.closeAllOpenDivs();
            // Find Country
            clickResponse("Ην. Βασίλειο", "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div", leftNav);
            // Find Division 
            clickResponse("Αγγλία - Πρέμιερ Λιγκ", "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div[1]/div[3]/div[2]/div[3]/div[2]/div", leftNav);
            // Select Match
            clickResponse("Άρσεναλ v Λέστερ", "html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/div/div[1]/div[2]/div/div[1]/div/div/div", leftNav);
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
    }
}
