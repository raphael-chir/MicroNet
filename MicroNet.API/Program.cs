using Couchbase.Extensions.DependencyInjection;
using MicroNet.Infrastructure;
using MicroNet.Domain;
using MicroNet.Application;
using Couchbase;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration.GetSection("Couchbase");

builder.Services.AddControllers();

builder.Services.AddScoped<IContratRepository, ContratRepository>();
builder.Services.AddScoped<ContratService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCouchbase(config.GetSection("Couchbase"));
builder.Services.AddCouchbaseBucket<INamedBucketProvider>("mat-sample");
builder.Services.AddCouchbase(config);

var app = builder.Build();

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
