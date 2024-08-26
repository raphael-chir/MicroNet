namespace MicroNet.Tests;
using Couchbase.KeyValue; // Lambda expression support
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using MicroNet.Domain;
using Couchbase.Core.IO.Operations.Errors;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Couchbase.Core.Exceptions;
using Couchbase.Transactions;
using Couchbase.Transactions.Config;
using Couchbase.Transactions.Error;
using Couchbase.KeyValue.RangeScan;
using Couchbase.Core.IO.Serializers;

public class S08CBTranscoderTests : IClassFixture<TestFixture>
{
    private readonly INamedBucketProvider _provider;

    public S08CBTranscoderTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider.GetRequiredService<INamedBucketProvider>();
    }

    [Fact]
    public async Task test()
    {
        var bucket = await _provider.GetBucketAsync();
        var scope = await bucket.ScopeAsync("contrats");
        var collectionContrats = await scope.CollectionAsync("contrats");

        // Create a key societaire::idTech with many contracts
        string numSocietaire = "6fdf60fb2400";

        // List all contracts of a given societaire 
        IAsyncEnumerable<IScanResult> results = collectionContrats.ScanAsync(new PrefixScan(numSocietaire + "::"));

        await foreach (var scanResult in results)
        {
            Console.WriteLine(scanResult);
        }
    }

    [Fact]
    public async Task testSQL()
    {
        var bucket = await _provider.GetBucketAsync();
        var scope = await bucket.ScopeAsync("contrats");
         var collectionContrats = await scope.CollectionAsync("contrats");
        var result1 = await scope.QueryAsync<Contrat>(
            "SELECT c.* FROM `contrats` c WHERE c.numSocietaire=$numsocietaire",
            options => { 
                options.Parameter("numsocietaire", "6fdf60fb2400");
                }
        );

        var result2 = await collectionContrats.GetAsync("095e7ddd61b8::29d0d9a5-d8a1-4dac-8530-2ed8bb5f9c4f");
        Console.WriteLine(result2.ContentAs<Contrat>().NumSocietaire);

        IAsyncEnumerable<Contrat> contrats = result1.Rows;

        //iterate over rows
        await foreach (var contrat in contrats)
        {
            Console.WriteLine(contrat.Id);
        }
    }
}