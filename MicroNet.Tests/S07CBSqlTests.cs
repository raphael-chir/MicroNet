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
            Insert a situation to a given contract
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







        //Permet de créer une résiliation sur un contrat



        //lister les references situation dans le but d'avoir un historique d'un contrat mav

        //Récupère les offres promotionnelles d'un societaire

        //Permet de créer une souscription
        //Récupère un contrat mav
        //Permet de controler une souscription
        //Contrôler l'objet résiliation
        //Permet de créer une résiliation sur un contrat
        //Contrôler l'objet annulation
        //Permet de créer une annulation sur un contrat
        //lister les situations d'un contrat mav
        //lister les references situation dans le but d'avoir un historique d'un contrat mav
        //lister les résiliation d'une situation d'un contrat mav
        //Met à jour les informations de signature de la situation
        //Récupère les offres promotionnelles d'un societaire
        //Liste des Canaux
        //Liste des Origines
        //Liste des Csp
        //Liste des code taxe Pays
        //Liste des Contextes souscription
        //Liste des modes de paiement
        //Liste des erreurs
        //listerMotifResiliation
        //listerSousMotifResiliation
        //Liste les modes de remboursement résiliation
        //Liste les mmotivations societaire résiliation
        //Permet de contrôler la résiliation de tous les contrats MAV d'un sociétaire
        //Permet de résilier tous les contrats MAV d'un sociétaire
        //Permet d'annuler des résiliations d'un sociétaire. A utiliser pour annluer par une résilation globale

    }


}