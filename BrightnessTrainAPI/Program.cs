using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BrightnessTrainAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if Release
                .UseUrls("http://*:7001")
#endif
                .UseStartup<Startup>();
    }
}
