using System.Windows;
using System.Windows.Controls;
using Contracts.ViewModels;

namespace _2Cclient.Views.Pages
{
    public partial class DepartamentEditPage : Page
    {
        private readonly bool _isEdit;
        private readonly DepartamentVM? _original;

        public DepartamentEditPage()
        {
            InitializeComponent();
            _isEdit = false;
            TitleText.Text = "Добавить подразделение";
            SubtitleText.Text = "Заполните поля и нажмите «Сохранить»";
            IsDeletedCheck.IsEnabled = false;
        }

        public DepartamentEditPage(DepartamentVM vm)
        {
            InitializeComponent();
            _isEdit = true;
            _original = vm;

            TitleText.Text = "Обновить подразделение";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            NameBox.Text = vm.Name;
            IsDeletedCheck.IsChecked = vm.IsDeleted;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название не должно быть пустым");
                return;
            }

            MessageBox.Show(_isEdit
                ? $"Сохранено (редактирование): {name}"
                : $"Сохранено (создание): {name}");

            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
