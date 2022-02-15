using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;

namespace Team5_OH.Function
{

    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ratings", 
                collectionName: "ratingscollection", 
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            HttpClient client = new HttpClient();

            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];
            string userId = null;
            string productId = null;
            string locationName = null;
            int rating = 0;
            string userNotes = null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userId = userId ?? data?.userId;
            productId = productId ?? data?.productId;
            locationName = locationName ?? data?.locationName;
            rating = data?.rating;
            userNotes = userNotes ?? data?.userNotes;

            var queryforgetproduct = new Dictionary<string, string>()
            {
                ["productId"] = productId
            };

            var uriforgetproduct = QueryHelpers.AddQueryString("https://serverlessohapi.azurewebsites.net/api/GetProduct", queryforgetproduct);

            HttpResponseMessage responsegetproduct = await client.GetAsync(uriforgetproduct);
            if (responsegetproduct.IsSuccessStatusCode)
            {
                //product = await response.Content.ReadAsAsync<Product>();
            }

            var queryforgetuser = new Dictionary<string, string>()
            {
                ["userId"] = userId
            };

            var uriforgetuser = QueryHelpers.AddQueryString("https://serverlessohapi.azurewebsites.net/api/GetUser", queryforgetuser);

            HttpResponseMessage responseget = await client.GetAsync(uriforgetuser);
            if (responsegetproduct.IsSuccessStatusCode)
            {
                //product = await response.Content.ReadAsAsync<Product>();
            }

            // Add a JSON document to the output container.
            await documentsOut.AddAsync(new
            {
                // create a random ID
                id = System.Guid.NewGuid().ToString(),
                userId = userId,
                productId = productId,
                timestamp = DateTime.UtcNow,
                locationName = locationName,
                rating = rating,
                userNotes = userNotes
            });           

            string responseMessage = "test";
            /*string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";*/

            return new OkObjectResult(responseMessage);
        }
    }
}
