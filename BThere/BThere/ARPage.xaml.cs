using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Devices;
using System.Xml.Linq;
using Microsoft.Phone.Shell;

namespace BThere
{

    public partial class ARPage : PhoneApplicationPage
    {
        public Microsoft.Devices.PhotoCamera m_camera;
        bool cameraStatus = true;

        public ARPage()
        {
            InitializeComponent();
            videoBrushTransform.Rotation = 90;
            Loaded += ARPage_Loaded;
        }

        void ARPage_Loaded(object sender, RoutedEventArgs e)
        {

            ApplicationBar appBar = new ApplicationBar();

            ApplicationBarIconButton recordVideo = new ApplicationBarIconButton(new Uri("images/appbar.feature.video.rest.png", UriKind.Relative));
            recordVideo.Click += new EventHandler(RecordVideoMenu_Click);
            recordVideo.Text = "Record Video";

            ApplicationBarIconButton quiz = new ApplicationBarIconButton(new Uri("images/appbar.questionmark.rest.png", UriKind.Relative));
            quiz.Click += new EventHandler(TakeQuizMenu_Click);
            quiz.Text = "Quiz";

            ApplicationBarIconButton game = new ApplicationBarIconButton(new Uri("images/appbar.world.icon.png", UriKind.Relative));
            game.Click += new EventHandler(GameMenu_Click);
            game.Text = "Game";


            ApplicationBarIconButton settings = new ApplicationBarIconButton(new Uri("images/appbar.feature.settings.rest.png", UriKind.Relative));
            settings.Click += new EventHandler(SettingsMenu_Click);
            settings.Text = "Settings";


            var help = new ApplicationBarMenuItem("Help");
            help.Click += new EventHandler(HelpMenu_Click);



            appBar.Buttons.Add(settings);
            appBar.Buttons.Add(quiz);
            appBar.Buttons.Add(game);
            appBar.Buttons.Add(recordVideo);
            appBar.MenuItems.Add(help);



            ApplicationBar = appBar;



            if (App.childSafetyOn == false)
                recordVideo.IsEnabled = true;
            else
                recordVideo.IsEnabled = false;




        }


        private void LoadData()
        {
            arPanel.Children.Clear();

            if (App.allCoutries != null)
                App.allCoutries.Clear();

            XDocument loadedData = XDocument.Load("Resources/CountryDetails.xml");

            var data = from query in loadedData.Descendants("CountryDetails")
                       select new CountryDetails
                       {
                           CountryLongName = (string)query.Element("Country"),
                           CountryShortName = (string)query.Element("Short_Name"),
                           lat = Convert.ToDouble((string)query.Element("Latitude")),
                           lon = Convert.ToDouble((string)query.Element("Longitude")),
                           CountryCapital = (string)query.Element("Capital"),
                           CountryPresident = (string)query.Element("President"),
                           CountryPrimeMinister = (string)query.Element("Prime_Minister"),
                           CountryPopulation = Convert.ToDouble((string)query.Element("Population")),
                           CountryOtherLargestCities = (string)query.Element("Other_Large_Cities"),
                           CountryCurrency = (string)query.Element("Currency"),
                           CountryNationalDay = (string)query.Element("National_Day"),
                           CountryLanguage = (string)query.Element("Language"),
                           CountryReligion = (string)query.Element("Religion"),
                           CountryLiteracyRate = Convert.ToDouble((string)query.Element("Literacy_rate")),
                       };


            foreach (CountryDetails country in data)
            {
                App.allCoutries.Add(country);
                if (country.CountryLongName.Trim().ToLower() != App.countryName.Trim().ToLower())
                    pointVirtual(arPanel, App.lat, App.lon, country);
            }
            arPanel.Children.Add(nhp);
            arPanel.Children.Add(whp);
            arPanel.Children.Add(shp);
            arPanel.Children.Add(ehp);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            switch (e.Orientation)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                    videoBrushTransform.Rotation = 0;
                    break;
                case PageOrientation.LandscapeRight:
                    videoBrushTransform.Rotation = 180;
                    break;
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                    videoBrushTransform.Rotation = 90;
                    break;
                default: break;
            }

            base.OnOrientationChanged(e);
        }

        private void pointVirtual(SharpGIS.AR.Controls.ARPanel panel, double myLatitude, double myLongtitude, CountryDetails country)
        {
            double virtualLong = 0;
            double virtualLa = 0;
            double hisLatitude = country.lat;
            double hisLongtitude = country.lon;
            string countryName = country.CountryLongName;

            if (myLongtitude != hisLongtitude && myLongtitude + hisLongtitude != 180 && myLongtitude + hisLongtitude != -180)
            {
                if (myLongtitude > 0)
                {
                    if (hisLongtitude > 0)
                    {
                        if (myLongtitude - hisLongtitude > 0)
                        {
                            virtualLong = (hisLongtitude - myLongtitude) / 2;
                            //west
                            if (myLatitude - hisLatitude != 90 && myLatitude - hisLatitude != -90)
                            {
                                if (myLatitude > 0)
                                {
                                    if (hisLatitude > 0)
                                    {
                                        virtualLa = 270 + hisLatitude - myLatitude;//west
                                        // virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                    else
                                    {
                                        if (myLatitude - hisLatitude < 90)
                                        {
                                            virtualLa = 270 + hisLatitude - myLatitude;//west
                                            //  virtualLa = 90 + myLatitude - hisLatitude;//east
                                        }
                                        else
                                        {
                                            virtualLa = 180 + myLatitude - hisLatitude;//west
                                            // virtualLa = 180 - myLatitude + hisLatitude;//east
                                        }
                                    }
                                }
                                else
                                {
                                    if (hisLatitude > 0)
                                    {
                                        if (myLatitude - hisLatitude > -90)
                                        {
                                            virtualLa = 270 - myLatitude + hisLatitude;//west
                                            //virtualLa = 90 - hisLatitude + myLatitude;//east
                                        }
                                        else
                                        {
                                            virtualLa = 360 + myLatitude - hisLatitude;//west
                                            // virtualLa = hisLatitude - myLatitude;//east
                                        }
                                    }
                                    else
                                    {
                                        virtualLa = 270 - myLatitude + hisLatitude;//west
                                        //  virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                }
                            }
                            else
                            {
                                virtualLa = -90;
                            }
                        }
                        else
                        {
                            virtualLong = (myLongtitude - hisLongtitude) / 2;
                            //east
                            if (myLatitude - hisLatitude != 90 && myLatitude - hisLatitude != -90)
                            {
                                if (myLatitude > 0)
                                {
                                    if (hisLatitude > 0)
                                    {

                                        // virtualLa = 270 + hisLatitude - myLatitude;//west
                                        virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                    else
                                    {
                                        if (myLatitude - hisLatitude < 90)
                                        {
                                            //  virtualLa = 270 + hisLatitude - myLatitude;//west
                                            virtualLa = 90 + myLatitude - hisLatitude;//east
                                        }
                                        else
                                        {
                                            // virtualLa = 180 + myLatitude - hisLatitude;//west
                                            virtualLa = 180 - myLatitude + hisLatitude;//east
                                        }
                                    }
                                }
                                else
                                {
                                    if (hisLatitude > 0)
                                    {
                                        if (myLatitude - hisLatitude > -90)
                                        {
                                            // virtualLa = 270 - myLatitude + hisLatitude;//west
                                            virtualLa = 90 - hisLatitude + myLatitude;//east
                                        }
                                        else
                                        {
                                            //  virtualLa = 360 + myLatitude - hisLatitude;//west
                                            virtualLa = hisLatitude - myLatitude;//east
                                        }
                                    }
                                    else
                                    {
                                        //  virtualLa = 270 - myLatitude + hisLatitude;//west
                                        virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                }
                            }
                            else
                            {
                                virtualLa = -90;
                            }
                        }
                    }
                    else
                    {
                        if (180 + hisLongtitude - myLongtitude > 0)
                        {
                            virtualLong = (hisLongtitude - myLongtitude) / 2;
                            //west
                            if (myLatitude - hisLatitude != 90 && myLatitude - hisLatitude != -90)
                            {
                                if (myLatitude > 0)
                                {
                                    if (hisLatitude > 0)
                                    {
                                        virtualLa = 270 + hisLatitude - myLatitude;//west
                                        // virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                    else
                                    {
                                        if (myLatitude - hisLatitude < 90)
                                        {
                                            virtualLa = 270 + hisLatitude - myLatitude;//west
                                            //  virtualLa = 90 + myLatitude - hisLatitude;//east
                                        }
                                        else
                                        {
                                            virtualLa = 180 + myLatitude - hisLatitude;//west
                                            // virtualLa = 180 - myLatitude + hisLatitude;//east
                                        }
                                    }
                                }
                                else
                                {
                                    if (hisLatitude > 0)
                                    {
                                        if (myLatitude - hisLatitude > -90)
                                        {
                                            virtualLa = 270 - myLatitude + hisLatitude;//west
                                            //virtualLa = 90 - hisLatitude + myLatitude;//east
                                        }
                                        else
                                        {
                                            virtualLa = 360 + myLatitude - hisLatitude;//west
                                            // virtualLa = hisLatitude - myLatitude;//east
                                        }
                                    }
                                    else
                                    {
                                        virtualLa = 270 - myLatitude + hisLatitude;//west
                                        //  virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                }
                            }
                            else
                            {
                                virtualLa = -90;
                            }
                        }
                        else
                        {
                            virtualLong = (myLongtitude + hisLongtitude - 180) / 2;
                            //east
                            if (myLatitude - hisLatitude != 90 && myLatitude - hisLatitude != -90)
                            {
                                if (myLatitude > 0)
                                {
                                    if (hisLatitude > 0)
                                    {
                                        // virtualLa = 270 + hisLatitude - myLatitude;//west
                                        virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                    else
                                    {
                                        if (myLatitude - hisLatitude < 90)
                                        {
                                            //  virtualLa = 270 + hisLatitude - myLatitude;//west
                                            virtualLa = 90 + myLatitude - hisLatitude;//east
                                        }
                                        else
                                        {
                                            // virtualLa = 180 + myLatitude - hisLatitude;//west
                                            virtualLa = 180 - myLatitude + hisLatitude;//east
                                        }
                                    }
                                }
                                else
                                {
                                    if (hisLatitude > 0)
                                    {
                                        if (myLatitude - hisLatitude > -90)
                                        {
                                            // virtualLa = 270 - myLatitude + hisLatitude;//west
                                            virtualLa = 90 - hisLatitude + myLatitude;//east
                                        }
                                        else
                                        {
                                            //  virtualLa = 360 + myLatitude - hisLatitude;//west
                                            virtualLa = hisLatitude - myLatitude;//east
                                        }
                                    }
                                    else
                                    {
                                        //  virtualLa = 270 - myLatitude + hisLatitude;//west
                                        virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                }
                            }
                            else
                            {
                                virtualLa = -90;
                            }
                        }
                    }


                }
                else
                {
                    if (hisLongtitude > 0)
                    {
                        if (hisLongtitude - myLongtitude - 180 < 0)
                        {
                            virtualLong = (myLongtitude - hisLongtitude) / 2;
                            //east
                            if (myLatitude - hisLatitude != 90 && myLatitude - hisLatitude != -90)
                            {
                                if (myLatitude > 0)
                                {
                                    if (hisLatitude > 0)
                                    {
                                        // virtualLa = 270 + hisLatitude - myLatitude;//west
                                        virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                    else
                                    {
                                        if (myLatitude - hisLatitude < 90)
                                        {
                                            //  virtualLa = 270 + hisLatitude - myLatitude;//west
                                            virtualLa = 90 + myLatitude - hisLatitude;//east
                                        }
                                        else
                                        {
                                            // virtualLa = 180 + myLatitude - hisLatitude;//west
                                            virtualLa = 180 - myLatitude + hisLatitude;//east
                                        }
                                    }
                                }
                                else
                                {
                                    if (hisLatitude > 0)
                                    {
                                        if (myLatitude - hisLatitude > -90)
                                        {
                                            // virtualLa = 270 - myLatitude + hisLatitude;//west
                                            virtualLa = 90 - hisLatitude + myLatitude;//east
                                        }
                                        else
                                        {
                                            //  virtualLa = 360 + myLatitude - hisLatitude;//west
                                            virtualLa = hisLatitude - myLatitude;//east
                                        }
                                    }
                                    else
                                    {
                                        //  virtualLa = 270 - myLatitude + hisLatitude;//west
                                        virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                }
                            }
                            else
                            {
                                virtualLa = -90;
                            }
                        }
                        else
                        {
                            virtualLong = (hisLongtitude - myLongtitude - 360) / 2;
                            //west
                            if (myLatitude - hisLatitude != 90 && myLatitude - hisLatitude != -90)
                            {
                                if (myLatitude > 0)
                                {
                                    if (hisLatitude > 0)
                                    {
                                        virtualLa = 270 + hisLatitude - myLatitude;//west
                                        // virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                    else
                                    {
                                        if (myLatitude - hisLatitude < 90)
                                        {
                                            virtualLa = 270 + hisLatitude - myLatitude;//west
                                            //  virtualLa = 90 + myLatitude - hisLatitude;//east
                                        }
                                        else
                                        {
                                            virtualLa = 180 + myLatitude - hisLatitude;//west
                                            // virtualLa = 180 - myLatitude + hisLatitude;//east
                                        }
                                    }
                                }
                                else
                                {
                                    if (hisLatitude > 0)
                                    {
                                        if (myLatitude - hisLatitude > -90)
                                        {
                                            virtualLa = 270 - myLatitude + hisLatitude;//west
                                            //virtualLa = 90 - hisLatitude + myLatitude;//east
                                        }
                                        else
                                        {
                                            virtualLa = 360 + myLatitude - hisLatitude;//west
                                            // virtualLa = hisLatitude - myLatitude;//east
                                        }
                                    }
                                    else
                                    {
                                        virtualLa = 270 - myLatitude + hisLatitude;//west
                                        //  virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                }
                            }
                            else
                            {
                                virtualLa = -90;
                            }
                        }
                    }
                    else
                    {
                        if (myLongtitude - hisLongtitude > 0)
                        {
                            virtualLong = (hisLongtitude - myLongtitude) / 2;
                            //west
                            if (myLatitude - hisLatitude != 90 && myLatitude - hisLatitude != -90)
                            {
                                if (myLatitude > 0)
                                {
                                    if (hisLatitude > 0)
                                    {
                                        virtualLa = 270 + hisLatitude - myLatitude;//west
                                        // virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                    else
                                    {
                                        if (myLatitude - hisLatitude < 90)
                                        {
                                            virtualLa = 270 + hisLatitude - myLatitude;//west
                                            //  virtualLa = 90 + myLatitude - hisLatitude;//east
                                        }
                                        else
                                        {
                                            virtualLa = 180 + myLatitude - hisLatitude;//west
                                            // virtualLa = 180 - myLatitude + hisLatitude;//east
                                        }
                                    }
                                }
                                else
                                {
                                    if (hisLatitude > 0)
                                    {
                                        if (myLatitude - hisLatitude > -90)
                                        {
                                            virtualLa = 270 - myLatitude + hisLatitude;//west
                                            //virtualLa = 90 - hisLatitude + myLatitude;//east
                                        }
                                        else
                                        {
                                            virtualLa = 360 + myLatitude - hisLatitude;//west
                                            // virtualLa = hisLatitude - myLatitude;//east
                                        }
                                    }
                                    else
                                    {
                                        virtualLa = 270 - myLatitude + hisLatitude;//west
                                        //  virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                }
                            }
                            else
                            {
                                virtualLa = -90;
                            }
                        }
                        else
                        {
                            virtualLong = (myLongtitude - hisLongtitude) / 2;
                            //east
                            if (myLatitude - hisLatitude != 90 && myLatitude - hisLatitude != -90)
                            {
                                if (myLatitude > 0)
                                {
                                    if (hisLatitude > 0)
                                    {
                                        // virtualLa = 270 + hisLatitude - myLatitude;//west
                                        virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                    else
                                    {
                                        if (myLatitude - hisLatitude < 90)
                                        {
                                            //  virtualLa = 270 + hisLatitude - myLatitude;//west
                                            virtualLa = 90 + myLatitude - hisLatitude;//east
                                        }
                                        else
                                        {
                                            // virtualLa = 180 + myLatitude - hisLatitude;//west
                                            virtualLa = 180 - myLatitude + hisLatitude;//east
                                        }
                                    }
                                }
                                else
                                {
                                    if (hisLatitude > 0)
                                    {
                                        if (myLatitude - hisLatitude > -90)
                                        {
                                            // virtualLa = 270 - myLatitude + hisLatitude;//west
                                            virtualLa = 90 - hisLatitude + myLatitude;//east
                                        }
                                        else
                                        {
                                            //  virtualLa = 360 + myLatitude - hisLatitude;//west
                                            virtualLa = hisLatitude - myLatitude;//east
                                        }
                                    }
                                    else
                                    {
                                        //  virtualLa = 270 - myLatitude + hisLatitude;//west
                                        virtualLa = 90 + myLatitude - hisLatitude;//east
                                    }
                                }
                            }
                            else
                            {
                                virtualLa = -90;
                            }
                        }
                    }
                }

            }

            else
            {

                if (myLatitude - hisLatitude != 90 && myLatitude - hisLatitude != -90)
                {
                    if (myLatitude > 0)
                    {
                        if (hisLatitude > 0)
                        {
                            if (hisLatitude - myLatitude > 0)
                            {
                                virtualLa = 0;
                                virtualLong = myLatitude - hisLatitude;
                            }
                            else
                            {
                                virtualLa = 180;
                                virtualLong = myLatitude - hisLatitude;
                            }
                            // virtualLa = 270 + hisLatitude - myLatitude;//west
                            // virtualLa = 90 + myLatitude - hisLatitude;//east
                        }
                        else
                        {
                            if (myLatitude - hisLatitude < 90)
                            {
                                virtualLa = 180;
                                virtualLong = hisLatitude - myLatitude;
                                //  virtualLa = 270 + hisLatitude - myLatitude;//west
                                //  virtualLa = 90 + myLatitude - hisLatitude;//east
                            }
                            else
                            {
                                virtualLa = 0;
                                virtualLong = myLatitude - hisLatitude - 180;
                                // virtualLa = 180 + myLatitude - hisLatitude;//west
                                // virtualLa = 180 - myLatitude + hisLatitude;//east
                            }
                        }
                    }
                    else
                    {
                        if (hisLatitude > 0)
                        {
                            if (myLatitude - hisLatitude > -90)
                            {
                                virtualLa = 0;
                                virtualLong = myLatitude - hisLatitude;
                                // virtualLa = 270 - myLatitude + hisLatitude;//west
                                //virtualLa = 90 - hisLatitude + myLatitude;//east
                            }
                            else
                            {
                                virtualLa = 180;
                                virtualLong = hisLatitude - myLatitude - 180;
                                //  virtualLa = 360 + myLatitude - hisLatitude;//west
                                // virtualLa = hisLatitude - myLatitude;//east
                            }
                        }
                        else
                        {
                            if (myLatitude - hisLatitude < 0)
                            {
                                virtualLa = 0;
                                virtualLong = myLatitude - hisLatitude;
                            }
                            else
                            {
                                virtualLa = 180;
                                virtualLong = myLatitude - hisLatitude;
                            }
                            //  virtualLa = 270 - myLatitude + hisLatitude;//west
                            //  virtualLa = 90 + myLatitude - hisLatitude;//east
                        }
                    }
                }
                else
                {
                    virtualLa = -90;
                    virtualLong = 0;
                }
            }
            /*
            if (myLongtitude - hisLongtitude < 0)
            {
                if (myLongtitude - hisLongtitude > -180)
                {
                    virtualLong = (myLongtitude - hisLongtitude) / 2;
                    virtualLa = 90 + myLatitude - hisLatitude;
                }
                else
                {
                    virtualLong = (hisLongtitude - myLongtitude - 360) / 2;
                        virtualLa = 90 - myLatitude + hisLatitude;
                }
            }
            else
            {
                if (myLongtitude - hisLongtitude < 180)
                {
                    virtualLong = (hisLongtitude - myLongtitude) / 2;
                    virtualLa = 90 + myLatitude - hisLatitude;
                }
                else
                {
                    virtualLong = (myLongtitude - hisLongtitude - 360) / 2;
                    virtualLa = 90 - myLatitude + hisLatitude;
                }
            }
            TextBlock countryNameText = new TextBlock() { Text = countryName };
            SharpGIS.AR.Controls.ARPanel.SetDirection(countryNameText, new Point(0, 30));
            panel.Children.Add(countryNameText);
            TextBlock countryNameText1 = new TextBlock() { Text = countryName };
            SharpGIS.AR.Controls.ARPanel.SetDirection(countryNameText1, new Point(-10, 200));
            panel.Children.Add(countryNameText1);
            TextBlock countryNameText2 = new TextBlock() { Text = countryName };
            SharpGIS.AR.Controls.ARPanel.SetDirection(countryNameText2, new Point(-80, 90));
            panel.Children.Add(countryNameText2);
            TextBlock countryNameText3 = new TextBlock() { Text = countryName };
            SharpGIS.AR.Controls.ARPanel.SetDirection(countryNameText3, new Point(-30, 330));
            panel.Children.Add(countryNameText3);
             */


            Button countryNameBTN = new Button();
            countryNameBTN.Content = countryName;
            countryNameBTN.Tag = countryName + "/" + hisLatitude.ToString() + "/" + hisLongtitude.ToString();
            countryNameBTN.Foreground = new SolidColorBrush(Colors.Red);
            //countryNameBTN.Click += countryNameBTN_Click;
            countryNameBTN.Click += (a, b) => {
                App.selectedCountryDetails = country;
                Button clickedBtn = (Button)a;
                string[] information = clickedBtn.Tag.ToString().Split('/');
                var queryData = string.Format("?c={0}&lat={1}&lon={2}", information[0], information[1], information[2]);
                NavigationService.Navigate(new Uri("/SelectedCountryDetail.xaml" + queryData, UriKind.Relative));
            };
            countryNameBTN.BorderBrush = new SolidColorBrush(Colors.Transparent);
            countryNameBTN.BorderThickness = new Thickness(0);

            SharpGIS.AR.Controls.ARPanel.SetDirection(countryNameBTN, new Point(virtualLong, virtualLa));

            panel.Children.Add(countryNameBTN);



        }
        
        private void ARPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = sender as SharpGIS.AR.Controls.ARPanel;
            //For motion to be supported, device needs at least a compass and
            //an accelerometer. A gyro as well makes the experience much better though
            if (Microsoft.Devices.Sensors.Motion.IsSupported)
            {
                //Start the AR PAnel
                panel.Start();
            }
            else //Bummer! 
            {
                panel.Visibility = System.Windows.Visibility.Collapsed;
                //MessageBox.Show("Sorry - Motion sensor is not supported on this device. App cannot run!!!");
            }
        }

        private void ARPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            //Remember to stop the motion sensor when leaving
            (sender as SharpGIS.AR.Controls.ARPanel).Stop();
        }

        //private void AddHeadingDots(SharpGIS.AR.Controls.ARPanel panel)
        //{
        //    //Go 360 along the horizon to display heading text
        //    for (double heading = 20; heading < 360; heading += 20)
        //    {
        //        if (heading % 90 == 0) continue; //skip cardinal directions, since we already added markers for those in xaml
        //        TextBlock tb = new TextBlock() { Text = string.Format("{0}°", heading) };
        //        SharpGIS.AR.Controls.ARPanel.SetDirection(tb, new Point(0, heading));
        //        panel.Children.Add(tb);
        //    }
        //    //Display an up/down angle for each cardinal direction
        //    for (int i = 0; i < 360; i += 90)
        //    {
        //        for (int azimuth = -80; azimuth < 90; azimuth += 10)
        //        {
        //            if (azimuth == 0) continue; //skip cardinal directions, since we already added markers for those in xaml
        //            TextBlock tb = new TextBlock() { Text = string.Format("{0}", azimuth) };
        //            SharpGIS.AR.Controls.ARPanel.SetDirection(tb, new Point(azimuth, i));
        //            panel.Children.Add(tb);
        //        }
        //    }

        //    TextBlock tb1 = new TextBlock() { Text = "UK" };
        //    SharpGIS.AR.Controls.ARPanel.SetDirection(tb1, new Point(51.500622, -0.126662));
        //    panel.Children.Add(tb1);
        //}

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // Dispose camera to minimize power consumption and to expedite shutdown.

            m_camera.Dispose();

            // Good place to unhook camera event handlers too.
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {


            //var data = this.NavigationContext.QueryString;

            StartCamera();
            /*
            if (data.ContainsKey("c") && data.ContainsKey("lat") && data.ContainsKey("lon"))
            {
                CountryTextBlock.Text = data["c"];
                LatitudeTextBlock.Text = data["lat"];
                LongitudeTextBlock.Text = data["lon"];
            }*/

            LoadData();

            base.OnNavigatedTo(e);
        }

        private void StartCamera()
        {
            m_camera = new Microsoft.Devices.PhotoCamera();
            viewfinderBrush.SetSource(m_camera);
        }

        private void NoARMenu_Click(object sender, EventArgs e)
        {

            ApplicationBarIconButton menuItemClicked = sender as ApplicationBarIconButton;
            if (cameraStatus)
            {
                menuItemClicked.Text = "World View On";
                //m_camera.;
                videoRectangle.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                videoRectangle.Visibility = System.Windows.Visibility.Visible;
                menuItemClicked.Text = "World View Off";
                //StartCamera();
            }
            cameraStatus = !cameraStatus;

        }

        private void TakeQuizMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Quiz.xaml", UriKind.Relative));
        }

        private void GameMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Game.xaml", UriKind.Relative));
        }

        private void SettingsMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void RecordVideoMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/RecordVideo.xaml?r=ARPage.xaml", UriKind.Relative));
        }

        private void HelpMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Help.xaml?r=ARPage.xaml", UriKind.Relative));
        }

    }
}