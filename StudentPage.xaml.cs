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

            // Сортировка
            if (RadioButtonUp.IsChecked.Value)
            {
                students = students.OrderBy(p => p.Год_рождения).ToList();
            }
            if (RadioButtonDown.IsChecked.Value)
            {
                students = students.OrderByDescending(p => p.Год_рождения).ToList();
            }

            // Фильтрация по выбранной категории из ComboBox
            switch (RangComboBox.SelectedIndex)
            {
                case 0: // Все
                    break;
                case 1: // Отличники
                    students = students.Where(p => p.Успеваемость == "Отличник").ToList();
                    break;
                case 2: // Хорошисты
                    students = students.Where(p => p.Успеваемость == "Хорошист").ToList();
                    break;
                case 3: // Троечники
                    students = students.Where(p => p.Успеваемость == "Троечник").ToList();
                    break;
                default:
                    break;
            }

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

                studentsWithScores.Add(new StudentWithAverageScore
                {
                    Student = student,
                    AverageScore = averageScore
                });
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
            var currentClient = StudentListView.SelectedItem as Студенты;
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
                    UpdateStudentPage(); // Обновляем данные после успешного удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Нельзя удалить студента, так как он имеет связь к ведомости успеваемости.");
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Студенты));
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
