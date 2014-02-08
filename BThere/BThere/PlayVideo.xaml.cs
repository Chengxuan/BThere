using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;

namespace BThere
{
    public partial class PlayVideo : PhoneApplicationPage
    {
        IsolatedStorageFileStream isolatedStorageFileStream;
        IsolatedStorageFile isolatedStorageFile;

        public PlayVideo()
        {
            InitializeComponent();
            this.Loaded += PlayVideo_Loaded;
        }


        public bool NoNetwork()
        {
            return (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
        }

        void PlayVideo_Loaded(object sender, RoutedEventArgs e)
        {
            if (NoNetwork() == false)
            {
                DownloadVideo();
            }
            else
            {
                MessageBox.Show("A network connection can not be established.\r\nPlease press refresh or check your network settings.");
                return;
            }

        }

        private void DownloadVideo()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
            webClient.OpenReadAsync(new Uri(App.videoURL));
        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                if (progressMedia.Value <= progressMedia.Maximum)
                {
                    progressMedia.Value = (double)e.ProgressPercentage;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected bool IncreaseIsolatedStorageSpace(long quotaSizeDemand)
        {
            bool CanSizeIncrease = false;
            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();

            //Get the Available space
            long maxAvailableSpace = isolatedStorageFile.AvailableFreeSpace;
            if (quotaSizeDemand > maxAvailableSpace)
            {
                if (!isolatedStorageFile.IncreaseQuotaTo(isolatedStorageFile.Quota + quotaSizeDemand))
                {
                    CanSizeIncrease = false;
                    return CanSizeIncrease;
                }
                CanSizeIncrease = true;
                return CanSizeIncrease;
            }
            return CanSizeIncrease;
        }

        void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {

                    #region Isolated Storage Copy Code
                    isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();

                    bool checkQuotaIncrease = IncreaseIsolatedStorageSpace(e.Result.Length);

                    string VideoFile = "PlayFile.mp4";
                    isolatedStorageFileStream = new IsolatedStorageFileStream(VideoFile, FileMode.Create, isolatedStorageFile);
                    long VideoFileLength = (long)e.Result.Length;
                    byte[] byteImage = new byte[VideoFileLength];
                    e.Result.Read(byteImage, 0, byteImage.Length);
                    isolatedStorageFileStream.Write(byteImage, 0, byteImage.Length);

                    #endregion

                    mediaFile.SetSource(isolatedStorageFileStream);
                    mediaFile.Play();
                    progressMedia.Visibility = Visibility.Collapsed;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mediaFile_MediaEnded(object sender, RoutedEventArgs e)
        {
            Replay();
        }

        private void Replay()
        {
            MessageBoxResult res = MessageBox.Show("Do you want to Replay the file", "Decide", MessageBoxButton.OKCancel);

            if (res == MessageBoxResult.OK)
            {
                mediaFile.Play();
            }
            else
            {
                isolatedStorageFileStream.Close();
                isolatedStorageFile.Dispose();
                mediaFile.ClearValue(MediaElement.SourceProperty);
                NavigationService.GoBack();
            }
        }

    }
}