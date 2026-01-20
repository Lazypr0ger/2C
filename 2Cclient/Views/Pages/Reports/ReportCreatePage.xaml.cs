using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _2Cclient.Views.Pages.Reports
{
    public partial class ReportCreatePage : Page
    {
        private readonly string _typeCode;
        private readonly string _typeTitle;

        public ReportCreatePage(string typeCode, string typeTitle)
        {
            InitializeComponent();
            _typeCode = typeCode;
            _typeTitle = typeTitle;

            Loaded += (_, __) =>
            {
                TitleText.Text = $"Новый отчёт: {_typeTitle}";
                SubtitleText.Text = "Заполните параметры и нажмите “Сформировать”";

                NameBox.Text = _typeTitle;

                var now = DateTime.Now;
                BuildDate.SelectedDate = now.Date;
                FromDate.SelectedDate = new DateTime(now.Year, now.Month, 1);
                ToDate.SelectedDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
            };
        }

        private async void Build_Click(object sender, RoutedEventArgs e)
        {
            var name = (NameBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название отчёта.");
                return;
            }

            if (!FromDate.SelectedDate.HasValue || !ToDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период (дата от/по).");
                return;
            }

            if (!BuildDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату формирования.");
                return;
            }

            try
            {
                BuildBtn.IsEnabled = false;

                // TODO: Реальный API:
                // var api = App.Services.GetRequiredService<ReportApi>();
                // var id = await api.BuildAsync(_typeCode, new ReportBuildRequest{ Name=name, From=..., To=..., BuildDate=..., Comment=... });

                await Task.Delay(150);
                var fakeId = Guid.NewGuid().ToString("N");

          //      NavigationService?.Navigate(new ReportViewerPage(fakeId, _typeCode, name));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчёта:\n{ex.Message}");
            }
            finally
            {
                BuildBtn.IsEnabled = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
