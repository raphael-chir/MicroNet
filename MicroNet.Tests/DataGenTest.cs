namespace MicroNet.Tests;

using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Threading.Tasks;
using Couchbase;
using MicroNet.Domain;

public class DataGenTest
{
    [Fact]
    public async Task DataGenTestAsync()
    {
        var cluster = await Cluster.ConnectAsync(
            // Update these credentials for your Local Couchbase instance!
            "ec2-51-20-133-117.eu-north-1.compute.amazonaws.com, ec2-16-170-172-163.eu-north-1.compute.amazonaws.com, ec2-51-20-133-117.eu-north-1.compute.amazonaws.com",
            "admin",
            "111111");

        // get a bucket reference
        var bucket = await cluster.BucketAsync("mat-sample");

        // get a user-defined collection reference
        var scope = await bucket.ScopeAsync("contrats");
        var collectionContrats = await scope.CollectionAsync("contrats");
        var collectionSituations = await scope.CollectionAsync("situations");

        // data
        string[] operationsSituations = ["AVT", "REC", "RAF"];
        string[] offrePromoCode = ["GOLD", "SILVER", "BRONZE"];
        string[] motivationsResiliations = ["Insatisfaction", "Concurrence"];
        string[] assureNom = ["Gauthier", "Moudou", "Chang", "Zimmer", "Favatti", "Fernandez", "Benton", "Martin", "Dupont", "Joly"];
        string[] assurePrenom = ["Theo", "Aminata", "François", "Hans", "Mario", "Jose", "Gerard", "Lucie", "Josiane", "Gustave"];

        // Contrats generation
        Random aleatoire = new Random();

        for (int k = 0; k < 100; k++)
        {
            int situationsCount = aleatoire.Next(1, 4);
            int yearDebutEffet = aleatoire.Next(2015, 2025);
            int monthDebutEffet = aleatoire.Next(1, 13);
            string numSocietaire = Guid.NewGuid().ToString().Split("-")[4];
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

                var situation = new Situation(idSituation, operationsSituations[opCount], datePremierEffet, new List<Assure>(), new List<Resiliation>(), new List<OffrePromotionnelle>());
                //
                int assuresCount = aleatoire.Next(1, 4);
                for (int j = 0; j < assuresCount; j++)
                {
                    int magic = aleatoire.Next(10);
                    string nom = assureNom[magic];
                    string prenom = assurePrenom[magic];
                    situation.Assures.Add(new Assure(nom, prenom));
                }
                //
                int resCount = aleatoire.Next(2);
                for (int j = 0; j < resCount; j++)
                {
                    int year = aleatoire.Next(2015, 2025);
                    int month = aleatoire.Next(1, 13);
                    int magic = aleatoire.Next(2);
                    situation.Resiliations.Add(new Resiliation(new DateTime(year, month, 15, 16, 30, 0), motivationsResiliations[magic]));
                }
                //
                int offreCount = aleatoire.Next(3);
                for (int j = 0; j < resCount; j++)
                {
                    int year = aleatoire.Next(2015, 2025);
                    int month = aleatoire.Next(1, 13);
                    int magic = aleatoire.Next(3);
                    situation.OffrePromotionnelles.Add(new OffrePromotionnelle(new DateTime(year, month, 15, 16, 30, 0), offrePromoCode[magic]));
                }
                Console.WriteLine(JsonSerializer.Serialize(situation));
                contrat.Situations.Add(new SituationRef(idSituation, datePremierEffet));
                await collectionSituations.InsertAsync(situation.Id, situation);

            }

            Console.WriteLine(JsonSerializer.Serialize(contrat));
            await collectionContrats.InsertAsync(contrat.NumSocietaire + "::" + contrat.Id, contrat);
        }
    }
}