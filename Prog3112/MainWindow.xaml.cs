using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Prog3112
{
    public partial class MainWindow : Window
    {
        //Below are my variables
       
       
        public List<string> callNumbers = new List<string>();
        private int score = 0;
        private int secondsLeft = 180;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool isDragging = true;
        private Point startPoint;
        private int draggedIndex = -1;
        private List<string> sortedCallNumbers = new List<string>();


        //---------------------------------------------------------------------------------------------------------------//
        public MainWindow()
        {
            InitializeComponent();

            // Initialize timer
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            // this  event handlers are for drag-and-drop
            BooksListView.AllowDrop = true;

            BooksListView.PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
            BooksListView.PreviewMouseMove += ListView_PreviewMouseMove;

            BooksListView.Drop += ListView_Drop;

            BooksListView2.AllowDrop = true;

            BooksListView2.PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
            BooksListView2.PreviewMouseMove += ListView_PreviewMouseMove;

            BooksListView2.Drop += ListView_Drop;


            callNumbersListBox.PreviewMouseLeftButtonDown += CallNumber_MouseLeftButtonDown;
            callNumbersListBox.PreviewMouseMove += CallNumber_MouseMove;
            callNumbersListBox.PreviewMouseLeftButtonUp += CallNumber_MouseLeftButtonUp;
            callNumbersListBox.Drop += CallNumbersListBox_Drop;

            
        }

        private ListViewItem _draggedItem;
        private Point _startPoint;

        //---------------------------------------------------------------------------------------------------------------//
        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store initial mouse click position and the item being dragged
            _startPoint = e.GetPosition(null);
            _draggedItem = GetListViewItem(e.OriginalSource as DependencyObject);
        }

        private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Check if the user is dragging an item and meets drag criteria
            if (e.LeftButton == MouseButtonState.Pressed && _draggedItem != null)
            {
                Point currentPosition = e.GetPosition(null);
                if (Math.Abs(currentPosition.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPosition.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    // Start dragging the item
                    DragDrop.DoDragDrop(_draggedItem, _draggedItem, DragDropEffects.Move);
                }
            }
        }

        private ListViewItem GetListViewItem(DependencyObject obj)
        {
            // Retrieve the ListViewItem from the visual tree
            while (obj != null && !(obj is ListViewItem))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }
            return obj as ListViewItem;
        }


        private void ListView_Drop(object sender, DragEventArgs e)
        {
            // Handle item drop in ListView
            ListView destinationListView = sender as ListView;

            if (destinationListView != null && _draggedItem != null)
            {
                if (e.Effects == DragDropEffects.Move)
                {
                    string draggedText = _draggedItem.Content as string;

                    if (draggedText != null)
                    {
                        // Determine the source ListView and remove the item
                        ListView sourceListView = (_draggedItem.Parent as ListView);
                        if (sourceListView != null)
                        {
                            sourceListView.Items.Remove(draggedText);
                        }

                        // Add the item to the destination ListView
                        destinationListView.Items.Add(draggedText);
                    }
                }
            }

            _draggedItem = null;
        }


        //---------------------------------------------------------------------------------------------------------------//
        private void ReplaceBooks_Click(object sender, RoutedEventArgs e)
        {
            // Generate 10 random call numbers and display them
            callNumbers = GenerateRandomCallNumbers(10);
            DisplayCallNumbers();

            // Enable user interaction for reordering
            BooksListView.IsEnabled = true;

            // Start the timer
            timer.Start();
        }
//---------------------------------------------------------------------------------------------------------------//
        private void DisplayCallNumbers()
        {
            // Display the generated call numbers in the ListView

            BooksListView.ItemsSource = callNumbers;
        }

        private List<string> GenerateRandomCallNumbers(int count)
        {
            // Generate and return random call numbers
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                double randomDouble = rand.NextDouble();
                string callNumber = (randomDouble * 1000).ToString("F2"); // Generates numbers like 123.45

                // Add 3 different random letters
                string randomLetters = GenerateDifferentRandomLetters(3, rand);
                callNumber = $"{callNumber} {randomLetters}";

                callNumbers.Add(callNumber);
            }


            return callNumbers;
        }


        private string GenerateDifferentRandomLetters(int count, Random rand)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] randomChars = new char[count];

            for (int i = 0; i < count; i++)
            {
                char randomChar;
                do
                {
                    randomChar = letters[rand.Next(letters.Length)];
                } while (Array.Exists(randomChars, c => c == randomChar));

                randomChars[i] = randomChar;
            }

            return new string(randomChars);
        }

        //---------------------------------------------------------------------------------------------------------------//



        private void ChechingOrder_Click()
        {
            SortCallNumbersNumerically();
            // Get the reordered list from the ListBox
            List<string> reorderedCallNumbers = new List<string>();
            foreach (var item in BooksListView.Items)
            {
                reorderedCallNumbers.Add(item.ToString());
            }

            // Check if the user's order is in ascending order
            bool isCorrectOrder = IsAscendingOrder(reorderedCallNumbers);

            // Display feedback to the user
            if (isCorrectOrder)
            {
                MessageBox.Show("Ordering is correct!", "Result", MessageBoxButton.OK, MessageBoxImage.Information);

                // Calculate the score
                score = CalculateScore();

               
            }
            else
            {
                MessageBox.Show("Ordering is incorrect!", "Result", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //---------------------------------------------------------------------------------------------------------------//

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the timer display and check for timeout
            if (secondsLeft > 0)
            {
                secondsLeft--;
                timeLeftTextBlock.Text = secondsLeft.ToString();

                // Update the score continuously
                score = CalculateScore();
                scoreTextBlock.Text = $"Score: {score}";
            }
            else
            {
                timer.Stop();
                ChechingOrder_Click(); // Call the CheckOrder method directly
            }
        }


        //---------------------------------------------------------------------------------------------------------------//

        private int CalculateScore()
        {
            // Check if the user's order is in ascending order
            bool isCorrectOrder = IsAscendingOrder(callNumbers);

            if (isCorrectOrder)
            {
                return 100; // 100 points for a correct order
            }
            else
            {
                return 0; // 0 points for an incorrect order
            }
        }



        //---------------------------------------------------------------------------------------------------------------//

        private void FindCallNumbers_Click(object sender, RoutedEventArgs e)
        {
            // Find books with similar topic numbers or author abbreviations
            string prescribedCallNumber = "005.73 JAM";

            string[] prescribedParts = prescribedCallNumber.Split(' ');
            string prescribedTopicNumber = prescribedParts[0];
            string prescribedAuthorAbbreviation = prescribedParts[1];

            List<string> foundBooks = new List<string>();

            foreach (string book in sortedCallNumbers)
            {
                string[] parts = book.Split(' ');
                string topicNumber = parts[0];
                string authorAbbreviation = parts[1];

                if (topicNumber == prescribedTopicNumber || authorAbbreviation == prescribedAuthorAbbreviation)
                {
                    foundBooks.Add(book);
                }
            }

            if (foundBooks.Count > 0)
            {
                string message = "Found books with similar topic numbers or author abbreviations:\n";
                foreach (string foundBook in foundBooks)
                {
                    message += foundBook + "\n";
                }

                MessageBox.Show(message, "Found Books", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No matching books found.", "Found Books", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        //---------------------------------------------------------------------------------------------------------------//

        private bool IsAscendingOrder(List<string> callNumbers)
        {
            // Check if the list of call numbers is in ascending order
            for (int i = 1; i < callNumbers.Count; i++)
            {
                if (string.Compare(callNumbers[i - 1], callNumbers[i], StringComparison.Ordinal) > 0)
                {
                    return false;
                }
            }
            return true;
        }

//---------------------------------------------------------------------------------------------------------------//

        private void CallNumber_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Handle mouse button release during drag-and-drop
            ListBox listBox = sender as ListBox;
            if (listBox == null || !isDragging)
                return;

            isDragging = true;
            listBox.ReleaseMouseCapture();
        }
        //--------------------------------------------------------------------------------------------------------------------//
        private void CallNumber_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Handle mouse button down during drag-and-drop
            ListBox listBox = sender as ListBox;
            if (listBox == null)
                return;

            startPoint = e.GetPosition(null);
            draggedIndex = GetNearestIndex(listBox, startPoint);
            if (draggedIndex >= 0)
            {
                isDragging = true;
                listBox.CaptureMouse();
            }
        }
        //---------------------------------------------------------------------------------------------------------------//

        private int GetNearestIndex(ListBox listBox, Point position)
        {
            // Find the nearest index based on the mouse position
            int index = -1;
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                ListBoxItem item = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[i]);
                if (item != null)
                {
                    Point itemPosition = item.TranslatePoint(new Point(0, 0), listBox);
                    if (Math.Abs(itemPosition.Y - position.Y) < item.ActualHeight / 2)
                    {
                        index = i;
                        break;
                    }
                }
            }
            if (index == -1)
                index = listBox.Items.Count;

            return index;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------//
        private void CallNumber_MouseMove(object sender, MouseEventArgs e)
        {
            // Handle mouse movement during drag-and-drop
            ListBox listBox = sender as ListBox;
            if (listBox == null || !isDragging)
                return;

            Point currentPoint = e.GetPosition(null);

            if (Math.Abs(startPoint.X - currentPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(startPoint.Y - currentPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                isDragging = true;
                listBox.ReleaseMouseCapture();

                if (draggedIndex >= 0)
                {
                    DataObject data = new DataObject("ListBoxItem", draggedIndex);
                    DragDrop.DoDragDrop(listBox, data, DragDropEffects.Move);
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------------//

        private void CallNumbersListBox_Drop(object sender, DragEventArgs e)
        {
            // Handle item drop in the callNumbersListBox
            ListBox listBox = sender as ListBox;
            if (listBox == null || draggedIndex < 0)
                return;

            int targetIndex = GetNearestIndex(listBox, e.GetPosition(listBox));

            if (targetIndex < 0)
                return;

            // Reorder the items in the list
            string draggedItem = callNumbers[draggedIndex];
            callNumbers.RemoveAt(draggedIndex);

            if (targetIndex > draggedIndex)
            {
                targetIndex--;
            }

            callNumbers.Insert(targetIndex, draggedItem);

            // Update the ListBox
            DisplayCallNumbers();
        }



        //---------------------------------------------------------------------------------------------------------------//



        private void CheckingOrder_Click(object sender, RoutedEventArgs e)
        {
            SortCallNumbersNumerically(); // Ensure that the sortedCallNumbers list is up to date

            // Get the reordered list from BooksListView2
            List<string> reorderedCallNumbers = new List<string>();
            foreach (var item in BooksListView2.Items)
            {
                reorderedCallNumbers.Add(item.ToString());
            }

            // Check if the user's order matches the sorted order
            bool isCorrectOrder = IsCorrectOrder(sortedCallNumbers, reorderedCallNumbers);

            // Display feedback to the user
            if (isCorrectOrder)
            {
                MessageBox.Show("Ordering iscorrect! Well done.", "Result", MessageBoxButton.OK, MessageBoxImage.Information);

                // Calculate the score
                score = CalculateScore();

                // Update the displayed score in the TextBlock
                scoreTextBlock.Text = $"{score}";
            }
            else
            {
                MessageBox.Show("Ordering is incorrect!", "Result", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsCorrectOrder(List<string> sortedOrder, List<string> userOrder)
        {
            // Compare the user's order to the sorted order
            if (sortedOrder.Count != userOrder.Count)
            {
                return false;
            }

            for (int i = 0; i < sortedOrder.Count; i++)
            {
                if (sortedOrder[i] != userOrder[i])
                {
                    return false;
                }
            }

            return true;
        }

        private List<string> SortCallNumbersNumerically()
        {
            callNumbers.Sort((x, y) =>
            {
                // Extract the numerical part of the call numbers (excluding any spaces and random letters)
                string[] xParts = x.Split(' ');
                string[] yParts = y.Split(' ');

                if (xParts.Length != 2 || yParts.Length != 2)
                {
                   
                    return 0;
                }

                int xNumber = int.Parse(xParts[0].Substring(5));
                int yNumber = int.Parse(yParts[0].Substring(5));

                // Compare the numerical parts for sorting in ascending order
                int compareResult = xNumber.CompareTo(yNumber);

                if (compareResult != 0)
                {
                    // If the numerical parts are different, return the comparison result
                    return compareResult;
                }
                else
                {
                    // If the numerical parts are equal, compare the random letters
                    return string.Compare(xParts[1], yParts[1]);
                }
            });

            // Clear the existing sortedCallNumbers list and add the sorted values
            sortedCallNumbers.Clear();
            sortedCallNumbers.AddRange(callNumbers);

           

            return sortedCallNumbers;
        }





        //---------------------------------------------------------------------------------------------------------------//


        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            // Reset the application state and timer

            BooksListView2.Items.Clear();

            // Reset other game-related variables or logic here.
            score = 0;
            secondsLeft = 180;
            scoreTextBlock.Text = $"Score: {score}";
            timeLeftTextBlock.Text = secondsLeft.ToString();

            // Stop the timer (if running) and restart it
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
            timer.Start();

           
            btnCheckorder.IsEnabled = true;
            
            
        }

        private void btn_id_click(object sender, RoutedEventArgs e)
        {
            PartTwoo P2 = new PartTwoo();
            P2.Show();
        }
    }
}
//---------------------------------------------------------------------------------------------------------------//


//references 
//There are parts that i used chatgpt in the code 
//i also followed some youtube vidoes 

