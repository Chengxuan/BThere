using System;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.Generic;

namespace BThere
{
    public partial class ZoomIn : PhoneApplicationPage
    {

        IDictionary<string, string> data;

        public ZoomIn()
        {
            InitializeComponent();
        }

        public bool CheckNetwork()
        {
            return (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            if (CheckNetwork() == true)
            {
                MessageBox.Show("A network connection can not be established.\r\nPlease press refresh or check your network settings.");
                return;
            }
            else
            {

                data = this.NavigationContext.QueryString;

                if (data.ContainsKey("id"))
                {
                    // abc.Text = data["id"];
                    bigImg.Source = new BitmapImage(new Uri(data["id"], UriKind.Absolute));   //give url to image
                }
            }
        }
    }
}