namespace MicroNet.Tests;

using System.Threading.Tasks;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

public class CBCnxCBDICnxTest : IClassFixture<TestFixture>
{
    private readonly INamedBucketProvider _provider;

    public CBCnxCBDICnxTest(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider.GetRequiredService<INamedBucketProvider>();
    }
    [Fact]
    public async Task DependencyInjectionTestAsync()
    {
        var bucket = await _provider.GetBucketAsync();
        var scope = await bucket.ScopeAsync("tenant_agent_00");
        var collection = await scope.CollectionAsync("users");

        // Upsert Document
        var upsertResult = await collection.UpsertAsync("my-document-key", new { Name = "Ted", Age = 31 });
        var getResult = await collection.GetAsync("my-document-key");

        Console.WriteLine(getResult.ContentAs<dynamic>());

        // Call the QueryAsync() function on the scope object and store the result.
        var inventoryScope = bucket.Scope("inventory");
        var queryResult = await inventoryScope.QueryAsync<dynamic>("SELECT * FROM airline WHERE id = 10");
        
        // Iterate over the rows to access result data and print to the terminal.
        await foreach (var row in queryResult) {
            Console.WriteLine(row);
        }
    }
}