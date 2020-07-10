using ContosoCrafts.WebSite.Models;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ContosoCrafts.WebSite.Services
{
    public class JsonFileProductService
    {
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; set; }

        private string JsonsFileName
        {
            get { return Path.Combine(WebHostEnvironment.WebRootPath, "data", "products.json"); }
        }

        public IEnumerable<Product> GetProducts()
        {
            using(StreamReader jsonFileReader = File.OpenText(JsonsFileName))
            {
                return JsonSerializer.Deserialize<Product[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});
            }
        }

        public void AddRating(string productId, int rating)
        {
            IEnumerable<Product> products = GetProducts();

            //LINQ
            var query = products.FirstOrDefault(x => x.Id == productId);

            if(query == null) { return; }

            if(query.Ratings == null)
            {
                query.Ratings = new int[] { rating };
            }
            else
            {
                List<int> ratings = query.Ratings.ToList();
                ratings.Add(rating);
                query.Ratings = ratings.ToArray();
            }

            using(var outputStream = File.OpenWrite(JsonsFileName))
            {
                JsonSerializer.Serialize<IEnumerable<Product>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions { 
                        SkipValidation = true, Indented = true
                    }),
                    products);
            }
        }
    }
}
