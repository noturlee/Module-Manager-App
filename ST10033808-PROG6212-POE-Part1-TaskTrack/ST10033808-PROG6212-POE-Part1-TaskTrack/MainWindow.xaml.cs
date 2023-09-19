using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ModuleDataLibrary;

namespace ST10033808_PROG6212_POE_Part1_TaskTrack
{
    public partial class MainWindow : Window
    {
        private List<FormData> dataList = new List<FormData>();

        public MainWindow()
        {
            InitializeComponent();
            GridDisplay.Visibility = Visibility.Hidden;
            ViewTab.Visibility = Visibility.Hidden;

        }

        private void AddModuleBtn_Click(object sender, RoutedEventArgs e)
        {
            DataEntryGB.IsEnabled = true;
            DataEntryGB.Visibility = Visibility.Visible;
            GridDisplay.Visibility = Visibility.Visible;
            ViewTab.Visibility = Visibility.Hidden;

        }

        private bool IsModuleCodeNameDuplicate(string moduleCode, string moduleName)
        {
            // Check if module code or module name already exists in the list
            return dataList.Any(data => data.MCode == moduleCode || data.ModuleName == moduleName);
        }


        private void EnterDataBtn_Click(object sender, RoutedEventArgs e)
        {
            // Reset the TextBox border brushes to their default color
            ModuleCodeTxt.BorderBrush = Brushes.White;
            ModuleNameTxt.BorderBrush = Brushes.White;
            CreditNumberTxt.BorderBrush = Brushes.White;
            NumweekSem.BorderBrush = Brushes.White;
            ClassHrInWeekTxt.BorderBrush = Brushes.White;
            CampusStartDateTxt.BorderBrush = Brushes.White;

            string moduleCode = ModuleCodeTxt.Text.Trim();
            string moduleName = ModuleNameTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(moduleCode) || string.IsNullOrWhiteSpace(moduleName))
            {
                MessageBox.Show("Module code and module name are required.");
                ModuleCodeTxt.BorderBrush = Brushes.Red;
                ModuleNameTxt.BorderBrush = Brushes.Red;
                return;
            }

            double creditNumber;
            if (!double.TryParse(CreditNumberTxt.Text, out creditNumber))
            {
                MessageBox.Show("Invalid credit number.");
                CreditNumberTxt.BorderBrush = Brushes.Red;
                return;
            }

            double numWeeks;
            if (!double.TryParse(NumweekSem.Text, out numWeeks))
            {
                MessageBox.Show("Invalid number of weeks.");
                NumweekSem.BorderBrush = Brushes.Red;
                return;
            }

            double classHours;
            if (!double.TryParse(ClassHrInWeekTxt.Text, out classHours))
            {
                MessageBox.Show("Invalid class hours.");
                ClassHrInWeekTxt.BorderBrush = Brushes.Red;
                return;
            }

            DateTime startDate;
            if (!DateTime.TryParse(CampusStartDateTxt.Text, out startDate))
            {
                MessageBox.Show("Invalid start date.");
                CampusStartDateTxt.BorderBrush = Brushes.Red;
                return;
            }

            if (IsModuleCodeNameDuplicate(moduleCode, moduleName))
            {
                MessageBox.Show("Module code or name already exists.");
                return;
            }

            var newData = new FormData
            {
                MCode = moduleCode,
                ModuleName = moduleName,
                NumberCredits = creditNumber,
                NumberofWeeks = numWeeks,
                ClassHours = classHours,
                StartDate = startDate,
                StudyHrs = (creditNumber * 10) / numWeeks - classHours,
            };

        
            MessageBox.Show($"{moduleName} has been added");
            dataList.Add(newData);

            var groupedAndSortedData = dataList
                .GroupBy(data => data.MCode)
                .OrderBy(group => group.Key)  
                .SelectMany(group => group.OrderBy(data => data.MCode));
            ModuleCodeComboBox.Items.Clear();
            ModuleDataGrid.ItemsSource = groupedAndSortedData.ToList();
            foreach (var item in dataList) {
                ModuleCodeComboBox.Items.Add(item);
            }
          

            ModuleCodeTxt.Text = "";
            ModuleNameTxt.Text = "";
            CreditNumberTxt.Text = "";
            NumweekSem.Text = "";
            ClassHrInWeekTxt.Text = "";
            CampusStartDateTxt.Text = "";
            DataEntryGB.IsEnabled = false;
        }


        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            //ModuleCodeComboBox.ItemsSource = dataList;
            DataEntryGB.Visibility = Visibility.Hidden;
            TaskTrackerLabel.Visibility = Visibility.Hidden;
            GridDisplay.Visibility = Visibility.Hidden;
            ViewTab.Visibility = Visibility.Visible;
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to open File Explorer", "Please Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                OpenFileDialog OpenFile = new OpenFileDialog();
                if (OpenFile.ShowDialog() == true)
                {
                    ProfilePic.Source = new BitmapImage(new Uri(OpenFile.FileName));
                }
            }
            else
            {
                MessageBox.Show("Operation Canceled");
            }
        }

        private void EnterStudyDataBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the module code, study hours, and date from the input fields
            string moduleCode = ModuleCodeComboBox.Text;
            double studyHours = double.Parse(TotalSelfStudyHoursTextBlock.Text);
            DateTime enteredDate = DateTime.Parse(DateDatePicker.SelectedDate.ToString());

            // Find the module in the dataList
            var module = dataList.FirstOrDefault(data => data.MCode == moduleCode);

            if (module == null)
            {
                MessageBox.Show("Module not found.");
                return;
            }

            // Calculate the date range
            DateTime startDate = module.StartDate.Date;
            DateTime endDate = startDate.AddDays(module.NumberofWeeks * 7);

            // Check if the entered dat            ModuleDataGrid.ItemsSource = dataList;
            if (enteredDate <= startDate || enteredDate >= endDate)
            {
                MessageBox.Show("Entered date is not within the specified date range.");
                return;
            }

            // Subtract study hours from the module's StudyHrs
            module.StudyHrs -= studyHours;

            // Update the DataGridView

            ModuleDataGrid.ItemsSource = dataList;

            MessageBox.Show($"{studyHours} study hours deducted from {module.ModuleName}\nRemaining study hours: {module.StudyHrs}");
        }


        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Goodbye");
            System.Environment.Exit(0);
        }
    }
}

