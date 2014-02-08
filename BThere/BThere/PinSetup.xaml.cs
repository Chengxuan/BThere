using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;

namespace BThere
{
    public partial class PinSetup : PhoneApplicationPage
    {

        string returnPath = "ARPage.xaml";

        public PinSetup()
        {
            InitializeComponent();
        }

        private void saveBTN1_Click(object sender, EventArgs e)
        {
            if (VerifyMatch()) //pin is ok
            {
                App.pin = Convert.ToInt32(pin1tb.Text.ToString());
                App.childSafetyOn = false;
                SaveSetupLog();
                //MessageBoxResult result = MessageBox.Show("Pin saved", "BThere setup", MessageBoxButton.OK);
                MessageBox.Show("Pin saved");
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    NavigationService.Navigate(new Uri("/" + returnPath, UriKind.Relative));
                }
                
            }
        }

        private void SaveSetupLog()
        {
            IsolatedStorageFile myStorageArea = IsolatedStorageFile.GetUserStoreForApplication(); //creating an isolated storage file object
                
            string folderPath = "BThereFolder";
            string filePath = "BThereFolder\\SetupLog.txt";

            try
            {
                //if exist else creating a folder
                if (myStorageArea.DirectoryExists(folderPath) == false)
                    myStorageArea.CreateDirectory(folderPath);

                //opening or creating a new file
                using (var myIS_Stream = new IsolatedStorageFileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, myStorageArea))
                {
                    // create a stream writer to write data to the file
                    using (var myIS_FileWrite = new System.IO.StreamWriter(myIS_Stream))
                    {
                        //saving the highest score again
                        myIS_FileWrite.WriteLine(App.countryName);

                        //saving how many times game has been played till now
                        myIS_FileWrite.WriteLine(App.lat.ToString());

                        //saving how many times game has been played and won
                        myIS_FileWrite.WriteLine(App.lon.ToString());

                        //saving how many times game has been played and lost
                        myIS_FileWrite.WriteLine(App.pin.ToString());

                        myIS_FileWrite.WriteLine("false");


                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to save the file.\rSorry for that now" + e.Message, "BThere setup", MessageBoxButton.OK);
            }

            myStorageArea.Dispose();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // Good place to unhook event handlers too.
        }

        public bool CheckNetwork()
        {
            return (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType != Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var data = this.NavigationContext.QueryString;

            //err.Text += App.pin.ToString();

            if (data.ContainsKey("Page"))
            {
                returnPath = data["Page"];
            }

            /*
            countryShortCodeTB.Text = App.selectedCountryDetails.CountryShortName;
            countryCapitalTB.Text = App.selectedCountryDetails.CountryCapital;
            */

            //base.OnNavigatedTo(e);
        }

        private bool VerifyMatch()
        {
            string pin1 = pin1tb.Text.ToString();


            if (!(pin1.Length > 0))
            {
                pin1tb.Text = "";
                pin2tb.Text = "";
                MessageBox.Show("Please enter a 4 digit pin.", "BThere setup", MessageBoxButton.OK);
                return false;
            }

            if (pin1.Length < 4)
            {
                pin1tb.Text = "";
                pin2tb.Text = "";
                MessageBox.Show("Please enter a 4 digit pin.", "BThere setup", MessageBoxButton.OK);
                return false;
            }

            string pin2 = null;
            pin2 = pin2tb.Text.ToString();


            if (pin1.Contains("."))
            {
                pin1tb.Text = "";
                pin2tb.Text = "";
                MessageBox.Show("Pin can contain only digits", "BThere setup", MessageBoxButton.OK);
                return false;
            }


            if (pin2.Contains("."))
            {
                pin1tb.Text = "";
                pin2tb.Text = "";
                MessageBox.Show("Pin can contain only digits", "BThere setup", MessageBoxButton.OK);
                return false;
            }


            if (!(pin1.ToString() == pin2.ToString()))
            {
                pin1tb.Text = "";
                pin2tb.Text = "";
                MessageBox.Show("Pins don't match", "BThere setup", MessageBoxButton.OK);
                return false;
            }

            return true;
        }

    }
}