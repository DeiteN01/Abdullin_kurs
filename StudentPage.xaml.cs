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
            var currentTour = AbdullinDBEntities.GetContext().Студенты.ToList();
            ProductListView.ItemsSource = currentTour;
            RangComboBox.SelectedIndex = 0;
            UpdateProductPage();
            int ProductMaxCount = currentTour.Count;
            ProductMaxCountTextBlock.Text = ProductMaxCount.ToString();
        }

        private void UpdateProductPage()
        {
            var currentProducts = AbdullinDBEntities.GetContext().Студенты.ToList();

            // Поиск
            currentProducts = currentProducts.Where(p => p.Фамилия.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();

            // Сортировка
            if (RadioButtonUp.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderBy(p => p.Год_рождения).ToList();
            }
            if (RadioButtonDown.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderByDescending(p => p.Год_рождения).ToList();
            }

            // Фильтрация по выбранной категории из ComboBox
            switch (RangComboBox.SelectedIndex)
            {
                case 0: // Все
                    break;
                case 1: // Отличники
                    currentProducts = currentProducts.Where(p => p.Успеваемость == "Отличник").ToList();
                    break;
                case 2: // Хорошисты
                    currentProducts = currentProducts.Where(p => p.Успеваемость == "Хорошист").ToList();
                    break;
                case 3: // Троечники
                    currentProducts = currentProducts.Where(p => p.Успеваемость == "Троечник").ToList();
                    break;
                default:
                    break;
            }

            ProductListView.ItemsSource = currentProducts;

            int ProductCount = currentProducts.Count();
            ProductCountTextBlock.Text = ProductCount.ToString();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = ProductListView.SelectedItem as Студенты;
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
                    UpdateProductPage(); // Обновляем данные после успешного удаления
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
            UpdateProductPage();
        }

        private void RadioButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProductPage();
        }

        private void RadioButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProductPage();
        }

        private void RangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProductPage();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProductPage();
        }
    }
}
