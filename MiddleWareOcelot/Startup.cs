using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace MiddleWareOcelot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // ВАЖНО: SwaggerForOcelot
            services.AddSwaggerForOcelot(Configuration);

            // Ocelot
            services.AddOcelot(Configuration);

            // Регистрируем наш middleware
            services.AddScoped<RequestLoggingMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // Добавляем middleware для логирования ВСЕХ запросов
            app.UseMiddleware<RequestLoggingMiddleware>();

            // UI будет доступен по /swagger
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseOcelot().Wait();
        }
    }
}