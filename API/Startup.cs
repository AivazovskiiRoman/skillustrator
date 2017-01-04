using Skillustrator.Api.Infrastructure;
using Skillustrator.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skillustrator.Api.Entities;
using Newtonsoft.Json;

namespace ConsoleApplication
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder() // Collection of sources for read/write key/value pairs
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables(); // Overrides environment variables with valiues from config files/etc
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddJsonFormatters(o=> o.PreserveReferencesHandling = PreserveReferencesHandling.Objects);

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<SkillustratorContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IArticlesRepository, ArticlesRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IRepository<Skill>, BaseRepository<Skill>>();
            services.AddScoped<IRepository<Tag>, BaseRepository<Tag>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            var startupLogger = loggerFactory.CreateLogger<Startup>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // Create DB on startup
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<SkillustratorContext>().Database.Migrate();
            }

            // Not currently seeding anything, fix
            var applicantSeeder = new TestDataSeeder();
            applicantSeeder.SeedAsync(app.ApplicationServices).Wait();
            startupLogger.LogInformation("Data seed completed.");
            
        }
    }
}