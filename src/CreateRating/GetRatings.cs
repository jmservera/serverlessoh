using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Team5_OH.Function
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetRatings/{userId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "ratings", 
                collectionName: "ratingscollection", 
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.userId={userId} ORDER BY c.timestamp DESC")] IEnumerable<RatingInfo> ratings,
            ILogger log)
        {            
            if(ratings==null || ratings.Count()==0)
            {
                return new NotFoundObjectResult("No ratings found");
            }

            return new OkObjectResult(ratings);
        }
    }
}
