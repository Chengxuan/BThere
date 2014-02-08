using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;

namespace BThere
{
    public partial class Quiz : PhoneApplicationPage
    {
        List<string> questionAlreadyAsked = new List<string>();
        RadioButton alreadySelected = new RadioButton();
        int numberOfQuestionAlreadyAsked = 0;
        int numberOfQuestionGotRight = 0;
        string rightAnswer = "";
        DispatcherTimer timer = new DispatcherTimer();
        public Quiz()
        {
            InitializeComponent();
        }

        private void DelayDisplay()
        {
            timer.Tick +=
                 delegate(object s, EventArgs args)
                 {
                     timer.Stop();
                     txtResult.Visibility = Visibility.Collapsed;
                 };
            timer.Interval = new TimeSpan(0, 0, 1); // one seconds delay
            timer.Start();
        }

        private void DisplayQuestion()
        {
            numberOfQuestionAlreadyAsked++;
            submit.IsEnabled = false;
            if (numberOfQuestionAlreadyAsked <= App.allCoutries.Count)
            {
                if (alreadySelected != null)
                {
                    alreadySelected.IsChecked = false;
                    alreadySelected = null;
                }

                var questionToAsk = GetQuestionData();


                questionTXT.Text = "What is the capital of " + questionToAsk[0].CountryLongName.ToString() + "?";


                Random rand = new Random();

                int temp = rand.Next(0, 4);
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        ans1TXT.Content = questionToAsk[temp % 5].CountryCapital.ToString();
                        temp++;
                    }
                    if (i == 1)
                    {
                        ans2TXT.Content = questionToAsk[temp % 5].CountryCapital.ToString();
                        temp++;
                    }
                    if (i == 2)
                    {
                        ans3TXT.Content = questionToAsk[temp % 5].CountryCapital.ToString();
                        temp++;
                    }
                    if (i == 3)
                    {
                        ans4TXT.Content = questionToAsk[temp % 5].CountryCapital.ToString();
                        temp++;
                    }
                    if (i == 4)
                    {
                        ans5TXT.Content = questionToAsk[temp % 5].CountryCapital.ToString();
                        temp++;
                    }
                }


                flag.Source = new BitmapImage(new Uri("/Images/flags/" + questionToAsk[0].CountryShortName + ".png", UriKind.Relative));


                rightAnswer = questionToAsk[0].CountryCapital.ToString();
            }
            else
            {

                if (alreadySelected != null)
                {
                    string guess = alreadySelected.Content.ToString();

                    if (guess.ToString() == rightAnswer)
                    {
                        txtResult.Text = "Correct";
                        txtResult.Foreground = new SolidColorBrush(Colors.Red);
                        txtResult.Visibility = Visibility.Visible;
                        DelayDisplay();
                        numberOfQuestionGotRight++;
                    }
                    else
                    {
                        txtResult.Text = "Wrong";
                        txtResult.Foreground = new SolidColorBrush(Colors.Green);
                        txtResult.Visibility = Visibility.Visible;
                        DelayDisplay();
                    }
                    alreadySelected.IsChecked = false;
                    alreadySelected = null;
                    score.Text = "Score: " + numberOfQuestionGotRight.ToString() + "/" + App.allCoutries.Count.ToString();
                }
                submit.Content = "Redo";
                submit.Click += submitredo_Click;
                submit.IsEnabled = true;

            }

        }

        private void submitredo_Click(object sender, RoutedEventArgs e)
        {
            submit.Content = "Check Answer";
            submit.Click += submit_Click;
            questionAlreadyAsked.Clear();
            numberOfQuestionAlreadyAsked = 0;
            numberOfQuestionGotRight = 0;
            DisplayQuestion();
            score.Text = "Score: 0/" + App.allCoutries.Count.ToString();

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

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            if (alreadySelected != null)
            {
                string guess = alreadySelected.Content.ToString();

                if (guess.ToString() == rightAnswer)
                {
                    txtResult.Text = "Correct";
                    txtResult.Foreground = new SolidColorBrush(Colors.Red);
                    txtResult.Visibility = Visibility.Visible;
                    DelayDisplay();
                    numberOfQuestionGotRight++;
                }
                else
                {
                    txtResult.Text = "Wrong";
                    txtResult.Foreground = new SolidColorBrush(Colors.Green);
                    txtResult.Visibility = Visibility.Visible;
                    DelayDisplay();
                }

                score.Text = "Score: " + numberOfQuestionGotRight.ToString() + "/" + App.allCoutries.Count.ToString();

                DisplayQuestion();
            }
        }

        private void ansTXT_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton clicked = sender as RadioButton;
            if (alreadySelected != null)
            {
                if (!(alreadySelected == clicked))
                {
                    alreadySelected.IsChecked = false;
                    alreadySelected = clicked;
                }
            }
            else
            {
                alreadySelected = clicked;
            }
            submit.IsEnabled = true;
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            DisplayQuestion();
            txtResult.Text = "Start";
            txtResult.Foreground = new SolidColorBrush(Colors.Red);
            txtResult.Visibility = Visibility.Visible;
            DelayDisplay();
            base.OnNavigatedTo(e);
        }

    }
}