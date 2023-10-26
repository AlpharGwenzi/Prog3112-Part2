using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading; //  namespace for the timer

namespace Prog3112
{
    public partial class PartTwoo : Window
    {
        private int score = 0;
        private DispatcherTimer gameTimer;
        private int timeLimit = 60;

        private Random random = new Random();
        private ObservableCollection<string> droppedItems = new ObservableCollection<string>();
        private string currentlySelectedAnswer;
        private List<string> selectedAnswers = new List<string>();

        public PartTwoo()
        {
            InitializeComponent();
            InitializeGameTimer();
        }

        // Initialize the game timer
        private void InitializeGameTimer()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;
        }

        // Handle the timer tick event
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            timeLimit--;
            TimeDisplay.Text = $"Time: {timeLimit} seconds";

            // Check if the time has run out
            if (timeLimit == 0)
            {
                gameTimer.Stop();
                CheckAnswers();
            }
        }

        // Start a new game
        private void OrganizeNumbers_Click(object sender, RoutedEventArgs e)
        {
            // Initialize or reset game variables
            score = 0;
            ScoreDisplay.Text = "Score: 0";
            timeLimit = 60;
            TimeDisplay.Text = "Time: 60 seconds";
            gameTimer.Start();

            selectedAnswers.Clear();
            droppedItems.Clear();
            AnswersDrop.ItemsSource = droppedItems;
            AnswersDrop.AllowDrop = true;

            AnswersListBox.SelectedIndex = -1;

            // Generate random numbers for the game
            List<string> numbers = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                int randomNumber = random.Next(100, 1000);
                string randomText = RandomString(3);
                int randomNumbers = random.Next(10, 99);
                string randomTexts = RandomString(2);
                string formattedNumber = $"{randomNumber:D3}.{randomNumbers:D2} {randomText} ";
                numbers.Add(formattedNumber);
            }

            Questions.ItemsSource = numbers;

            // Create a dictionary to categorize numbers
            Dictionary<int, string> numberToCategory = new Dictionary<int, string>();
            foreach (var number in numbers)
            {
                int firstThreeDigits = int.Parse(number.Substring(0, 3));
                string category = CategorizeNumber(firstThreeDigits);
                numberToCategory[firstThreeDigits] = category;
            }

            List<string> correctCategories = new List<string>();
            foreach (var kvp in numberToCategory)
            {
                correctCategories.Add(kvp.Value);
            }

            // Prepare the answers with a mix of correct and incorrect categories
            List<string> answers = new List<string>();
            answers.AddRange(correctCategories);

            List<string> allCategories = new List<string> { "General Knowledge", "Philosophy & psychology", "Religion", "Social Sciences", "Languages", "Science", "Technology", "Arts & recreation", "Literature", "History & geography" };
            List<string> wrongCategories = new List<string>();
            foreach (var category in allCategories)
            {
                if (!correctCategories.Contains(category))
                {
                    wrongCategories.Add(category);
                }
                if (wrongCategories.Count >= 3)
                {
                    break;
                }
            }

            answers.AddRange(wrongCategories);
            answers = answers.OrderBy(x => random.Next()).ToList();
            AnswersListBox.ItemsSource = answers;
        }

        // Categorize a number based on its first three digits
        private string CategorizeNumber(int firstThreeDigits)
        {
            if (firstThreeDigits >= 0 && firstThreeDigits <= 99)
            {
                return "General Knowledge";
            }
            else if (firstThreeDigits >= 100 && firstThreeDigits <= 199)
            {
                return "Philosophy & psychology";
            }
            else if (firstThreeDigits >= 200 && firstThreeDigits <= 299)
            {
                return "Religion";
            }
            else if (firstThreeDigits >= 300 && firstThreeDigits <= 399)
            {
                return "Social Sciences";
            }
            else if (firstThreeDigits >= 400 && firstThreeDigits <= 499)
            {
                return "Languages";
            }
            else if (firstThreeDigits >= 500 && firstThreeDigits <= 599)
            {
                return "Science";
            }
            else if (firstThreeDigits >= 600 && firstThreeDigits <= 699)
            {
                return "Technology";
            }
            else if (firstThreeDigits >= 700 && firstThreeDigits <= 799)
            {
                return "Arts & recreation";
            }
            else if (firstThreeDigits >= 800 && firstThreeDigits <= 899)
            {
                return "Literature";
            }
            else
            {
                return "History & geography";
            }
        }

        // Generate a random string of characters
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Handle mouse left button down event for drag and drop
        private void AnswersListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null && listBox.SelectedItem != null)
            {
                currentlySelectedAnswer = listBox.SelectedItem as string;
                DragDrop.DoDragDrop(listBox, listBox.SelectedItem, DragDropEffects.Move);
            }
        }

        // Handle drag enter event for the drop area
        private void AnswersDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)) && droppedItems.Count < 4)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        // Handle drag over event for the drop area
        private void AnswersDrop_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)) && droppedItems.Count < 4)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        // Handle drop event for the drop area
        private void AnswersDrop_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)) && droppedItems.Count < 4)
            {
                string droppedText = (string)e.Data.GetData(typeof(string));

                if (selectedAnswers.Contains(droppedText))
                {
                    return;
                }

                selectedAnswers.Add(droppedText);
                droppedItems.Add(droppedText);
                AnswersDrop.ItemsSource = droppedItems;
            }
            else if (e.Data.GetDataPresent(typeof(string)))
            {
                string droppedText = (string)e.Data.GetData(typeof(string));

                // If the item is dropped back to AnswersDrop, remove it from there
                if (droppedItems.Contains(droppedText))
                {
                    droppedItems.Remove(droppedText);
                    AnswersDrop.ItemsSource = droppedItems;
                }

                // Add it back to AnswersListBox
                AnswersListBox.Items.Add(droppedText);
            }
        }

        // Check the answers and display the result
        private void CheckAnswers()
        {
            List<string> droppedItemsList = new List<string>(droppedItems);
            List<string> questions = new List<string>();
            foreach (string item in Questions.Items)
            {
                questions.Add(item);
            }

            int correctAnswers = CalculateCorrectAnswers(questions, droppedItemsList);

            score += correctAnswers;
            ScoreDisplay.Text = $"Score: {score}";

            if (correctAnswers == 4)
            {
                MessageBox.Show("Perfect score! You completed the game!", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Your answers are incorrect.", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Stop the timer after checking answers
            gameTimer.Stop();
        }

        // Handle the check answers button click event
        private void CheckAnswers_Click(object sender, RoutedEventArgs e)
        {
            CheckAnswers();
        }

        // Calculate the number of correct answers
        private int CalculateCorrectAnswers(List<string> questions, List<string> answers)
        {
            int correctAnswers = 0;

            for (int i = 0; i < questions.Count; i++)
            {
                string questionFirstThree = questions[i].Substring(0, 3);
                string answerFirstThree = answers[i].Substring(0, 3);

                if (questionFirstThree.Equals(answerFirstThree, StringComparison.OrdinalIgnoreCase))
                {
                    correctAnswers++;
                }
            }

            return correctAnswers;
        }
    }
}
