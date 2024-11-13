using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Abdullin_kurs
{
    public partial class AddEditPage : Page
    {
        private Студенты _currentChel;

        // Конструктор для создания нового или редактирования существующего студента
        public AddEditPage(StudentModel SelectedChel = null)
        {
            InitializeComponent();

            _currentChel = (SelectedChel == null || SelectedChel.Student == null) ? new Студенты() : SelectedChel.Student;

            DataContext = _currentChel;
        }

        // Изменение фотографии студента
        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; 
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentChel.Фото = myOpenFileDialog.FileName; 
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName)); 
            }
        }

        // Сохранение данных студента
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder(); // Собирать все ошибки в строку

            if (string.IsNullOrWhiteSpace(_currentChel.Фамилия)) 
            {
                errors.AppendLine("Укажите фамилию студента");
            }

            if (_currentChel.Группа_ID <= 0) 
            {
                errors.AppendLine("Укажите группу студента");
            }

            if (string.IsNullOrWhiteSpace(_currentChel.Отчество))
            {
                errors.AppendLine("Укажите отчество студента");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
           
            if (_currentChel.Студент_ID == 0)
            {
                AbdullinDBEntities.GetContext().Студенты.Add(_currentChel);
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
            var currentChel = (sender as Button).DataContext as Студенты;

            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем студента из базы данных
                    AbdullinDBEntities.GetContext().Студенты.Remove(currentChel);
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
