[![Generic badge](https://img.shields.io/badge/Version-1.0-<COLOR>.svg)](https://shields.io/)
[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://GitHub.com/Naereen/StrapDown.js/graphs/commit-activity)
![Maintainer](https://img.shields.io/badge/maintainer-raphael.chir@gmail.com-blue)

# MicroNet DDD microservice

## Architecture setup
Prerequisites : install .NET 8 https://dotnet.microsoft.com/en-us/download/dotnet/8.0

Open a terminal, create your root folder project
mkdir MicroNet

Open vscode, open or use terminal
cd MicroNet

First create your solution project that will contain all your subprojects Domain, then Application, Infrastructure and API
dotnet new sln -n MicroNet

Create the subprojects
```
dotnet new classlib -n MicroNet.Domain
dotnet new classlib -n MicroNet.Application
dotnet new classlib -n MicroNet.Infrastructure
dotnet new webapi -n MicroNet.API
```

Init C# project definition in each subprojects
```
dotnet sln add MicroNet.Domain/MicroNet.Domain.csproj
dotnet sln add MicroNet.Application/MicroNet.Application.csproj
dotnet sln add MicroNet.Infrastructure/MicroNet.Infrastructure.csproj
dotnet sln add MicroNet.API/MicroNet.API.csproj
```

Configure the dependencies between the subproject (Note that Domain must be independant)
```
dotnet add MicroNet.Application/MicroNet.Application.csproj reference MicroNet.Domain/MicroNet.Domain.csproj
dotnet add MicroNet.Infrastructure/MicroNet.Infrastructure.csproj reference MicroNet.Domain/MicroNet.Domain.csproj
```

Continue for API subproject as he can use Domain, Application and Infrastructure layer

Clean, build and run

```
dotnet clean
dotnet build
dotnet run --project MicroNet.API
```

[http://localhost:5183/swagger/index.html](http://localhost:5183/swagger/index.html)

## Tests subproject

Create and configure dependencies of MicroNet.Tests project, using XUnit and not NUnit

```
dotnet new xunit -o MicroNet.Tests
dotnet sln add ./MicroNet.Tests/MicroNet.Tests.csproj
dotnet add ./MicroNet.Tests/MicroNet.Tests.csproj reference ./MicroNet.Domain/MicroNet.Domain.csproj
dotnet add ./MicroNet.Tests/MicroNet.Tests.csproj reference ./MicroNet.Infrastructure/MicroNet.Infrastructure.csproj
dotnet add ./MicroNet.Tests/MicroNet.Tests.csproj reference ./MicroNet.Application/MicroNet.Application.csproj
```

## Infrastructure : Add Couchbase .NET SDK dependency

cd into MicroNet.Infrastructure and execute
```
dotnet add package CouchbaseNetClient --version 3.6.2
```

cd into MicroNet.Tests and execute 
```
dotnet test
```

See CBStandardCnxTest.cs

## Dependency Injection

Another connexion method is shown here.

### Test first

To begin, add extensions, including Couchbase.Extensions.DependencyInjection to take advantage of DI feature ans ASP.NET
cd into MicroNet.Tests and execute

```
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Couchbase.Extensions.DependencyInjection --version 3.6.2
```
Use appsettings.json
Create TestFixture.cs

```
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
            .AddJsonFile("/Users/raphael.chir/Workspaces/MicroNet/MicroNet.Tests/appsettings.json", optional: false, reloadOnChange: true); // TODO relative path  

        Configuration = configurationBuilder.Build();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IConfiguration>(Configuration);
                services.AddCouchbase(Configuration.GetSection("Couchbase")); // Refer to appsettings.json
                services.AddCouchbaseBucket<INamedBucketProvider>("travel-sample");
            })
            .Build();

        ServiceProvider = host.Services.CreateScope().ServiceProvider;
    }
}
```

appsettings.json

```
{
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "Couchbase": {
      "ConnectionString": "ec2-51-20-133-117.eu-north-1.compute.amazonaws.com, ec2-16-170-172-163.eu-north-1.compute.amazonaws.com, ec2-51-20-133-117.eu-north-1.compute.amazonaws.com",
      "Username": "admin",
      "Password": "111111"
    }
  }
```

see CBCnxCBDICnxTest.cs and run tests

## MicroNet API Configuration

AS Micronet.API project access to MicroNet.Infrastructure we include Couchbase.Extensions.DependencyInjection in MicroNet.Infrastructure.csproj

```
    <PackageReference Include="Couchbase.Extensions.DependencyInjection" Version="3.6.2" />
```

In MicroNet.API update appsettings.json and appsettings.Development.json

Simply configure Dependencies Injection in Program.cs 

```
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

IBucket bucket;
    try
    {
        bucket = app.Services.GetRequiredService<IBucketProvider>().GetBucketAsync("mat-sample").GetAwaiter().GetResult();
    }
    catch (Exception)
    {
        throw new InvalidOperationException("Ensure that you have the travel-sample bucket loaded in the cluster.");
    }

    
app.Run();
```

ContratsController.cs calls ContratsServices that calls ContratsRepository
Inject INamedBucketProvider in ContratsRepository

```
    public class ContratRepository : IContratRepository
    {
        private readonly INamedBucketProvider _provider;

        public ContratRepository(INamedBucketProvider provider)
        {
            _provider = provider;
        }

```

Clean, build and run MicroNet.API projects to see that the connexion is well established in the console

```
dotnet clean
dotnet build
dotnet run --project MicroNet.API
```
### Data generation

See DataGenTests.cs to create a sample of data
