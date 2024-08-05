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

public class S06CBKVRangeScanTests : IClassFixture<TestFixture>
{
    private readonly INamedBucketProvider _provider;

    public S06CBKVRangeScanTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider.GetRequiredService<INamedBucketProvider>();
    }

    [Fact]
    public async Task SelectTest()
    {
        var bucket = await _provider.GetBucketAsync();
        var scope = await bucket.ScopeAsync("contrats");
        var collectionContrats = await scope.CollectionAsync("contrats");

        // Create a key societaire::idTech with many contracts
        string numSocietaire = Guid.NewGuid().ToString().Split("-")[4];

        for (int i = 0; i < 3; i++)
        {
            string contratId = Guid.NewGuid().ToString();
            string contratKey = numSocietaire + "::" + contratId;
            string situationId = Guid.NewGuid().ToString();
            var contrat = new Contrat(contratId, numSocietaire, DateTime.Today);
            await collectionContrats.InsertAsync(contratKey, contrat);
        }

        Console.WriteLine("Insertions completed ...");

        // List all contracts of a given societaire 
        IAsyncEnumerable<IScanResult> results = collectionContrats.ScanAsync(new PrefixScan(numSocietaire + "::"));

        await foreach (var scanResult in results)
        {
            Console.WriteLine(scanResult);
        }
    }
}