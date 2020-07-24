using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections.Generic;
using static SqlExtensionSamples.ProductUtilities;

namespace SqlExtensionSamples.InputBindingSamples
{
    public static class GetProductsBuffered
    {
        [FunctionName("GetProductsBuffered")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getproducts-buffered/{cost}")]
        HttpRequest req,
            [Sql("select * from Products where cost = @Cost",
            CommandType = System.Data.CommandType.Text,
            Parameters = "@Cost={cost}",
            ConnectionStringSetting = "SQLServerAuthentication",
            Buffered = true)]
        IEnumerable<Product> products)
        {
            var enumerator = products.GetEnumerator();
            var list = new List<Product>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return (ActionResult)new OkObjectResult(list);
        }
        
    }
}
