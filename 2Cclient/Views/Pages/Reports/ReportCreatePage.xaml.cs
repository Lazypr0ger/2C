using System;
using System.Windows;
using System.Windows.Controls;
using _2Cclient.Services.Api;
using _2Cclient.UI;
using Contracts.BindingModels;
using Contracts.Enums;

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

                FieldValidation.ClearError(NameBox);
                FieldValidation.ClearError(FromDate);
                FieldValidation.ClearError(ToDate);
                FieldValidation.ClearError(BuildDate);
            };
        }

        private static ReportTypeCodes MapTypeCode(string typeCode) => typeCode switch
        {
            "actual_cost_distribution" => ReportTypeCodes.ActualCostDistribution,
            "sales_statement" => ReportTypeCodes.SalesStatement,
            "realised_deviation" => ReportTypeCodes.RealisedDeviationStatement,
            _ => ReportTypeCodes.ActualCostDistribution
        };

        private bool ValidateForm(out string name, out DateTime from, out DateTime to, out DateTime build)
        {
            name = (NameBox.Text ?? "").Trim();
            from = default;
            to = default;
            build = default;

            FieldValidation.ClearError(NameBox);
            FieldValidation.ClearError(FromDate);
            FieldValidation.ClearError(ToDate);
            FieldValidation.ClearError(BuildDate);

            if (string.IsNullOrWhiteSpace(name))
            {
                FieldValidation.SetError(NameBox, "Введите название отчёта");
                return false;
            }

            if (!FromDate.SelectedDate.HasValue)
            {
                FieldValidation.SetError(FromDate, "Выберите дату «от»");
                return false;
            }

            if (!ToDate.SelectedDate.HasValue)
            {
                FieldValidation.SetError(ToDate, "Выберите дату «по»");
                return false;
            }

            if (!BuildDate.SelectedDate.HasValue)
            {
                FieldValidation.SetError(BuildDate, "Выберите дату формирования");
                return false;
            }

            from = FromDate.SelectedDate.Value.Date;
            to = ToDate.SelectedDate.Value.Date;
            build = BuildDate.SelectedDate.Value.Date;

            if (from > to)
            {
                FieldValidation.SetError(FromDate, "Дата «от» не может быть больше даты «по»");
                FieldValidation.SetError(ToDate, "Дата «по» не может быть меньше даты «от»");
                return false;
            }

            return true;
        }

        private async void Build_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm(out var name, out var from, out var to, out var build))
                return;

            try
            {
                BuildBtn.IsEnabled = false;

                var api = (ReportApi)App.Services.GetService(typeof(ReportApi))!;

                var bm = new ReportBuildBM
                {
                    TypeCode = MapTypeCode(_typeCode),
                    From = from,
                    To = to,
                    Name = name,
                    Comment = (CommentBox.Text ?? "").Trim()
                };

                var report = await api.BuildAsync(bm);

                // Переход на новый Viewer (который принимает ReportResultVM)
                NavigationService?.Navigate(new ReportViewerPage(report, bm.Comment ?? ""));
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
