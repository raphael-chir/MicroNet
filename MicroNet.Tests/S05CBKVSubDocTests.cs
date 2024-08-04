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

public class S05CBKVSubDocTests : IClassFixture<TestFixture>
{
    private readonly INamedBucketProvider _provider;

    public S05CBKVSubDocTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider.GetRequiredService<INamedBucketProvider>();
    }

    [Fact]
    public async Task LookUpInTest()
    {
        // We need a ref to cluster to perform transaction
        var cluster = await Cluster.ConnectAsync(
            // Update these credentials for your Local Couchbase instance!
            "ec2-51-20-133-117.eu-north-1.compute.amazonaws.com, ec2-16-170-172-163.eu-north-1.compute.amazonaws.com, ec2-51-20-133-117.eu-north-1.compute.amazonaws.com",
            "admin",
            "111111");

        var bucket = await cluster.BucketAsync("mat-sample");
        var scope = await bucket.ScopeAsync("contrats");
        var collectionContrats = await scope.CollectionAsync("contrats");
        var collectionSituations = await scope.CollectionAsync("situations");

        // Create a key societaire::idTech
        string numSocietaire = Guid.NewGuid().ToString().Split("-")[4];
        string contratId = Guid.NewGuid().ToString();
        string contratKey = numSocietaire + "::" + contratId;
        string situationId = Guid.NewGuid().ToString();

        //Create Situation DO to store
        DateTime datePremierEffetSituation = DateTime.Now;
        var situation = new Situation(situationId, SampleDataValues.operationsSituations[0], datePremierEffetSituation);
        situation.Assures.Add(new Assure(SampleDataValues.assureNom[0], SampleDataValues.assurePrenom[0]));
        situation.OffrePromotionnelles.Add(new OffrePromotionnelle(DateTime.Now, SampleDataValues.offrePromoCode[0]));
        //Create Contrat DO to store
        var contrat = new Contrat(contratId, numSocietaire, DateTime.Today);
        contrat.SituationsRefs.Add(new SituationRef(situationId, datePremierEffetSituation));

        // Create the single Transactions object
        var transactions = Transactions.Create(cluster, TransactionConfigBuilder.Create());

        try
        {
            //Create a transaction to store the 2 DOs as separate document
            await transactions.RunAsync(async (ctx) =>
                {
                    await ctx.InsertAsync(collectionSituations, situationId, situation).ConfigureAwait(true); // in XUnit we can't parralelize operation
                    await ctx.InsertAsync(collectionContrats, contratKey, contrat).ConfigureAwait(true); // in XUnit we can't parralelize operation
                    await ctx.CommitAsync().ConfigureAwait(true);
                }).ConfigureAwait(true);
        }
        catch (TransactionCommitAmbiguousException e)
        {
            Console.WriteLine("Transaction possibly committed");
            Console.WriteLine(e);
        }
        catch (TransactionFailedException e)
        {
            Console.WriteLine("Transaction did not reach commit point");
            Console.WriteLine(e);
        }

        // LookUpIn
        ILookupInResult lookupInResult = await collectionSituations.LookupInAsync(situationId, [
            LookupInSpec.Get("assures[0].nom"),
            LookupInSpec.Get("assures[0].prenom"),
            LookupInSpec.Get("offrePromotionnelles[0]")
        ]);

        var nom = lookupInResult.ContentAs<string>(0);
        var prenom = lookupInResult.ContentAs<string>(1);
        var offrePromotionnelles = lookupInResult.ContentAs<OffrePromotionnelle>(2);

        Console.WriteLine(nom + " " + prenom);
        Console.WriteLine("Offre : " + offrePromotionnelles);
        Console.WriteLine("Offre : " + offrePromotionnelles?.Code);

        // LookUpIn with lambda exp
        ILookupInResult lookupInLambdaResult = await collectionSituations.LookupInAsync(situationId, specs =>
        {
            specs.Get("offrePromotionnelles[0].dateDebutEffet");
            specs.Get("offrePromotionnelles[0].code");
        });

        Console.WriteLine(lookupInLambdaResult.ContentAs<string>(0) + " " + lookupInLambdaResult.ContentAs<string>(1));

        //MutateIn
        await collectionSituations.MutateInAsync(situationId, specs =>
        {
            specs.Replace("operation", SampleDataValues.operationsSituations[1]);
            specs.ArrayAppend("assures",new Assure("Chir", "Raphael"));
            specs.Insert("dateFinEffet", DateTime.Now);
        });

    }
}