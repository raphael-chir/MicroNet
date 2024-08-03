namespace MicroNet.Tests;

using System.Threading.Tasks;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

public class S01CBDICnxTests : IClassFixture<TestFixture>
{
    private readonly INamedBucketProvider _provider;

    public S01CBDICnxTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider.GetRequiredService<INamedBucketProvider>();
    }
    [Fact]
    public async Task DependencyInjectionTestAsync()
    {
        var bucket = await _provider.GetBucketAsync();
        Console.WriteLine(bucket.PingAsync());
    }
}