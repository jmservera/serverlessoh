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
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetRating/{ratingId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "ratings", 
                collectionName: "ratingscollection", 
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.Id={ratingId}")] IEnumerable<RatingInfo> rating,
            ILogger log)
        {            
            if(rating==null || rating.Count()==0)
            {
                return new NotFoundObjectResult("No rating found");
            }

            return new OkObjectResult(rating.First());
        }
    }
}