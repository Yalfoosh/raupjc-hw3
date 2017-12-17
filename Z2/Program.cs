using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Z2
{
    public class Program
    {
        public static void Main(string[] args) => BuildWebHost(args).Run();

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   /*.UseSerilog((context, logger) =>
                    {
                        var connectionString = context.Configuration["ConnectionString"];

                        logger.MinimumLevel
                              .Error()
                              .Enrich.FromLogContext()
                              .WriteTo
                              .MSSqlServer(connectionString, "Errors", autoCreateSqlTable: true);

                        if (context.HostingEnvironment.IsDevelopment())
                            logger.WriteTo.RollingFile("error-log.txt");
                    })*/
                   .Build();
    }
}