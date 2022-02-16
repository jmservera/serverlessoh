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
    public static class CreateRating{
    
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ratings", 
                collectionName: "ratingscollection", 
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<RatingInfo> documentsOut,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RatingInfo rating = JsonConvert.DeserializeObject<RatingInfo>(requestBody);

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://serverlessohapi.azurewebsites.net");
                    var result = await client.GetAsync($"/api/GetProduct?productId={rating.ProductId}");
                    string resultBody = await result.Content.ReadAsStringAsync();
                    Product product = JsonConvert.DeserializeObject<Product>(resultBody);
                }
            }
            catch (System.Exception ex)
            {   
                return new NotFoundObjectResult($"Could not find product with id: {rating.ProductId}");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://serverlessohapi.azurewebsites.net");
                    var result = await client.GetAsync($"/api/GetUser?userId={rating.UserId}");
                    string resultBody = await result.Content.ReadAsStringAsync();
                    User user = JsonConvert.DeserializeObject<User>(resultBody);
                }
            }
            catch (System.Exception ex)
            {
                return new NotFoundObjectResult($"Could not find user with id: {rating.UserId}");

            }

             // Add a property called id with a GUID value
            rating.Id = Guid.NewGuid().ToString();

            // Add a property called timestamp with the current UTC date time
            rating.Timestamp = DateTime.UtcNow;

            // Validate that the rating field is an integer from 0 to 5
            if (rating.Rating < 0 || rating.Rating > 5)
            {
                return new BadRequestObjectResult($"Rating must be between 0 and 5");
            }

            // Use a data service to store the ratings information to the backend
            await documentsOut.AddAsync(rating);

            return new OkObjectResult(rating);
        }
    }
}
