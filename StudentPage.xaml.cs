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
            var dbEntities = AbdullinDBEntities.GetContext();
            var groups = dbEntities.Учебные_группы;
            students = students.Where(p => p.Фамилия.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();

            List<StudentModel> studentModels = new List<StudentModel>();

            foreach (var student in students)
            {
                double ScoreSum = 0;
                double counter = 0;

                foreach (var ведомость in student.Ведомость_успеваемости)
                {
                    ScoreSum += double.Parse(ведомость.Оценка.ToString());
                    counter++;
                }

                var group = groups.FirstOrDefault(g => g.Студенты.Any(stud => stud.Студент_ID == student.Студент_ID));
                string groupName = group != null ? group.Название_группы : "Группа не найдена";

                string departmentName = "Кафедра не найдена";
                string facultyName = "Факультет не найден";

                if (group != null && group.Кафедры != null)
                {
                    departmentName = group.Кафедры.Название_кафедры;

                    if (group.Кафедры.Факультеты != null)
                    {
                        facultyName = group.Кафедры.Факультеты.Название_факультета;
                    }
                }

                double averageScore = counter > 0 ? ScoreSum / counter : 0;
                averageScore = Math.Round(averageScore, 2);

                studentModels.Add(new StudentModel
                {
                    Student = student,
                    AverageScore = averageScore,
                    GroupName = groupName,
                    DepartmentName = departmentName,
                    FacultyName = facultyName
                });
            }

            // Сортировка по среднему баллу
            if (RadioButtonUp.IsChecked.Value)
            {
                studentModels = studentModels.OrderBy(p => p.Student.Фамилия).ThenBy(p => p.Student.Имя).ToList();
            }

            if (RadioButtonDown.IsChecked.Value)
            {
                studentModels = studentModels.OrderByDescending(p => p.Student.Фамилия)
                    .ThenByDescending(p => p.Student.Имя).ToList();
            }

            // Фильтрация по среднему баллу
            switch (RangComboBox.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    studentModels = studentModels.Where(p => p.AverageScore == 5.0).ToList();
                    break;
                case 2:
                    studentModels = studentModels.Where(p => p.AverageScore >= 4 && p.AverageScore < 5)
                        .ToList();
                    break;
                case 3:
                    studentModels = studentModels.Where(p => p.AverageScore < 4).ToList();
                    break;
                default:
                    break;
            }

            StudentListView.ItemsSource = studentModels;

            int StudentCount = studentModels.Count;
            StudentCountTextBlock.Text = StudentCount.ToString();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (StudentListView.SelectedItem as StudentModel).Student;
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
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as StudentModel));
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