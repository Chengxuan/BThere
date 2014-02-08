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
using System.Device.Location;
using Microsoft.Devices;
using System.IO.IsolatedStorage;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Info;

namespace BThere
{
    public partial class MainPage : PhoneApplicationPage
    {

        //Global Variables

        GeoCoordinateWatcher watcher; // Geo Watcher to get lat and lon

        Boolean locationFixed = false; // to check if location coordinates are fixed

        Boolean setupFinished = false; // to check if need more json calls to get the country

        Boolean gettingLocation = false; // flag to keep track of access to location services

        //IsolatedStorageHelper fileHelper = new IsolatedStorageHelper();

        IsolatedStorageFile myStorageArea =  null; //creating an isolated storage file object

        
        
        string folderPath = "BThereFolder";
        string filePath = "BThereFolder\\SetupLog.txt";
        


        public MainPage()
        {
            InitializeComponent();
        }

        //check sensor needed
        private bool CheckSensors()
        {
            //an accelerometer. A gyro as well makes the experience much better though
            if (Microsoft.Devices.Sensors.Motion.IsSupported)
            {
                //Warn user that without gyro, the experience isn't as good as it can get
                if (!Microsoft.Devices.Sensors.Gyroscope.IsSupported)
                {
                    if (App.firstRun)
                    {
                        ErrorTextBlock.Text += "- No gyro detected. Experience may be degraded\r\n";
                    }
                }
            }
            else //Bummer! 
            {
                ErrorTextBlock.Text += "Sorry - Motion sensor is not supported on this device. App cannot run!!!";
                return false;
            }
            return true;
        }

        //executes when navigating to this page
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            myStorageArea = IsolatedStorageFile.GetUserStoreForApplication(); //creating an isolated storage file object
            //fileHelper.foldername = "BThere";
            //fileHelper.filename = "setupfile.txt";
            if (setupFinished == false)
            {
                if (CheckSensors() == true)
                {
                    // Read startup values
                    ReadSetupLog();

                    if (App.firstRun)
                    {
                        object deviceID;

                        if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out deviceID))
                        {
                            byte[] bID = (byte[])deviceID;
                            deviceID = Convert.ToBase64String(bID);
                            App.UID = deviceID.ToString();
                        }
                        else
                        {
                            App.UID = "UID-hidden";
                        }

                        // Start location services
                        gettingLocation = true;
                        StartLocationService(GeoPositionAccuracy.High);
                        ContinueBTN.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        ContinueBTN.Content = "Continue...";
                        ContinueBTN.Visibility = System.Windows.Visibility.Visible;
                        ContinueBTN.IsEnabled = true;
                        refreshBTN.Visibility = System.Windows.Visibility.Visible;
                        CountryTextBlock.Text = App.countryName.ToString().Trim();
                        LatitudeTextBlock.Text = App.lat.ToString("0.00").Trim();
                        LongitudeTextBlock.Text = App.lon.ToString("0.00").Trim();
                        ErrorTextBlock.Text = "App will use your previous location. Click CONTINUE to use the app with previously known location or click REFRESH to get more accurate location details.";
                    }
                }
                else
                {
                    ContinueBTN.Content = "Can't Continue";
                }
            }
            else
            {
                ContinueBTN.IsEnabled = true;
            }
   
        }

        //executes when navigating away from the page
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {

            //do all the clean up
            
            // stop the geo watcher service to reduce the battery drain
            // good place to unhook any watcher events too...
            // also the storagearea

            if (watcher != null && setupFinished==false && gettingLocation==true)
            {
                watcher.StatusChanged -= watcher_StatusChanged;
                watcher.PositionChanged -= watcher_PositionChanged;
                watcher.Stop();
                watcher.Dispose();
            }

            if (myStorageArea != null)
            {
                myStorageArea.Dispose();
            }
        }
        
        //Read the setup file
        private void ReadSetupLog()
        {
            try
            {
                if (myStorageArea.FileExists(filePath))
                {
                    using (var myIS_Stream = new IsolatedStorageFileStream(filePath, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read, myStorageArea))
                    {
                        // create a stream reader to read data from the file
                        using (var myIS_FileRead = new System.IO.StreamReader(myIS_Stream))
                        {
                            App.countryName = Convert.ToString(myIS_FileRead.ReadLine());
                            App.lat = Convert.ToDouble(myIS_FileRead.ReadLine());
                            App.lon = Convert.ToDouble(myIS_FileRead.ReadLine());
                            App.pin = Convert.ToInt16(myIS_FileRead.ReadLine());
                            App.firstRun = Convert.ToBoolean(myIS_FileRead.ReadLine());
                            App.UID = Convert.ToString(myIS_FileRead.ReadLine());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Welcome\r\nAs this is your first time running this app, Please bear with few configuration steps.");
                }
            }
            catch (Exception e) //cannot read the legend file
            { throw e; }
        }

        //write the setup file
        private void SaveSetupLog()
        {

            
            try
            {
                //if exist else creating a folder
                if (myStorageArea.DirectoryExists(folderPath) == false)
                    myStorageArea.CreateDirectory(folderPath);

                //opening or creating a new file
                using (var myIS_Stream = new IsolatedStorageFileStream(filePath, System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, myStorageArea))
                {
                    // create a stream writer to write data to the file
                    using (var myIS_FileWrite = new System.IO.StreamWriter(myIS_Stream))
                    {
                        myIS_FileWrite.WriteLine(App.countryName);

                        myIS_FileWrite.WriteLine(App.lat.ToString());

                        myIS_FileWrite.WriteLine(App.lon.ToString());

                        myIS_FileWrite.WriteLine(App.pin.ToString());

                        myIS_FileWrite.WriteLine("false");

                        myIS_FileWrite.WriteLine(App.UID.ToString());

                    }
                }
            }
            catch (Exception e) { MessageBox.Show("Unable to save the file.\rSorry for that now"+e.Message, "BThere setup", MessageBoxButton.OK); }
    
            /*
            string[] fileContent = new string[] { App.countryName, App.lat.ToString(), App.lon.ToString(), App.pin.ToString(), "false" };

            int result = fileHelper.WriteToFile(fileContent);

            
            if (result != 0)
            {
                MessageBox.Show(fileHelper.writeResult.ToString());
            }
            else 
            {
                MessageBox.Show(fileHelper.writeResult.ToString());
            }
             * */
        }

        /// Helper method to start up the location data acquisition
        private void StartLocationService(GeoPositionAccuracy accuracy)
        {
            // Reinitialize the GeoCoordinateWatcher
            watcher = new GeoCoordinateWatcher(accuracy);

            watcher.MovementThreshold = 20;

            // Add event handlers for StatusChanged and PositionChanged events
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            // Start data acquisition
            watcher.Start();
        }

        /// Handler for the StatusChanged event. This invokes MyStatusChanged on the UI thread and passes the GeoPositionStatusChangedEventArgs
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyStatusChanged(e));

        }

        // Custom method called from the StatusChanged event handler
        void MyStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The location service is disabled or unsupported.
                    // Alert the user
                    ErrorTextBlock.Text += "\r\nThe A/GPS is disable. Please enable it.";
                    ContinueBTN.Content = "Can't Continue";
                    //ContinueBTN.IsEnabled = false;
                    locationFixed = false; // set the location found flag to false
                    break;
                case GeoPositionStatus.Initializing:
                    // The location service is initializing.
                    // Disable the Start Location button
                    locationFixed = false; // set the location found flag to false
                    break;
                case GeoPositionStatus.NoData:
                    // The location service is working, but it cannot get location data
                    // Alert the user and enable the Stop Location button
                    ErrorTextBlock.Text += "\r\nNo GPS data recieved. Can't Continue";
                    ContinueBTN.Content = "Can't Continue";
                    //ContinueBTN.IsEnabled = false;
                    locationFixed = false; // set the location found flag to false
                    break;
                case GeoPositionStatus.Ready:
                    // The location service is working and is receiving location data
                    // Show the current position and enable the Stop Location button
                    locationFixed = true;
                    //GetCountry();
                    break;
            }
        }

        // Handler for the PositionChanged event. This invokes MyStatusChanged on the UI thread and passes the GeoPositionStatusChangedEventArgs
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyPositionChanged(e));
        }

        // Custom method called from the PositionChanged event handler
        void MyPositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Update the TextBlocks to show the current location
            LatitudeTextBlock.Text = e.Position.Location.Latitude.ToString("0.00");
            LongitudeTextBlock.Text = e.Position.Location.Longitude.ToString("0.00");

            if (locationFixed == true)
            {
                App.lat = e.Position.Location.Latitude;
                App.lon = e.Position.Location.Longitude;
                watcher.Stop();
                if(setupFinished==false)
                    GetCountry();
            }
        }

        //check network availability
        public bool CheckNetwork()
        {
            return (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
        }

        //initiate a call to google reverse geocode service to retrieve country name based on the gps data
        public void GetCountry()
        {
            if (!CheckNetwork())
            {
                Uri serviceUri = new Uri("http://maps.googleapis.com/maps/api/geocode/json?latlng="+App.lat+","+App.lon+"&sensor=true");
                //Uri serviceUri = new Uri("http://maps.googleapis.com/maps/api/geocode/json?latlng=31.546110,74.340392&sensor=true");

                WebClient downloader = new WebClient();
                downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(downloader_OpenReadCompleted);
                downloader.OpenReadAsync(serviceUri);

            }
            else
            {
                CountryTextBlock.Text = App.countryName.ToString().Trim();
                ContinueBTN.Content = "Continue";
                ContinueBTN.IsEnabled = true;
                ErrorTextBlock.Text += "\r\nApplication cannot access network. Many features will be disabled. Please check your network.";
            }
        }

        //call back for the GetCountry
        void downloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                StreamReader reader = new StreamReader(e.Result);
                string JsonString = reader.ReadToEnd();

                RootObject deserializedJSON = JsonConvert.DeserializeObject<RootObject>(JsonString);

                var addresses = deserializedJSON.results.ToArray();

                if (addresses.Length > 0)
                {
                    var addressParts = addresses[0].formatted_address.ToString().Trim().Split(',');
                    if (addressParts.Length > 0)
                    {
                        App.countryName = addressParts[addressParts.Length - 1].ToString();

                        CountryTextBlock.Text = App.countryName.Trim();

                        
                        setupFinished = true;

                        ContinueBTN.Content = "Continue";
                        ContinueBTN.IsEnabled = true;
                    }
                    else
                    {
                        CountryTextBlock.Text = App.countryName.Trim();
                        ContinueBTN.Content = "Continue";
                        ContinueBTN.IsEnabled = true;
                        ErrorTextBlock.Text += "\r\nApplication cannot determine your location. Invalid JSON Response for address";
                    }
                }
                else
                {
                    CountryTextBlock.Text = App.countryName.Trim();
                    ContinueBTN.Content = "Continue";
                    ContinueBTN.IsEnabled = true;
                    ErrorTextBlock.Text += "\r\nApplication cannot determine your location. Empty JSON Response";
                }

            }
            catch
            {
                
                // cannot get json
                CountryTextBlock.Text = App.countryName.Trim();
                ContinueBTN.Content = "Continue";
                ContinueBTN.IsEnabled = true;
                ErrorTextBlock.Text += "\r\nApplication cannot determine your location.";
            }

            SaveSetupLog();

        }

        //continue button
        private void ContinueBTN_Click(object sender, RoutedEventArgs e)
        {
            var queryData = string.Format("?c={0}&lat={1}&lon={2}", App.countryName, App.lat, App.lon);
            NavigationService.Navigate(new Uri("/ARPage.xaml" + queryData, UriKind.Relative));
        }

        //refresh button
        private void refreshBTN_Click(object sender, RoutedEventArgs e)
        {
            gettingLocation = true;
            ContinueBTN.IsEnabled = false;
            ContinueBTN.Content = "Please wait";
            ErrorTextBlock.Text = "";
            StartLocationService(GeoPositionAccuracy.High);
        }

    }
}