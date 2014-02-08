using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using MobileServices.Sdk;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Samples.Phone.Storage;
using System.IO;
using Microsoft.Phone.Info;
using System.Windows.Media.Imaging;
using System.Net;
using System.Device.Location;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Threading;


namespace BThere
{
    public partial class RecordVideo : PhoneApplicationPage
    {
        public FileSink mySink { get; set; }
        public IsolatedStorageFileStream myStream { get; set; }
        public CaptureSource mySource { get; set; }
        public VideoBrush myBrush { get; set; }
        public static MobileServiceClient mobileServiceClient;
        MobileServiceTable<GlobalVideos> usrStatusTable = null;
        GlobalVideos item;
        Stream thumbnailStream = new MemoryStream();

        string locationString = null;
        GeoCoordinateWatcher watcher = null;
        bool locationFixed = false;
        bool setupFinished = false;
        double lat = 0.00;
        double lon = 0.00;
        DispatcherTimer timer = new DispatcherTimer();


        bool videoFileFlag = false, imageFileFlag = false;

        public RecordVideo()
        {
            InitializeComponent();

            errMsg.Text = "Initializing";
            StartLocationService(GeoPositionAccuracy.High);
        }



        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


        }


        public bool NoNetwork()
        {
            return (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
        }

        private void Record()
        {
            StartRecord_BTN.IsEnabled = false;
            if (NoNetwork() == false)
            {

                VideoCaptureDevice videoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();

                if (videoCaptureDevice != null)
                {

                    this.mySource = new CaptureSource();
                    this.mySource.CaptureImageCompleted += mySource_CaptureImageCompleted;

                    this.mySource.VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
                    this.mySink = new FileSink();

                    mobileServiceClient = new MobileServiceClient("https://bethere.azure-mobile.net/", "XdjbcQDgRsMEXdNKeiKoiILoWNWLJE48");
                    usrStatusTable = mobileServiceClient.GetTable<GlobalVideos>();
                }
                else
                {
                    StartRecord_BTN.Content = "No Camera Available";
                    StartRecord_BTN.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("A network connection can not be established.\r\nPlease press refresh or check your network settings.");
                return;
            }

            this.mySource.VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
            this.mySink = new FileSink();
            errMsg.Text = "";

            using (IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (isolatedStorageFile.FileExists("video.mp4"))
                {
                    isolatedStorageFile.DeleteFile("video.mp4");
                }
            }

            this.myBrush = new VideoBrush();
            this.cameraView.Background = this.myBrush;
            this.myBrush.SetSource(this.mySource);
            this.mySink.CaptureSource = this.mySource;
            this.mySink.IsolatedStorageFileName = "video.mp4";
            this.mySource.Start();
            this.mySource.CaptureImageAsync();
            DelayDisplay();
        }

        private void DelayDisplay()
        {
            timer.Tick +=
                 delegate(object s, EventArgs args)
                 {
                     timer.Stop();
                     StartRecord_BTN.IsEnabled = true;
                 };
            timer.Interval = new TimeSpan(0, 0, 2); // two seconds delay
            timer.Start();
        }

        void mySource_CaptureImageCompleted(object sender, CaptureImageCompletedEventArgs e)
        {
            e.Result.SaveJpeg(thumbnailStream, 75, 75, 0, 100);
        }

        private void StopRecord()
        {

            StartRecord_BTN.IsEnabled = false;

            string countryName = App.countryName.ToString().ToLower().Trim();

            countryName.Replace(" ", "");

            this.mySource.Stop();
            this.mySink.CaptureSource = null;
            this.mySink.IsolatedStorageFileName = null;
            this.myBrush = null;
            this.myStream = new IsolatedStorageFileStream("video.mp4", FileMode.Open, FileAccess.Read, IsolatedStorageFile.GetUserStoreForApplication());
            UploadVideo(App.countryName.ToString().ToLower().Trim(), App.lon, App.lat, this.myStream, thumbnailStream);
        }

        public void UploadVideo(string countryName, double longtitude, double latitude, Stream videoStream, Stream thumbnailStream)
        {
            errMsg.Text = "               Uploading...\r\nPlease wait until upload Finish!";

            // By using CloudStorageClientResolverAccountAndKey.DevelopmentStorageAccountResolver you can connect directly 
            // against the Windows Azure Storage Emulator.
            DateTime dateNow = DateTime.Now;

            string UniqeID = dateNow.Year.ToString() + "" + dateNow.Month.ToString() + "" + dateNow.Day.ToString() + "" + dateNow.Hour.ToString() + "" + dateNow.Minute.ToString() + "" + dateNow.Second.ToString() + "" + App.UID.ToString();

            // By using CloudStorageClientResolverAccountAndKey you can connect to your Windows Azure Storage Services account directly.
            // Just replace your Windows Azure Storage account credentials and endpoints.
            var resolver = new CloudStorageClientResolverAccountAndKey(
             new StorageCredentialsAccountAndKey("bethere", "Xg7EJNfeb0g7dJXX5aKFN2GN9oPpzZQSMeF+HwO+z/WFaIOE6gaV5QNsprpfWuV8pQcCvYryBpxRE6qpT9Yd5Q=="),
             new Uri("http://bethere.blob.core.windows.net"),
             new Uri("http://bethere.queue.core.windows.net"),
             new Uri("http://bethere.table.core.windows.net"),
             Deployment.Current.Dispatcher);

            CloudStorageContext.Current.Resolver = resolver;

            var blobClient = CloudStorageContext.Current.Resolver.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(countryName + "video");
            container.CreateIfNotExist(
                BlobContainerPublicAccessType.Container,
                Response =>
                {
                    var blobVideo = container.GetBlobReference(UniqeID + ".mp4");
                    var sampleStreamVideo = videoStream; //new MemoryStream(Encoding.UTF8.GetBytes("xuan"));

                    blobVideo.UploadFromStream(
                        sampleStreamVideo,
                        ResponseVideo =>
                        {
                            videoFileFlag = ResponseVideo.Success;

                            if (videoFileFlag)
                            {
                                var containerjpg = blobClient.GetContainerReference(countryName + "thumbnail");
                                containerjpg.CreateIfNotExist(
                                    BlobContainerPublicAccessType.Container,
                                    ResponseBlob =>
                                    {
                                        var blobImage = containerjpg.GetBlobReference(UniqeID + ".jpg");
                                        var sampleStreamImage = thumbnailStream; //new MemoryStream(Encoding.UTF8.GetBytes("xuan"));

                                        blobImage.UploadFromStream(
                                            sampleStreamImage,
                                            ReponseImage =>
                                            {
                                                imageFileFlag = ReponseImage.Success;

                                                if (imageFileFlag)
                                                {

                                                    //Save data to windows azure Mobile service
                                                    //create insert object
                                                    item = new GlobalVideos
                                                    {
                                                        Countryname = countryName,
                                                        Date = dateNow,
                                                        Latitude = latitude,
                                                        Longtitude = longtitude,
                                                        Location = locationString,
                                                        Videourl = "http://bethere.blob.core.windows.net/" + countryName + "video/" + UniqeID + ".mp4",
                                                        Thumbnailurl = "http://bethere.blob.core.windows.net/" + countryName + "thumbnail/" + UniqeID + ".jpg"
                                                    };

                                                    PopulateDBTable();
                                                    
                                                }
                                                else
                                                {
                                                    imageFileFlag = false;
                                                    errMsg.Text = "Failed!!!";
                                                    StartRecord_BTN.Content = "OK";
                                                    StartRecord_BTN.IsEnabled = true;
                                                }
                                                // Some logic here.
                                            });
                                    });
                            }
                            else
                            {
                                videoFileFlag = false;
                                imageFileFlag = false;
                                errMsg.Text = "Failed!!!";
                                StartRecord_BTN.Content = "OK";
                                StartRecord_BTN.IsEnabled = true;
                                //operation failed
                            }
                            // Some logic here.
                        });
                });
        }

        private void PopulateDBTable()
        {
            usrStatusTable = mobileServiceClient.GetTable<GlobalVideos>();
            //Save data to windows azure Mobile service
            //create insert object
            usrStatusTable.Insert(item, (res, err) =>   //do the Insert
            {
                if (err != null)
                {
                    //handle it
                    errMsg.Text = "Failed!!!";
                }
                else
                {
                    errMsg.Text = "Finish!!!";
                }
                item = res;
                StartRecord_BTN.Content = "OK";
                StartRecord_BTN.IsEnabled = true;
            });


        }


        private void StartRecord_BTN_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            switch ((string)button.Tag)
            {
                case "Stopped":
                    button.Content = "Tap to Stop";
                    button.Tag = "Recording";
                    //this.cameraView.Visibility = Visibility.Visible;
                    this.Record();
                    break;
                case "Recording":
                    button.Content = "Uploading";
                    button.Tag = "Uploaded";
                    //this.cameraView.Visibility = Visibility.Collapsed;
                    this.StopRecord();
                    break;
                case "Uploaded":
                    button.Tag = "Stopped";
                    this.GoBack();
                    break;
                default:
                    break;

            }
        }

        private void GoBack()
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new Uri("/ARPage.xaml", UriKind.Relative));
            }

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
                    //ErrorTextBlock.Text += "\r\nThe A/GPS is disable. Please enable it.";
                    //ContinueBTN.Content = "Can't Continue";
                    //ContinueBTN.IsEnabled = false;
                    locationString = App.countryName;
                    lat = App.lat;
                    lon = App.lon;
                    locationFixed = false; // set the location found flag to false
                    break;
                case GeoPositionStatus.Initializing:
                    // The location service is initializing.
                    // Disable the Start Location button
                    locationString = App.countryName;
                    lat = App.lat;
                    lon = App.lon;
                    locationFixed = false; // set the location found flag to false
                    break;
                case GeoPositionStatus.NoData:
                    // The location service is working, but it cannot get location data
                    // Alert the user and enable the Stop Location button
                    //ErrorTextBlock.Text += "\r\nNo GPS data recieved. Can't Continue";
                    //ContinueBTN.Content = "Can't Continue";
                    //ContinueBTN.IsEnabled = false;
                    locationString = App.countryName;
                    lat = App.lat;
                    lon = App.lon;
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
            if (locationFixed == true)
            {
                lat = e.Position.Location.Latitude;
                lon = e.Position.Location.Longitude;
                watcher.Stop();
                if (setupFinished == false)
                {
                    GetCountry();
                }
            }
        }

        //initiate a call to google reverse geocode service to retrieve country name based on the gps data
        public void GetCountry()
        {
            if (!NoNetwork())
            {
                Uri serviceUri = new Uri("http://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lon + "&sensor=true");
                //Uri serviceUri = new Uri("http://maps.googleapis.com/maps/api/geocode/json?latlng=31.546110,74.340392&sensor=true");

                WebClient downloader = new WebClient();
                downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(downloader_OpenReadCompleted);
                downloader.OpenReadAsync(serviceUri);

            }
            else
            {
                locationString = App.countryName;
            }
        }

        //call back for the GetCountry
        void downloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            setupFinished = true;

            try
            {
                StreamReader reader = new StreamReader(e.Result);
                string JsonString = reader.ReadToEnd();

                RootObject deserializedJSON = JsonConvert.DeserializeObject<RootObject>(JsonString);

                var addresses = deserializedJSON.results.ToArray();

                if (addresses.Length > 0)
                {
                    locationString = addresses[0].formatted_address.ToString().Trim();
                }
                else
                {
                    locationString = App.countryName.Trim();
                }

            }
            catch
            {
                locationString = App.countryName.Trim();
            }

            errMsg.Text = "";
            StartRecord_BTN.IsEnabled = true;

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
    }
}