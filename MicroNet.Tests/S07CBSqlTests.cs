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

public class S07CBSqlTests : IClassFixture<TestFixture>
{
    private readonly INamedBucketProvider _provider;

    public S07CBSqlTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider.GetRequiredService<INamedBucketProvider>();
    }

    [Fact]
    public async Task SqlTest()
    {
        var bucket = await _provider.GetBucketAsync();
        var scope = await bucket.ScopeAsync("contrats");
        var collectionContrats = await scope.CollectionAsync("contrats");

        /*
        ********************************************************************************************
            Get all contracts of a given societaire
            SELECT c.* FROM `contrats` c WHERE c.numSocietaire=?
        ********************************************************************************************
        */
        var result = await scope.QueryAsync<Contrat>(
            "SELECT c.* FROM `contrats` c WHERE c.numSocietaire=$numsocietaire",
            options => options.Parameter("numsocietaire", "13c63f6edceb")
);

        IAsyncEnumerable<Contrat> contrats = result.Rows;

        // iterate over rows
        await foreach (var contrat in contrats)
        {
            Console.WriteLine(contrat.Id);
        }

        /*
         ********************************************************************************************
            List situations ref of a given contract id
            SELECT c.situationsRef FROM contrats c WHERE c.id=?
            Or return only ids array
            SELECT c.situationsRefs[].id FROM contrats c WHERE c.id="4bf8d3b1-9f08-4804-8376-bf446d38b6f8"
         ********************************************************************************************
        */

        /*
        ********************************************************************************************
           List resiliations of a given situation id
           SELECT s.resiliations[] FROM situations s WHERE s.id=?
        ********************************************************************************************
       */

        /*
         ********************************************************************************************
            List situations of a given contract id
            SELECT s.* FROM contrats c JOIN situations s ON KEYS ARRAY id FOR id IN c.situationsRefs[].id END WHERE c.id=?
         ********************************************************************************************
        */

        /*
        ********************************************************************************************
           List offres promotionnelles of a given societaire
           SELECT s.offrePromotionnelles[] FROM contrats c 
           JOIN situations s ON KEYS ARRAY id FOR id IN c.situationsRefs[].id END 
           WHERE c.numSocietaire="13c63f6edceb"
        ********************************************************************************************
       */

        /*
         ********************************************************************************************
            List contracts where a given assure name appears
            SELECT distinct c.id FROM contrats c 
            JOIN situations s ON KEYS ARRAY id FOR id IN c.situationsRefs[].id END 
            WHERE ANY assure IN s.assures SATISFIES assure.nom="Favatti" END
         ********************************************************************************************
        */

        /*
        ********************************************************************************************
           List all distinct assures from the database
           SELECT distinct a.nom, a.prenom FROM situations s UNNEST assures a
        ********************************************************************************************
       */

        /*
        ********************************************************************************************
           List all contracts containing a situation ref which datePremierEffet < 2019
           SELECT * FROM contrats c
           WHERE ANY situationRef IN c.situationsRefs[] SATISFIES DATE_DIFF_STR(situationRef.datePremierEffet,"2019-01-01T00:00:00","year")<0 end

           List all contracts containing a situation ref which datePremierEffet = 2019
           SELECT * from contrats c
           WHERE ANY situationRef IN c.situationsRefs[] SATISFIES DATE_FORMAT_STR(situationRef.datePremierEffet,"YYYY")="2021" end
        ********************************************************************************************
       */

        /*
         ********************************************************************************************
            Insert a situation to a given contract (can replace with UPSERT to update or insert)
            INSERT INTO situations (key, {})
         ********************************************************************************************
        */

        Console.WriteLine(JsonSerializer.Serialize(new Situation(Guid.NewGuid().ToString(), SampleDataValues.operationsSituations[0], DateTime.Now)));
        var insert = await scope.QueryAsync<Contrat>(
            "INSERT INTO situations VALUES ($key, $value)",
            options =>
            {
                options.Parameter("key", Guid.NewGuid().ToString());
                options.Parameter("value", new Situation(Guid.NewGuid().ToString(), SampleDataValues.operationsSituations[0], DateTime.Now));
            }
        );

        /*
         ********************************************************************************************
            Update a situation to a given contract
            INSERT INTO situations (key, {})
         ********************************************************************************************
        */
    }
}