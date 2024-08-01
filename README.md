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

Create and configure dependencies of MicroNet.Tests project

```
dotnet new xunit -o MicroNet.Tests
dotnet sln add ./MicroNet.Tests/MicroNet.Tests.csproj
dotnet add ./MicroNet.Tests/MicroNet.Tests.csproj reference ./MicroNet.Domain/MicroNet.Domain.csproj
dotnet add ./MicroNet.Tests/MicroNet.Tests.csproj reference ./MicroNet.Infrastructure/MicroNet.Infrastructure.csproj
dotnet add ./MicroNet.Tests/MicroNet.Tests.csproj reference ./MicroNet.Application/MicroNet.Application.csproj
```

cd into Micronet.Tests and execute 
```
dotnet test
```