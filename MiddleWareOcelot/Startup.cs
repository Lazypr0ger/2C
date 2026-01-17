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

            services.AddSwaggerForOcelot(Configuration);

           
            services.AddOcelot(Configuration);

            // Регистрируем наш middleware
            services.AddScoped<RequestLoggingMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

         
            app.UseMiddleware<RequestLoggingMiddleware>();

            
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseOcelot().Wait();
        }
    }
}