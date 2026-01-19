using System;
using System.Windows;
using System.Windows.Controls;
using Contracts.Enums;

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class WriteOffDeviationOperationEditPage : Page
    {
        private readonly OperationType _type = OperationType.WriteOffDeviations; // позже подставим нужный enum

        public WriteOffDeviationOperationEditPage()
        {
            InitializeComponent();

            DateOperationPicker.SelectedDate = DateTime.Today;
            NameDocumentBox.Text = $"Списание отклонений {DateTime.Today:MM.yyyy}";
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            var name = NameDocumentBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название документа не должно быть пустым");
                return;
            }

            if (DateOperationPicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату операции");
                return;
            }

            // Заглушка: позже вызов API на проведение операции списания
            MessageBox.Show(
                $"Списание (заглушка)\n" +
                $"Type={_type}\n" +
                $"Name={name}\n" +
                $"Date={DateOperationPicker.SelectedDate:dd.MM.yyyy}\n");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
