namespace WpfBlazorHyBird
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using WpfBlazorHyBird.Data;

    public static class Startup
    {
        public static IServiceProvider? Services { get; private set; }

        public static void Init()
        {
            var host = Host.CreateDefaultBuilder()
                           .ConfigureServices(WireupServices)
                           .Build();
            Services = host.Services;
        }

        private static void WireupServices(IServiceCollection services)
        {
            services.AddWpfBlazorWebView();
            services.AddSingleton<WeatherForecastService>();

#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif
        }
    }
}
