using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BThere
{
    public partial class PinChallenge : PhoneApplicationPage
    {
        string returnPath = "ARPage.xaml";

        public PinChallenge()
        {
            InitializeComponent();
        }

        private void checkBTN1_Click(object sender, EventArgs e)
        {
            if (pin1tb.ToString().Length < 4)
            {
                pin1tb.Text = "";
                MessageBox.Show("Pin is not less than 4 digits");
                return;
            }

            string pin = pin1tb.Text.ToString();

            int pinEntered = Convert.ToInt32(pin);

            if (App.pin == pinEntered)
            {
                App.childSafetyOn = false;
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    NavigationService.Navigate(new Uri("/" + returnPath, UriKind.Relative));
                }
            }
            else
            {
                pin1tb.Text = "";
                MessageBox.Show("Incorrect Pin", "Pin Challenge", MessageBoxButton.OK);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            //err.Text += App.pin.ToString();

            var data = this.NavigationContext.QueryString;

            if (data.ContainsKey("Page"))
            {
                returnPath = data["Page"];
            }

            /*
            countryShortCodeTB.Text = App.selectedCountryDetails.CountryShortName;
            countryCapitalTB.Text = App.selectedCountryDetails.CountryCapital;
            */

            base.OnNavigatedTo(e);
        }
    }
}