using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Abdullin_kurs
{
    public partial class StudentPage : Page
    {
        public StudentPage()
        {
            InitializeComponent();
            var StudentsList = AbdullinDBEntities.GetContext().Студенты.ToList();
            StudentListView.ItemsSource = StudentsList;
            RangComboBox.SelectedIndex = 0;
            UpdateStudentPage();
            int StudentsMaxCount = StudentsList.Count;
            ProductMaxCountTextBlock.Text = StudentsMaxCount.ToString();
        }

        private void UpdateStudentPage()
        {
            var students = AbdullinDBEntities.GetContext().Студенты.ToList();

            // Поиск
            students = students.Where(p => p.Фамилия.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();

            List<StudentWithAverageScore> studentsWithScores = new List<StudentWithAverageScore>();

            foreach (var student in students)
            {
                double ScoreSum = 0;
                double counter = 0;

                foreach (var ведомость in student.Ведомость_успеваемости)
                {
                    ScoreSum += double.Parse(ведомость.Оценка.ToString());
                    counter++;
                }

                double averageScore = counter > 0 ? ScoreSum / counter : 0;
                averageScore = Math.Round(averageScore, 2); // Округление до двух знаков после запятой

                studentsWithScores.Add(new StudentWithAverageScore
                {
                    Student = student,
                    AverageScore = averageScore
                });
            }

            // Сортировка по среднему баллу
            if (RadioButtonUp.IsChecked.Value)
            {
                studentsWithScores = studentsWithScores.OrderBy(p => p.Student.Фамилия).ThenBy(p => p.Student.Имя).ToList();
            }

            if (RadioButtonDown.IsChecked.Value)
            {
                studentsWithScores = studentsWithScores.OrderByDescending(p => p.Student.Фамилия).ThenByDescending(p => p.Student.Имя).ToList();
            }

            // Фильтрация по среднему баллу
            switch (RangComboBox.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    studentsWithScores = studentsWithScores.Where(p => p.AverageScore == 5.0).ToList();
                    break;
                case 2:
                    studentsWithScores = studentsWithScores.Where(p => p.AverageScore >= 4 && p.AverageScore < 5)
                        .ToList();
                    break;
                case 3:
                    studentsWithScores = studentsWithScores.Where(p => p.AverageScore < 4).ToList();
                    break;
                default:
                    break;
            }

            StudentListView.ItemsSource = studentsWithScores;

            int StudentCount = studentsWithScores.Count;
            StudentCountTextBlock.Text = StudentCount.ToString();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (StudentListView.SelectedItem as StudentWithAverageScore).Student;
            if (currentClient == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
                return;
            }

            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    AbdullinDBEntities.GetContext().Студенты.Remove(currentClient);
                    AbdullinDBEntities.GetContext().SaveChanges();
                    MessageBox.Show("Запись успешно удалена.");
                    UpdateStudentPage(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Нельзя удалить студента, так как он имеет связь к ведомости успеваемости.");
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as StudentWithAverageScore));
            UpdateStudentPage();
        }

        private void RadioButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateStudentPage();
        }

        private void RadioButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateStudentPage();
        }

        private void RangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStudentPage();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateStudentPage();
        }
    }
}
