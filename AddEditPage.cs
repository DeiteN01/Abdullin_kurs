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
        public AddEditPage(StudentWithAverageScore SelectedChel = null)
        {
            InitializeComponent();

            _currentChel = SelectedChel.Student ?? new Студенты(); // Если SelectedChel == null, создаём новый объект
            
            //if (_currentChel.Студент_ID != 0)
            //{
            //    ComboType.SelectedIndex = 0; // Это можно изменить в зависимости от вашего логики
            //}

            DataContext = _currentChel;
        }

        // Изменение фотографии студента
        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; // Добавляем фильтр для изображений
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentChel.Фото = myOpenFileDialog.FileName; // Сохраняем путь к файлу в объекте
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName)); // Отображаем изображение в UI
            }
        }

        // Сохранение данных студента
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder(); // Собирать все ошибки в строку

            if (string.IsNullOrWhiteSpace(_currentChel.Фамилия)) // Проверка обязательных полей
            {
                errors.AppendLine("Укажите фамилию студента");
            }

            if (_currentChel.Группа_ID <= 0) // Проверка на валидность группы
            {
                errors.AppendLine("Укажите группу студента");
            }

            //if (ComboType.SelectedItem == null) // Проверка на обязательность должности
            //{
            //    errors.AppendLine("Укажите успеваемость студента");
            //}
            //else
            //{
            //    _currentChel.Успеваемость = (ComboType.SelectedItem as TextBlock)?.Text; // Заполняем должность (Оценка)
            //}

            if (_currentChel.Год_рождения == null) // Проверка на год рождения
            {
                errors.AppendLine("Укажите год рождения студента");
            }

            if (string.IsNullOrWhiteSpace(_currentChel.Отчество)) // Проверка на отчество
            {
                errors.AppendLine("Укажите отчество студента");
            }

            // Если есть ошибки, выводим их
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Если это новый студент (ID == 0), добавляем его в базу
            if (_currentChel.Студент_ID == 0)
            {
                AbdullinDBEntities.GetContext().Студенты.Add(_currentChel);
            }

            try
            {
                // Пытаемся сохранить изменения
                //MessageBox.Show($"Фамилия: {_currentChel.Фамилия}, Группа_ID: {_currentChel.Группа_ID}, Год рождения: {_currentChel.Год_рождения?.ToString() ?? "null"}");

                //_currentChel.Год_рождения = _currentChel.Год_рождения.Value;
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
