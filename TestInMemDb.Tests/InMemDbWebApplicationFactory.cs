using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using TestInMemDb.Data;

namespace TestInMemDb.Tests
{
    public class InMemDbWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public static readonly InMemoryDatabaseRoot InMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public ServiceProvider ServiceProvider { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                services.AddDbContext<TestDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDb", InMemoryDatabaseRoot);
                    options.UseInternalServiceProvider(serviceProvider);
                });

                ServiceProvider = services.BuildServiceProvider();
            });
        }
    }
}