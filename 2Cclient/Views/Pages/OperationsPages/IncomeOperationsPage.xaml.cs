using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Contracts.ViewModels;

namespace _2Cclient.Views.Pages.OperationsPages
{
    public partial class IncomeOperationsPage : Page
    {
        private List<OperationVM> _items = new();

        public IncomeOperationsPage()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            // заглушка
            _items = new()
            {
                new OperationVM
                {
                    Id="1",
                    NameDocument="Поступление №1",
                    DateOperation=DateTime.Today,
                    TotalAmountDocument=3500m,
                    IsDeleted=false
                },
                new OperationVM
                {
                    Id="2",
                    NameDocument="Поступление №2",
                    DateOperation=DateTime.Today.AddDays(-1),
                    TotalAmountDocument=1200m,
                    IsDeleted=false
                }
            };

            OperationsList.ItemsSource = _items;
            OperationsList.SelectedItem = null;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var has = OperationsList.SelectedItem is OperationVM;
            DeleteBtn.IsEnabled = has;
            RepostBtn.IsEnabled = has;
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => UpdateButtons();

        private void Create_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new IncomeOperationEditPage());

        private void Repost_Click(object sender, RoutedEventArgs e)
        {
            if (OperationsList.SelectedItem is OperationVM vm)
                NavigationService?.Navigate(new IncomeOperationEditPage(vm));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
            => MessageBox.Show("Удаление (заглушка)");

        private void Back_Click(object sender, RoutedEventArgs e)
            => NavigationService?.GoBack();
    }
}
