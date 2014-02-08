using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using FlickrNet;
using System.Windows.Input;

namespace BThere
{
    public partial class FImages : PhoneApplicationPage
    {

        public FImages()
        {
            InitializeComponent();
            ShowPic();
        }

        public void ShowPic()
        {
            if (CheckNetwork() == true)
            {
                MessageBox.Show("A network connection can not be established.\r\nPlease press refresh or check your network settings.");
                return;
            }
            else
            {
                picList.ItemsSource = "";

                try
                {
                    //to use Flickr go to Project-> Manage NuGet Packages-> Search "Flickr" -> install for windows phone 7


                    Flickr flickr = new Flickr("9a03824af501c318fec232146c6b1d05", "cd5cbd132cfbc60c"); // Authorise by api key and secret
                    PhotoSearchOptions options = new PhotoSearchOptions();
                    options.Tags = App.selectedCountryDetails.CountryCapital.ToString(); //give a key word to search

                    flickr.PhotosSearchAsync(options, (pictures) =>
                    {
                        picList.Dispatcher.BeginInvoke(new Action(delegate()
                        {
                            picList.ItemsSource = pictures.Result; //binding source to listbox
                        }));
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show("An error occured. Please exit app and try again.\r\nError Details: " + e.Message.ToString());
                }
            }
        }

        //tap image function
        private void img_tap(object sender, GestureEventArgs e)
        {
            Image img = sender as Image;
            NavigationService.Navigate(new Uri("/ZoomIn.xaml?id=" + img.Tag, UriKind.Relative));
        }

        private void refreshMenu_Click(object sender, EventArgs e)
        {
            ShowPic();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        public bool CheckNetwork()
        {
            return (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            /*
            var data = this.NavigationContext.QueryString;

            if (data.ContainsKey("c") && data.ContainsKey("lat") && data.ContainsKey("lon"))
            {
                CountryTextBlock.Text = data["c"];
                LatitudeTextBlock.Text = data["lat"];
                LongitudeTextBlock.Text = data["lon"];
            }
            */



            /*
            countryShortCodeTB.Text = App.selectedCountryDetails.CountryShortName;
            countryCapitalTB.Text = App.selectedCountryDetails.CountryCapital;
            */

            //base.OnNavigatedTo(e);
        }

    }
}