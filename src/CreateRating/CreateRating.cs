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
            string productCheckStatusCode = null;
            string userCheckStatusCode = null;
            string responseMessage = null;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userId = userId ?? data?.userId;
            productId = productId ?? data?.productId;
            locationName = locationName ?? data?.locationName;
            rating = data?.rating;
            userNotes = userNotes ?? data?.userNotes;
            var queryGetProduct = new Dictionary<string, string>()
            {
                ["productId"] = productId
            };
            var uriGetProduct = QueryHelpers.AddQueryString("https://serverlessohapi.azurewebsites.net/api/GetProduct", queryGetProduct);
            HttpResponseMessage responseGetproduct = await client.GetAsync(uriGetProduct);
            if (responseGetproduct.IsSuccessStatusCode)
            {
                productCheckStatusCode = responseGetproduct.StatusCode.ToString();
            }
            var queryGetUser = new Dictionary<string, string>()
            {
                ["userId"] = userId
            };
            var uriGetUser = QueryHelpers.AddQueryString("https://serverlessohapi.azurewebsites.net/api/GetUser", queryGetUser);
            HttpResponseMessage responseGetUser = await client.GetAsync(uriGetUser);
            if (responseGetUser.IsSuccessStatusCode)
            {
                userCheckStatusCode = responseGetUser.StatusCode.ToString();
            }
            if(responseGetproduct.IsSuccessStatusCode && responseGetUser.IsSuccessStatusCode){
                if(rating >= 0 && rating <= 5){
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
                    responseMessage = $"Rating is successfully created!";  
                }
                else{
                    responseMessage = $"Rating cannot be created! Rating is not a valid number";
                }
                
            }
            else{
                responseMessage = $"Rating cannot be created!";
            }
            return new OkObjectResult(responseMessage);
        }
    }
}
