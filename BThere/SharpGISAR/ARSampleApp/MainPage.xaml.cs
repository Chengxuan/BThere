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

namespace ARSampleApp
{
	public partial class MainPage : PhoneApplicationPage
	{
        public Microsoft.Devices.PhotoCamera m_camera ;

		// Constructor
		public MainPage()
		{
			InitializeComponent();
            m_camera = new Microsoft.Devices.PhotoCamera();
            viewfinderBrush.SetSource(m_camera);

			//Add some more elements to the panel besides the ones already added in XAML
			//AddHeadingDots(arPanel);

            XDocument loadedData = XDocument.Load("res/CountryList.xml");

            var data = from query in loadedData.Descendants("CountryDetails")
                       select new CountryItems
                       {
                           name = (string)query.Element("Country"),
                           lat = Convert.ToDouble((string)query.Element("Latitude")),
                           lon = Convert.ToDouble((string)query.Element("Longitude")),
                       };


            foreach (CountryItems country in data)
            {
                pointVirtual(arPanel, country.name, 53, -8, country.lat, country.lon );
            }


            
		}

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);


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
                case PageOrientation.PortraitDown:
                    videoBrushTransform.Rotation = 270;
                    break;
            }
        }

        private void pointVirtual(SharpGIS.AR.Controls.ARPanel panel, string countryName, double myLatitude, double myLongtitude, double hisLatitude, double hisLongtitude)
        {
            double virtualLong = 0;
            double virtualLa = 0;
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
                            if (myLatitude-hisLatitude < 0)
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
            countryNameBTN.Foreground = new SolidColorBrush(Colors.Red);
            countryNameBTN.Click += countryNameBTN_Click;
            countryNameBTN.BorderBrush = new SolidColorBrush(Colors.Transparent);
            countryNameBTN.BorderThickness = new Thickness(0);
            
            SharpGIS.AR.Controls.ARPanel.SetDirection(countryNameBTN, new Point(virtualLong, virtualLa));

            panel.Children.Add(countryNameBTN);
        }

        void hp_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void countryNameBTN_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        
		private void ARPanel_Loaded(object sender, RoutedEventArgs e)
		{
			var panel = sender as SharpGIS.AR.Controls.ARPanel;
			//For motion to be supported, device needs at least a compass and
			//an accelerometer. A gyro as well makes the experience much better though
			if (Microsoft.Devices.Sensors.Motion.IsSupported)
			{
				//Warn user that without gyro, the experience isn't as good as it can get
				if (!Microsoft.Devices.Sensors.Gyroscope.IsSupported)
				{
					LayoutRoot.Children.Add(new TextBlock()
					{
						Text = "No gyro detected. Experience may be degraded",
						TextWrapping = System.Windows.TextWrapping.Wrap,
						VerticalAlignment = System.Windows.VerticalAlignment.Bottom
					});
				}
				//Start the AR PAnel
				panel.Start();
			}
			else //Bummer! 
			{
				panel.Visibility = System.Windows.Visibility.Collapsed;
				MessageBox.Show("Sorry - Motion sensor is not supported on this device");
			}
		}

		private void ARPanel_Unloaded(object sender, RoutedEventArgs e)
		{
			//Remember to stop the motion sensor when leaving
			(sender as SharpGIS.AR.Controls.ARPanel).Stop();
		}

		private void AddHeadingDots(SharpGIS.AR.Controls.ARPanel panel)
		{
			//Go 360 along the horizon to display heading text
			for (double heading = 20; heading < 360; heading += 20)
			{
				if (heading % 90 == 0) continue; //skip cardinal directions, since we already added markers for those in xaml
				TextBlock tb = new TextBlock() { Text = string.Format("{0}°", heading) };
				SharpGIS.AR.Controls.ARPanel.SetDirection(tb, new Point(0, heading));
				panel.Children.Add(tb);
			}
			//Display an up/down angle for each cardinal direction
			for (int i = 0; i < 360; i += 90)
			{
				for (int azimuth = -80; azimuth < 90; azimuth += 10)
				{
					if (azimuth == 0) continue; //skip cardinal directions, since we already added markers for those in xaml
					TextBlock tb = new TextBlock() { Text = string.Format("{0}", azimuth) };
					SharpGIS.AR.Controls.ARPanel.SetDirection(tb, new Point(azimuth, i));
					panel.Children.Add(tb);
				}
			}

            TextBlock tb1 = new TextBlock() { Text = "UK" };
            SharpGIS.AR.Controls.ARPanel.SetDirection(tb1, new Point(51.500622, -0.126662));
            panel.Children.Add(tb1);
		}

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // Dispose camera to minimize power consumption and to expedite shutdown.
            m_camera.Dispose();

            // Good place to unhook camera event handlers too.
        }
	}
}