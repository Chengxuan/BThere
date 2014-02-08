using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Runtime.Serialization;
using MobileServices.Sdk;
using System.Windows.Input;
using System.Device.Location;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;

namespace BThere
{
    public partial class TimeCapsule : PhoneApplicationPage
    {
        public static MobileServiceClient mobileServiceClient;
        MobileServiceTable<GlobalVideos> usrStatusTable = null;
        GlobalVideoList usrList = null;



        public TimeCapsule()
        {
            InitializeComponent();
        }

        public bool NoNetwork()
        {
            return (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
        }
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }



        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            
            if (NoNetwork() == false)
            {
                mobileServiceClient = new MobileServiceClient("https://bethere.azure-mobile.net/", "XdjbcQDgRsMEXdNKeiKoiILoWNWLJE48");
                usrStatusTable = mobileServiceClient.GetTable<GlobalVideos>();
                LoadListings();
            }
            else
            {
                MessageBox.Show("A network connection can not be established.\r\nPlease press refresh or check your network settings.");
                return;
            }

        }

        private void LoadListings()
        {
            if (NoNetwork() == true)
            {
                MessageBox.Show("A network connection can not be established.\r\nPlease press refresh or check your network settings.");
                return;
            }
            else
            {
                listBoxVideos.ItemsSource = "";
                usrList = new GlobalVideoList();
                usrList.usrInfo = new List<GlobalVideos>();
                //Async table from Azure
                usrStatusTable = mobileServiceClient.GetTable<GlobalVideos>();
                //Get Data and Bind them to list Box
                usrStatusTable.GetAll((res, err) =>
                {
                    
                    if (err != null)
                    {
                        Random ra = new Random();

                        //handle it
                        return;
                    }
                    foreach (GlobalVideos usrs in res)
                    {
                        usrList.usrInfo.Add(usrs);
                    }
                    listBoxVideos.ItemsSource = usrList.usrInfo.OrderByDescending(usrs => usrs.Date);
                });
            }

        }



        private void ListItem_Tap(object sender, GestureEventArgs e)
        {
            var whichItemClicked = sender as StackPanel;

            App.videoURL = whichItemClicked.Tag.ToString();

            //MessageBox.Show(""+whichItemClicked.Tag.ToString());

            NavigationService.Navigate(new Uri("/PlayVideo.xaml?r=TimeCapsule.xaml", UriKind.Relative));
        }

        public class GlobalVideoList
        {
            public List<GlobalVideos> usrInfo { get; set; }
        }

        public class GlobalVideos
        {
            public int Id { get; set; }

            [DataMember(Name = "country")]
            public string Countryname { get; set; }

            [DataMember(Name = "longtitude")]
            public double Longtitude { get; set; }

            [DataMember(Name = "latitude")]
            public double Latitude { get; set; }

            [DataMember(Name = "location")]
            public string Location { get; set; }

            [DataMember(Name = "date")]
            public DateTime Date { get; set; }

            [DataMember(Name = "videourl")]
            public string Videourl { get; set; }

            [DataMember(Name = "thumbnailUrl")]
            public string Thumbnailurl { get; set; }
        }

        private void RefreshMenu_Click(object sender, EventArgs e)
        {
            LoadListings();
        }

        private void RecordVid_Click(object sender, EventArgs e)
        {
            if (NoNetwork() == false)
            {
                NavigationService.Navigate(new Uri("/RecordVideo.xaml?r=TimeCapsule.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("A network connection can not be established.\r\nPlease press refresh or check your network settings.");
                return;
            }


        }

    }
}