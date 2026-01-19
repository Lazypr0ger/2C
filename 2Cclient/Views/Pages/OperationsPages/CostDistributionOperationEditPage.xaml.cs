using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class CostDistributionOperationEditPage : Page
    {
        private readonly OperationListItemVM? _editing;

        public CostDistributionOperationEditPage()
        {
            InitializeComponent();
            _editing = null;

            TitleText.Text = "Распределение фактической себестоимости";
            SubtitleText.Text = "Заполните параметры и нажмите «Распределить»";

            DateOperationPicker.SelectedDate = DateTime.Today;

            var now = DateTime.Today;
            StartDatePicker.SelectedDate = new DateTime(now.Year, now.Month, 1);
            EndDatePicker.SelectedDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            DepartamentBox.ItemsSource = new List<SimpleDepartamentVM>
            {
                new() { Id="1", Name="Цех 01" },
                new() { Id="2", Name="Цех 02" }
            };
            DepartamentBox.DisplayMemberPath = "Name";
            DepartamentBox.SelectedValuePath = "Id";

            ApplyBtn.Content = "Распределить";
        }

        public CostDistributionOperationEditPage(OperationListItemVM vm) : this()
        {
            _editing = vm;

            TitleText.Text = "Перепроведение распределения";
            SubtitleText.Text = "Измените параметры и нажмите «Распределить»";

            NameDocumentBox.Text = vm.NameDocument;
            DateOperationPicker.SelectedDate = vm.DateOperation;

            // Период/подразделение позже подтянем по id операции с API (когда будет эндпоинт GetById)
            ApplyBtn.Content = "Распределить";
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
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

            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите период расчёта");
                return;
            }

            var start = StartDatePicker.SelectedDate.Value.Date;
            var end = EndDatePicker.SelectedDate.Value.Date;

            if (end < start)
            {
                MessageBox.Show("Дата окончания периода не может быть меньше даты начала");
                return;
            }

            if (DepartamentBox.SelectedItem is not SimpleDepartamentVM dep)
            {
                MessageBox.Show("Выберите подразделение");
                return;
            }

            // Заглушка: здесь будет вызов API:
            // - если _editing == null -> Create+Post "Распределить"
            // - если _editing != null -> Repost "Распределить" по существующей операции
            MessageBox.Show(
                $"Распределение (заглушка)\n" +
                $"Mode={(_editing == null ? "Create" : "Repost")}\n" +
                $"Name={name}\n" +
                $"Date={DateOperationPicker.SelectedDate:dd.MM.yyyy}\n" +
                $"Departament={dep.Name}\n" +
                $"Period={start:dd.MM.yyyy}-{end:dd.MM.yyyy}");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private class SimpleDepartamentVM
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
        }

        public class OperationListItemVM
        {
            public string Id { get; set; } = "";
            public string NameDocument { get; set; } = "";
            public DateTime DateOperation { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
