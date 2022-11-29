using Microsoft.Win32;
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
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WhatsAppSender.Model;
using System.Web;
using System.Net;
using System.Threading;

namespace WhatsAppSender
{

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        
        
            //new ChromeDriver(@"C:");
        
        
        //System("webdriver.chrome.driver", "C:\\Users\\Shalini\\Downloads\\chrom86_driver\\chromedriver.exe");
        
        private List<Contact> contacts = new List<Contact>();
        private void ContactButton_Click(object sender, RoutedEventArgs e)
        {

            String[] contactFileData = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.CheckFileExists = true;
            ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                contactFileData = File.ReadAllLines(ofd.FileName);

                try
                {
                    foreach (var item in contactFileData)
                    {
                        contacts.Add(new Contact(long.Parse(item)));
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.CheckFileExists = true;
            ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                messageBox.Text = File.ReadAllText(ofd.FileName);
            }
        }

        private void SendButtonTelegramm_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {

            
            if (!String.IsNullOrEmpty(messageBox.Text.Trim()))
            {
                String message = Uri.EscapeDataString(messageBox.Text);
                ChromeOptions options = new ChromeOptions();
                
                //options.BinaryLocation = "C:/Program Files/Google/Chrome/Application/chrome.exe";
                options.AddUserProfilePreference("Paul Newman", @"C:/");
                options.AddArgument("--profile-directory=Profile_1");
                options.AddArgument("--user-data-dir=C:/custom_profile");
                //options.AddArgument("--headless=chrome");
                //options.BrowserVersion = "107.0.5304.62";
                //options.AddArgument("--disable-gpu");
                options.AcceptInsecureCertificates = true;
               
                //options.AddArguments("window-size=1920x1080");

                ChromeDriver driver = new ChromeDriver(@"c:/" ,options);

                
                driver.Navigate().GoToUrl("https://web.whatsapp.com/");
                var page = driver.PageSource;
                Thread.Sleep(15000);
                var page2 = driver.PageSource;
                //Screenshot screenshot = driver.GetScreenshot();
                //screenshot.SaveAsFile("QR.png", System.Drawing.Imaging.ImageFormat.Png);
                foreach (Contact i in contacts)
                {
                    try
                    {
                        driver.ExecuteScript("window.open('');");
                        driver.SwitchTo().Window(driver.WindowHandles[1]);
                        driver.Navigate().GoToUrl("https://web.whatsapp.com/send?phone=" + i.ContactNumber + "&text=" + message + "&source&data&app_absent");
                        Thread.Sleep(5000);
                        var page3 = driver.PageSource;
                        try
                        {
                            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("._1Ae7k"))).Click();
                            
                            output.AppendText(i.ContactNumber+" Its OK!\n");
                     
                        }
                        catch (Exception)
                        {
                            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));

                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.ClassName("_1U1xa"))).Click();
                            output.AppendText(i.ContactNumber + " Sent\n");
                        }

                        finally {
                            Thread.Sleep(4000);
                            driver.SwitchTo().Window(driver.WindowHandles[1]);
                            driver.Close();
                            driver.SwitchTo().Window(driver.WindowHandles[0]);
                        }
                        
                        
                       

                    }
                    catch (Exception eb) {
                        output.AppendText(i.ContactNumber + " Error: " + eb.Message + "\n");
                    }

                }


                //driver.Quit();
                MessageBox.Show("Done Sending All Messages");
                driver.Quit();
                messageBox.Clear();
                contacts.Clear();
            }
        }
    }
}
