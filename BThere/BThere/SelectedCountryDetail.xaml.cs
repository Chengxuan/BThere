using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Device.Location;

namespace BThere
{
    public partial class SelectedCountryDetail : PhoneApplicationPage
    {

        ApplicationBar appbar;

        public SelectedCountryDetail()
        {
            InitializeComponent();
        }

        private void DisplayMenu()
        {
            if (App.childSafetyOn == false)
            {
                DisplayMenuOptions();
            }
            else
            {
                var ib1 = new ApplicationBarIconButton(new Uri("/images/admin.png", UriKind.Relative)) { Text = "Advance Features" };
                ib1.Click += new EventHandler(ShowMenuOption);

                appbar.Buttons.Add(ib1);
            }


            this.ApplicationBar = appbar;
        }

        private void DisplayMenuOptions()
        {
            if (App.childSafetyOn == false)
            {
                appbar.MenuItems.Clear();

                var twitter = new ApplicationBarMenuItem("Twitter Hashtags");
                var news = new ApplicationBarMenuItem("Top Stories");
                var flickr = new ApplicationBarMenuItem("Pictures");
                var video = new ApplicationBarMenuItem("Live Video");
                var timeCapsule = new ApplicationBarMenuItem("Time Capsule");
                var help = new ApplicationBarMenuItem("Help");
                var changePin = new ApplicationBarMenuItem("Change PIN");



                help.Click += (a, b) =>
                {
                    // goto help page
                    NavigationService.Navigate(new Uri("/Help.xaml?r=SelectedCountryDetail.xaml", UriKind.Relative));
                };


                twitter.Click += (a, b) =>
                {
                    // goto twitter page
                    NavigationService.Navigate(new Uri("/TSearch.xaml?Page=SelectedCountryDetail.xaml", UriKind.Relative));

                };


                news.Click += (a, b) =>
                {
                    // goto twitter page
                    NavigationService.Navigate(new Uri("/NewsReader.xaml?Page=SelectedCountryDetail.xaml", UriKind.Relative));

                };


                flickr.Click += (a, b) =>
                {
                    // goto flickr images page
                    NavigationService.Navigate(new Uri("/FImages.xaml?Page=SelectedCountryDetail.xaml", UriKind.Relative));
                };

                changePin.Click += (a, b) =>
                {
                    // goto change pin page
                    NavigationService.Navigate(new Uri("/PinSetup.xaml?Page=SelectedCountryDetail.xaml", UriKind.Relative));
                };

                video.Click += (a, b) =>
                {
                    // goto video page
                    //NavigationService.Navigate(new Uri("/PinSetup.xaml?Page=SelectedCountryDetail.xaml", UriKind.Relative));
                };

                timeCapsule.Click += (a, b) =>
                {
                    // goto time capsule video page
                    NavigationService.Navigate(new Uri("/TimeCapsule.xaml?Page=SelectedCountryDetail.xaml", UriKind.Relative));
                };

                appbar.MenuItems.Add(news);
                appbar.MenuItems.Add(flickr);
                appbar.MenuItems.Add(twitter);
                appbar.MenuItems.Add(timeCapsule);
                appbar.MenuItems.Add(video);
                appbar.MenuItems.Add(changePin);
                appbar.MenuItems.Add(help);


            }
            else
            {
                if (App.pin.ToString().Length > 1) //pin was setup
                    NavigationService.Navigate(new Uri("/PinChallenge.xaml?Page=SelectedCountryDetail.xaml", UriKind.Relative));
                else
                    NavigationService.Navigate(new Uri("/PinSetup.xaml?Page=SelectedCountryDetail.xaml", UriKind.Relative));
            }
            

        }

        private void ShowMenuOption(object sender, EventArgs e)
        {
            DisplayMenuOptions();
        }

        private double GetDistance()
        {
            var sCoord = new GeoCoordinate(App.lat, App.lon);
            var eCoord = new GeoCoordinate(App.selectedCountryDetails.lat, App.selectedCountryDetails.lon);

            return sCoord.GetDistanceTo(eCoord);
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // Good place to unhook camera event handlers too.
            //Loaded -= SelectedCountryDetail_Loaded;
        }

        public bool CheckNetwork()
        {
            return (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType != Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {


            appbar = new ApplicationBar();

            DisplayMenu();
            
            PutInformation();

            var data = this.NavigationContext.QueryString;
            /*
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

        private void PutInformation()
        {
            //put flag
            flag.Source = new BitmapImage(new Uri("/Images/flags/" + App.selectedCountryDetails.CountryShortName + ".png", UriKind.Relative));


            #region longname
            if (App.selectedCountryDetails.CountryLongName != null && App.selectedCountryDetails.CountryLongName.ToString().Length > 0)
            {
                countryName.Text = App.selectedCountryDetails.CountryLongName;
            }
            #endregion

            #region short name
            if (App.selectedCountryDetails.CountryShortName != null && App.selectedCountryDetails.CountryShortName.ToString().Length > 0)
            {
                countryShortCodeTB.Text = App.selectedCountryDetails.CountryShortName.ToUpper();
                shortCodeGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                shortCodeGrid.Visibility = System.Windows.Visibility.Collapsed;

            }
            #endregion

            #region capital
            if (App.selectedCountryDetails.CountryCapital != null && App.selectedCountryDetails.CountryCapital.ToString().Length > 0)
            {
                countryCapitalTB.Text = App.selectedCountryDetails.CountryCapital;
                capitalGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                capitalGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

            #region other cities
            if (App.selectedCountryDetails.CountryOtherLargestCities != null && App.selectedCountryDetails.CountryOtherLargestCities.ToString().Length > 0)
            {
                string[] cities = App.selectedCountryDetails.CountryOtherLargestCities.ToString().Trim().Split(',');
                for(int i=0; i<cities.Length; i++)
                {
                    if (cities.Length > 1)
                    {
                        if(cities.Length==(i+1))
                            countryOtherCitiesTB.Text += cities[i].Trim();
                        else
                            countryOtherCitiesTB.Text += cities[i].Trim() + "\r\n";
                    }
                    else
                        countryOtherCitiesTB.Text += cities[i];
                }

                otherCitiesGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                otherCitiesGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

            #region president
            if (App.selectedCountryDetails.CountryPresident != null && App.selectedCountryDetails.CountryPresident.ToString().Length > 0)
            {
                countryPresidentTB.Text = App.selectedCountryDetails.CountryPresident;
                presidentGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                presidentGrid.Visibility = System.Windows.Visibility.Collapsed;

            }
            #endregion

            #region prime minister
            if (App.selectedCountryDetails.CountryPrimeMinister != null && App.selectedCountryDetails.CountryPrimeMinister.ToString().Length > 0)
            {
                countryPrimeMinisterTB.Text = App.selectedCountryDetails.CountryPrimeMinister;
                primeMinisterGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                primeMinisterGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

            #region population
            if (App.selectedCountryDetails.CountryPopulation > 0)
            {
                countryPopulationTB.Text = App.selectedCountryDetails.CountryPopulation.ToString();
                populationGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                populationGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

            #region currency
            if (App.selectedCountryDetails.CountryCurrency != null && App.selectedCountryDetails.CountryCurrency.ToString().Length > 0)
            {
                countryCurrencyTB.Text = App.selectedCountryDetails.CountryCurrency.ToString();
                currencyGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                currencyGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

            #region national day
            if (App.selectedCountryDetails.CountryNationalDay != null && App.selectedCountryDetails.CountryNationalDay.ToString().Length > 0)
            {
                string[] date = App.selectedCountryDetails.CountryNationalDay.ToString().Split('-');
                DateTime nationalDay = new DateTime(Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(date[2]));

                string monthName = "";
                switch (Convert.ToInt32(date[1]))
                {
                    case 1:
                        monthName = "January";
                        break;
                    case 2:
                        monthName = "Febuary";
                        break;
                    case 3:
                        monthName = "March";
                        break;
                    case 4:
                        monthName = "April";
                        break;
                    case 5:
                        monthName = "May";
                        break;
                    case 6:
                        monthName = "June";
                        break;
                    case 7:
                        monthName = "July";
                        break;
                    case 8:
                        monthName = "August";
                        break;
                    case 9:
                        monthName = "Septemeber";
                        break;
                    case 10:
                        monthName = "October";
                        break;
                    case 11:
                        monthName = "Novemeber";
                        break;
                    case 12:
                        monthName = "December";
                        break;
                }

                countryNationalDayTB.Text = nationalDay.Day + " " + monthName;
                nationalDayGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                nationalDayGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

            #region language
            if (App.selectedCountryDetails.CountryLanguage != null && App.selectedCountryDetails.CountryLanguage.ToString().Length > 0)
            {
                countryLanguageTB.Text = App.selectedCountryDetails.CountryLanguage.ToString();
                languageGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                languageGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

            #region religion
            if (App.selectedCountryDetails.CountryReligion != null && App.selectedCountryDetails.CountryReligion.ToString().Length > 0)
            {
                countryReligionTB.Text = App.selectedCountryDetails.CountryReligion.ToString();
                religionGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                religionGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

            #region literacy
            if (App.selectedCountryDetails.CountryLiteracyRate.ToString().Length > 0)
            {
                countryLiteracyRateTB.Text = (App.selectedCountryDetails.CountryLiteracyRate * 100).ToString();
                literacyGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                literacyGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

            #region distance
            double distance = GetDistance() / 1000;
            if (distance > 0)
            {
                countryDistanceTB.Text = distance.ToString("0km");
                distanceGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                distanceGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            #endregion

        }
    }
}