using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Windows.Media.Imaging;
using SharpGIS.AR.Controls;
using Microsoft.Devices;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;


namespace BThere
{
    public partial class Game : PhoneApplicationPage
    {
        List<string> questionAlreadyAsked = new List<string>();
        int numberOfQuestionAlreadyAsked = 0;
        int numberOfQuestionGotRight = 0;
        public Microsoft.Devices.PhotoCamera m_camera;
        DispatcherTimer timer = new DispatcherTimer();

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

        public void start(SharpGIS.AR.Controls.ARPanel arPanel, bool judgeFlag, int areaMark, string countryName, TextBlock txtRightCount)
        {


            Image img = new Image();
            img.Source = new BitmapImage(new Uri("/Images/flags/" + countryName + ".png", UriKind.Relative));
            img.Width = 100;
            img.Height = 90;
            img.Tap += (a, b) =>
            {
                if (judgeFlag)
                {
                    txtRightCount.Text = "Score: " + (++numberOfQuestionGotRight).ToString() + "/" + App.allCoutries.Count.ToString();
                    arPanel.Children.Clear();
                    txtResult.Text = "Correct";
                    txtResult.Foreground = new SolidColorBrush(Colors.Red);
                    txtResult.Visibility = Visibility.Visible; // show textblock
                    DelayDisplay(); // delay message
                    DisplayQuestion();
                }
                else
                {
                    txtRightCount.Text = "Score: " + numberOfQuestionGotRight.ToString() + "/" + App.allCoutries.Count.ToString();
                    arPanel.Children.Clear();
                    txtResult.Text = "Wrong";
                    txtResult.Foreground = new SolidColorBrush(Colors.Green);
                    txtResult.Visibility = Visibility.Visible; // show textblock
                    DelayDisplay();  // delay message
                    DisplayQuestion();
                }
            };
            ARPanel.SetDirection(img, new System.Windows.Point(-45, 0 + areaMark * 72));
            arPanel.Children.Add(img);
        }


        public Game()
        {
            InitializeComponent();
            videoBrushTransform.Rotation = 90;
            DisplayQuestion();
        }

        //to-do for cheng
        /*
         * Get five random countries...
         * on of them is the question itself
         * you don't have to put them where they are on the earth
         * just in different directions
         * display a dialoug e.g "what is the flag of the country"
         * display 5 flags...
         * clickable images
         * when they click wrong image...
         * display wrong
         * else right
         * start all over again...
         * no scores and nothing
         * 
         * you can get countries list from App.allCoutries
         */

        private void DelayDisplay()
        {
            timer.Tick +=
                 delegate(object s, EventArgs args)
                 {
                     timer.Stop();
                     txtResult.Visibility = Visibility.Collapsed;
                 };
            timer.Interval = new TimeSpan(0, 0, 2); // two seconds delay
            timer.Start();
        }

        private void DisplayQuestion()
        {
            numberOfQuestionAlreadyAsked++;
            if (numberOfQuestionAlreadyAsked <= App.allCoutries.Count)
            {
                var questionToAsk = GetQuestionData();
                questionTXT.Text = "Which is the flag of " + questionToAsk[0].CountryLongName.ToString() + "?";
                Random rand = new Random();
                int temp = rand.Next(0, 4);
                for (int i = 0; i < 5; i++)
                {

                    if (temp % 5 == 0)
                    {
                        start(arPanel, true, i, questionToAsk[0].CountryShortName, txtRightCount);
                        temp++;
                    }
                    else
                    {
                        start(arPanel, false, i, questionToAsk[temp % 5].CountryShortName, txtRightCount);
                        temp++;
                    }
                }
            }
            else
            {
                btnReplay.Visibility = Visibility.Visible;
            }

        }

        //gives you a country data which hasn't been displayed before
        private CountryDetails[] GetQuestionData()
        {

            bool uniqueFound = false;
            CountryDetails[] uniqueQuestion = new CountryDetails[5];
            int countryListItemIndex = 0;

            while (uniqueFound == false)
            {
                foreach (CountryDetails country in App.allCoutries)
                {
                    if (!questionAlreadyAsked.Contains(country.CountryLongName.ToString()))
                    {
                        uniqueFound = true;
                        uniqueQuestion[0] = country;
                        questionAlreadyAsked.Add(country.CountryLongName.ToString());
                        countryListItemIndex++;
                        break;
                    }
                    else
                    {
                        uniqueQuestion[0] = null;
                    }
                }
            }

            int totalElements = App.allCoutries.Count;

            bool allGuessesFound = false;

            List<int> alreadyChosen = new List<int>();
            int tempCounter = 1;

            while (allGuessesFound == false)
            {
                Random rand = new Random();
                int temp = rand.Next(0, totalElements - 1);
                if (alreadyChosen.Contains(temp) == false && temp != countryListItemIndex)
                {
                    alreadyChosen.Add(temp);
                    uniqueQuestion[tempCounter] = App.allCoutries.ElementAt(temp);
                    tempCounter++;
                }

                if (tempCounter > 4)
                    allGuessesFound = true;
            }

            return uniqueQuestion;
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

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // Dispose camera to minimize power consumption and to expedite shutdown.

            m_camera.Dispose();

            // Good place to unhook camera event handlers too.
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            var data = this.NavigationContext.QueryString;

            StartCamera();

            base.OnNavigatedTo(e);
        }


        private void StartCamera()
        {
            m_camera = new Microsoft.Devices.PhotoCamera();
            viewfinderBrush.SetSource(m_camera);
        }

        private void btnReplay_Click(object sender, RoutedEventArgs e)
        {
            btnReplay.Visibility = Visibility.Collapsed;
            questionAlreadyAsked.Clear();
            numberOfQuestionAlreadyAsked = 0;
            numberOfQuestionGotRight = 0;
            DisplayQuestion();
            txtRightCount.Text = "Score: 0/" + App.allCoutries.Count.ToString();
        }
       

    }
}