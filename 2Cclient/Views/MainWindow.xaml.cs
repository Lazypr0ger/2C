using System.Windows;
using System.Windows.Controls;
using _2Cclient.Views.Pages;
namespace _2Cclient.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Стартовая страница (если есть HomePage)
            // Если HomePage нет — закомментируй и просто выбери первый пункт меню.
            try
            {
                MainFrame.Navigate(new HomePage());
            }
            catch
            {
                // Если HomePage еще не создан — просто игнорим
            }

            // Сразу подсветить первый пункт меню (Справочники)
            NavList.SelectedIndex = 0;
        }

        private void NavList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavList.SelectedItem is not ListBoxItem item)
                return;

            var tag = item.Tag?.ToString();

            switch (tag)
            {
                case "Directories":
                    PageTitleText.Text = "Справочники";
                    MainFrame.Navigate(new DirectoriesPage());
                    break;

                case "Operations":
                    PageTitleText.Text = "Операции";
                    MainFrame.Navigate(new OperationsPage());
                    break;

                case "Reports":
                    PageTitleText.Text = "Отчёты";
                    MainFrame.Navigate(new ReportsPage());
                    break;

                case "Ledger":
                    PageTitleText.Text = "Журнал проводок";
                    MainFrame.Navigate(new LedgerPage());
                    break;
            }
        }
    }
}
