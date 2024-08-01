using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

public class TestFixture
{
    public IServiceProvider ServiceProvider { get; private set; }
    public IConfiguration Configuration { get; private set; }

    public TestFixture()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("/Users/raphael.chir/Workspaces/MicroNet/MicroNet.Tests/appsettings.json", optional: false, reloadOnChange: true);

        Configuration = configurationBuilder.Build();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IConfiguration>(Configuration);
                services.AddCouchbase(Configuration.GetSection("Couchbase"));
                services.AddCouchbaseBucket<INamedBucketProvider>("travel-sample");
            })
            .Build();

        ServiceProvider = host.Services.CreateScope().ServiceProvider;
    }
}