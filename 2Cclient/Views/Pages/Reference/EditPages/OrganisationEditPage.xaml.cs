using System.Windows;
using System.Windows.Controls;
using Contracts.ViewModels;

namespace _2Cclient.Views.Pages
{
    public partial class OrganisationEditPage : Page
    {
        private readonly bool _isEdit;
        private readonly OrganisationVM? _original;

        // Create
        public OrganisationEditPage()
        {
            InitializeComponent();
            _isEdit = false;
            TitleText.Text = "Добавить организацию";
            SubtitleText.Text = "Заполните поля и нажмите «Сохранить»";
            IsDeletedCheck.IsEnabled = false; // при создании обычно не дают "удалён"
        }

        // Edit
        public OrganisationEditPage(OrganisationVM vm)
        {
            InitializeComponent();
            _isEdit = true;
            _original = vm;

            TitleText.Text = "Обновить организацию";
            SubtitleText.Text = "Измените поля и нажмите «Сохранить»";

            NameBox.Text = vm.Name;
            AccountBox.Text = vm.AccountNumOrg;
            IsDeletedCheck.IsChecked = vm.IsDeleted;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text.Trim();
            var acc = AccountBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название не должно быть пустым");
                return;
            }

            if (string.IsNullOrWhiteSpace(acc))
            {
                MessageBox.Show("Счёт организации не должен быть пустым");
                return;
            }

            // Здесь позже будет вызов API.
            // Сейчас просто покажем, что сохранили:
            var isDeleted = IsDeletedCheck.IsChecked == true;

            MessageBox.Show(_isEdit
                ? $"Сохранено (редактирование): {name}"
                : $"Сохранено (создание): {name}");

            // Возвращаемся назад
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
