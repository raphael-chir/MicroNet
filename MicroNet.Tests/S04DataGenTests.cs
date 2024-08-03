namespace MicroNet.Tests;

using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Extensions.DependencyInjection;
using MicroNet.Domain;
using Microsoft.Extensions.DependencyInjection;

public class S04DataGenTests : IClassFixture<TestFixture>
{
    private readonly INamedBucketProvider _provider;

    public S04DataGenTests(TestFixture fixture)
    {
        _provider = fixture.ServiceProvider.GetRequiredService<INamedBucketProvider>();
    }
    [Fact]
    public async Task DataGenTestAsync()
    {
        var bucket = await _provider.GetBucketAsync();
        var scope = await bucket.ScopeAsync("contrats");
        var collectionContrats = await scope.CollectionAsync("contrats");
        var collectionSituations = await scope.CollectionAsync("situations");

        string numSocietaire = Guid.NewGuid().ToString().Split("-")[4]; ;

        // Contrats generation
        Random aleatoire = new Random();

        for (int k = 0; k < 10; k++)
        {
            int situationsCount = aleatoire.Next(1, 4);
            int yearDebutEffet = aleatoire.Next(2015, 2025);
            int monthDebutEffet = aleatoire.Next(1, 13);
            if (aleatoire.Next(2) == 0)
            {
                numSocietaire = Guid.NewGuid().ToString().Split("-")[4];
            }

            string idContrat = Guid.NewGuid().ToString();

            var contrat = new Contrat(idContrat, numSocietaire, new DateTime(yearDebutEffet, monthDebutEffet, 18, 16, 32, 0), new List<SituationRef>());

            for (int i = 0; i < situationsCount; i++)
            {
                string idSituation = Guid.NewGuid().ToString();
                int opCount = aleatoire.Next(3);
                DateTime datePremierEffet;
                if (i == 0)
                {
                    datePremierEffet = new DateTime(yearDebutEffet, monthDebutEffet, 18, 16, 32, 0);
                }
                else
                {
                    int year = aleatoire.Next(yearDebutEffet, 2025);
                    int month = aleatoire.Next(1, 13);
                    datePremierEffet = new DateTime(year, month, 18, 16, 32, 0);
                }

                var situation = new Situation(idSituation, SampleDataValues.operationsSituations[opCount], datePremierEffet, new List<Assure>(), new List<Resiliation>(), new List<OffrePromotionnelle>());
                //
                int assuresCount = aleatoire.Next(1, 4);
                for (int j = 0; j < assuresCount; j++)
                {
                    int magic = aleatoire.Next(10);
                    string nom = SampleDataValues.assureNom[magic];
                    string prenom = SampleDataValues.assurePrenom[magic];
                    situation.Assures.Add(new Assure(nom, prenom));
                }
                //
                int resCount = aleatoire.Next(2);
                for (int j = 0; j < resCount; j++)
                {
                    int year = aleatoire.Next(2015, 2025);
                    int month = aleatoire.Next(1, 13);
                    int magic = aleatoire.Next(2);
                    situation.Resiliations.Add(new Resiliation(new DateTime(year, month, 15, 16, 30, 0), SampleDataValues.motivationsResiliations[magic]));
                }
                //
                int offreCount = aleatoire.Next(3);
                for (int j = 0; j < resCount; j++)
                {
                    int year = aleatoire.Next(2015, 2025);
                    int month = aleatoire.Next(1, 13);
                    int magic = aleatoire.Next(3);
                    situation.OffrePromotionnelles.Add(new OffrePromotionnelle(new DateTime(year, month, 15, 16, 30, 0), SampleDataValues.offrePromoCode[magic]));
                }
                Console.WriteLine(JsonSerializer.Serialize(situation));
                contrat.SituationsRefs.Add(new SituationRef(idSituation, datePremierEffet));
                await collectionSituations.InsertAsync(situation.Id, situation);

            }

            Console.WriteLine(JsonSerializer.Serialize(contrat));
            await collectionContrats.InsertAsync(contrat.NumSocietaire + "::" + contrat.Id, contrat);
        }
    }
}