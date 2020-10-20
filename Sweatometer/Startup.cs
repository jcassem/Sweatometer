using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sweatometer.Data.Emoji;
using Sweatometer.Service;

namespace Sweatometer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.Configure<MergeOptions>(Configuration.GetSection("MergeOptions"));
            services.Configure<EmojiRelatedWordOptions>(Configuration.GetSection("EmojiRelatedWordOptions"));

            services.AddScoped<IWordFinderService, WordFinderService>();
            services.AddScoped<IMergeService, MergeService>();
            services.AddScoped<ISweatTestService, SweatTestService>();
            services.AddScoped<IEmojiSearchService, EmojiSearchService>();
            services.AddScoped<IEmojiDataGenerator, EmojiDataGenerator>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="emojiDataGenerator"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEmojiDataGenerator emojiDataGenerator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            EmojiDataLoader.LoadEmojiDataFromFile();
            // NB Inject a EmojiDataGenerator instance into this method to re-generate data here.
            // emojiDataGenerator.CreateRelatedWordsDictionary();
        }
    }
}
