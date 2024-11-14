using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Abdullin_kurs
{
    public partial class AddEditPage : Page
    {
        private Студенты _modelStudent;
        private readonly AbdullinDBEntities _dbContext;

        public AddEditPage(StudentModel studentModel = null)
        {
            InitializeComponent();
            _dbContext = AbdullinDBEntities.GetContext();

            var groups = _dbContext.Учебные_группы.ToList();
            _modelStudent = studentModel?.Student ?? new Студенты();

            if (_modelStudent.Студент_ID == 0)
            {
                DeleteButton.Visibility = Visibility.Hidden;
            }

            GroupComboBox.ItemsSource = groups;
            DataContext = _modelStudent;
        }

        // Изменение фотографии студента
        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _modelStudent.Фото = myOpenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void GroupComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGroup = GroupComboBox.SelectedItem as Учебные_группы;
            if (selectedGroup != null)
            {
                _modelStudent.Группа_ID = selectedGroup.Группа_ID;
            }
        }


        // Сохранение данных студента
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder(); // Собирать все ошибки в строку

            if (string.IsNullOrWhiteSpace(_modelStudent.Фамилия))
            {
                errors.AppendLine("Укажите фамилию студента");
            }

            if (_modelStudent.Группа_ID <= 0)
            {
                errors.AppendLine("Укажите группу студента");
            }

            if (string.IsNullOrWhiteSpace(_modelStudent.Отчество))
            {
                errors.AppendLine("Укажите отчество студента");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_modelStudent.Студент_ID == 0)
            {
                AbdullinDBEntities.GetContext().Студенты.Add(_modelStudent);
            }

            try
            {
                AbdullinDBEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack(); // Возвращаемся на предыдущую страницу
            }
            catch (Exception ex)
            {
                // Если возникла ошибка при сохранении
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        // Удаление студента
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_modelStudent == null || _modelStudent.Студент_ID == 0)
            {
                MessageBox.Show("Выделите студента для удаления");
                return;
            }

            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем студента из базы данных
                    AbdullinDBEntities.GetContext().Студенты.Remove(_modelStudent);
                    AbdullinDBEntities.GetContext().SaveChanges();
                    MessageBox.Show("Студент удалён");
                    Manager.MainFrame.GoBack(); // Возвращаемся на страницу студентов
                }
                catch (Exception ex)
                {
                    // Если возникла ошибка при удалении
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}