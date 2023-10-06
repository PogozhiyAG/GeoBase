using System.Text.Json.Serialization;
using GeoIp;

namespace GeoBaseWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddMvcCore().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                opt.JsonSerializerOptions.WriteIndented = true;
            });

            services.Configure<GeoIpOptions>(o =>
            {
                o.MaxRecordCount = 50;
            });
            services.AddSingleton(CreateGeoIpService());
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }


        public GeoIpDatabase CreateGeoIpService()
        {
            var data = File.ReadAllBytes(Configuration["GeoIP:DataFileName"]);
            var result = new GeoIpDatabase(data);
            return result;
        }
    }
}
