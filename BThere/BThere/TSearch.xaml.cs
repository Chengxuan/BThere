using System;
using System.Net;
using System.Windows;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using TweetSharp;

namespace BThere
{
    public partial class TSearch : PhoneApplicationPage
    {

        public TSearch()
        {
            InitializeComponent();
            DisplayTweetList();
        }

        public void DisplayTweetList()
        {
            if (CheckNetwork() == true)
            {
                MessageBox.Show("A network connection can not be established.\r\nPlease press refresh or check your network settings.");
                return;
            }
            else
            {
                resultListBox.ItemsSource = "";

                TwitterService service = new TwitterService("W0b46KugjRZXjYNSxt1w", "lC4zTfoSsOlWRtfmXVfuSWS9KNKKfWLValqjFN1u4");
                service.AuthenticateWith("750912427-zEW7TYdGoswu3AzpCUPlD3rZsUc9HMXYJkKzrVR8", "7FBXTf2Xyihe0DLV4L97COGvgdVhQajE1ixM02Bw0");

                service.Search("#" + App.selectedCountryDetails.CountryLongName, (results, response) =>
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        resultListBox.Dispatcher.BeginInvoke(new Action(delegate()
                        {  // Add data to listbox(name=resultList)
                            //resultList.Items.Add(response.StatusDescription); //To know whether request succeed
                            // showResults.Text = response.RequestUri + results.RawSource;// Json file is here.
                            // resultList.Items.Add(response.Response);
                            //TwitterSearch.Result result = new TwitterSearch.Result();
                            //var deserializedJSON = JsonConvert.DeserializeObject<TwitterSearch.tResult>(results.RawSource);
                            //resultList.Items.Add(response.RequestUri);  //Sent query url
                            string resultString = results.RawSource.ToString();
                            RootObject1 tresult = JsonConvert.DeserializeObject<RootObject1>(resultString);
                            if (tresult.results.Count == 0)
                            {
                                tResult empty = new tResult();
                                empty.text = "Sorry,No Result was found";
                                empty.profile_image_url = "https://twimg0-a.akamaihd.net/profile_images/2284174758/v65oai7fxn47qv9nectx_normal.png";
                                tresult.results.Add(empty);
                                resultListBox.ItemsSource = tresult.results;
                            }
                            else
                            {
                                resultListBox.ItemsSource = tresult.results;
                            }
                        }));

                    }
                    else
                    {
                        this.Dispatcher.BeginInvoke(new Action(delegate()
                        {
                            MessageBox.Show("An error occured. Please exit app and try again.\r\nError Details: " + response.StatusCode.ToString());

                        }));
                    }
                });
            }
        }

        private void refreshMenu_Click(object sender, EventArgs e)
        {
            DisplayTweetList();
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