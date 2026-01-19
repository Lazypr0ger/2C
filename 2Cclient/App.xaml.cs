using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Windows;
using _2Cclient.Services.Api;
using _2Cclient.Services.Api.HistoryApi;

namespace _2Cclient
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var apiOptions = config.GetSection("Api").Get<ApiOptions>() ?? new ApiOptions();

            var sc = new ServiceCollection();

            sc.AddSingleton(apiOptions);

            sc.AddHttpClient<ApiClient>(http =>
            {
                http.BaseAddress = new Uri(apiOptions.BaseUrl);
                http.DefaultRequestHeaders.Accept.Clear();
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                http.Timeout = TimeSpan.FromSeconds(15);
            });

            sc.AddTransient<DepartamentApi>();
            sc.AddTransient<OrganisationApi>();
            sc.AddTransient<ProductionApi>();
            sc.AddTransient<DepartamentHistoryApi>();
            sc.AddTransient<OrganisationHistoryApi>();
            sc.AddTransient<ProductionHistoryApi>();


            Services = sc.BuildServiceProvider();

            var main = new Views.MainWindow();
            main.Show();
        }
    }
}
