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

public class S02CBKVBasicTests : IClassFixture<TestFixture>
{
    private readonly INamedBucketProvider _provider;

    public S02CBKVBasicTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider.GetRequiredService<INamedBucketProvider>();
    }

    [Fact]
    public async Task CreateTest()
    {
        var bucket = await _provider.GetBucketAsync();
        var scope = await bucket.ScopeAsync("contrats");
        var collectionContrats = await scope.CollectionAsync("contrats");
        var collectionSituations = await scope.CollectionAsync("situations");

        // Create a key societaire::idTech
        string numSocietaire = Guid.NewGuid().ToString().Split("-")[4];
        string id = Guid.NewGuid().ToString();
        string key = numSocietaire + "::" + id;

        try
        {
            //Create a doc without DO
            var document = new { id = id, numSocietaire = numSocietaire, dateDePremierEffet = DateTime.Today };

            var result = await collectionContrats.InsertAsync(key, document, options =>
                {
                    options.Expiry(TimeSpan.FromSeconds(5));
                }
            );
        }
        catch (DocumentExistsException)
        {
            // handle exception
        }
    }

    [Fact]
    public async Task CreateDOTest()
    {
        var bucket = await _provider.GetBucketAsync();
        var scope = await bucket.ScopeAsync("contrats");
        var collectionContrats = await scope.CollectionAsync("contrats");
        var collectionSituations = await scope.CollectionAsync("situations");

        // Create a key societaire::idTech
        string numSocietaire = Guid.NewGuid().ToString().Split("-")[4];
        string id = Guid.NewGuid().ToString();
        string key = numSocietaire + "::" + id;

        try
        {
            //Create a DO
            var document = new Contrat(id, numSocietaire, DateTime.Today, new List<SituationRef>());

            var result = await collectionContrats.InsertAsync(key, document, options =>
                {
                    options.Expiry(TimeSpan.FromSeconds(5));
                }
            );
        }
        catch (DocumentExistsException)
        {
            // handle exception
        }
    }

    [Fact]
    public async Task CRUDTest()
    {
        var bucket = await _provider.GetBucketAsync();
        var scope = await bucket.ScopeAsync("contrats");
        var collectionContrats = await scope.CollectionAsync("contrats");
        var collectionSituations = await scope.CollectionAsync("situations");

        // Create a key societaire::idTech
        string numSocietaire = Guid.NewGuid().ToString().Split("-")[4];
        string id = Guid.NewGuid().ToString();
        string key = numSocietaire + "::" + id;
        DateTime now = DateTime.Now;

        try
        {
            Contrat contrat = new Contrat(id, numSocietaire, now, new List<SituationRef>());

            // Create
            IMutationResult mutationResult = await collectionContrats.InsertAsync(key, contrat, options =>
                {
                    options.Durability(DurabilityLevel.PersistToMajority);
                    options.Expiry(TimeSpan.FromSeconds(60));
                }
            );

            // Read
            var getResult = await collectionContrats.GetAsync(key);
            var contratToControl = getResult.ContentAs<Contrat>();

            Assert.NotNull(contratToControl);
            Assert.Equal(id, contratToControl.Id);
            Assert.Equal(numSocietaire, contratToControl.NumSocietaire);
            Assert.Equal(now, contratToControl.DatePremierEffet);
            Assert.Empty(contratToControl.SituationsRefs);

            // Update
            contrat.DatePremierEffet = new DateTime(2023,08,12,9,30,54);
            Console.WriteLine(contrat.DatePremierEffet);
            //to make sure document wont't be updated in the same time
            var cas = getResult.Cas;
            IMutationResult mutationResultReplace = await collectionContrats.ReplaceAsync<Contrat>(key, contrat, options => {
                options.Cas(cas);
            });
            //Or insert or update method IMutationResult mutationResultReplace = await collectionContrats.UpsertAsync<Contrat>(key, contrat);
            //But upsert doesn't handle cas
            var getResultTest2 = await collectionContrats.GetAsync(key);
            var contratToControl2 = getResultTest2.ContentAs<Contrat>();

            Assert.NotNull(contratToControl2);
            Assert.Equal(id, contratToControl2.Id);
            Assert.Equal(numSocietaire, contratToControl2.NumSocietaire);
            Assert.NotEqual(now, contratToControl2.DatePremierEffet);
            Assert.Empty(contratToControl2.SituationsRefs);

            // Delete
            await collectionContrats.RemoveAsync(key, options => {
                options.Cas(cas);
            });

        }
        catch (DocumentExistsException)
        {
            // handle exception
        }
        catch (DocumentNotFoundException)
        {
            // handle exception
        }
        catch (CasMismatchException)
        {
            // handle exception
        }
    }

}