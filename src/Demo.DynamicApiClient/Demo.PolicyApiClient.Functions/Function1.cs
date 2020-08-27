using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Demo.PolicyApiClient.Functions
{
    public static partial class Function1
    {
        private static HttpClient HttpClient = new HttpClient();
        private static Random NumberGenerator = new Random(Guid.NewGuid().GetHashCode());

        [FunctionName(nameof(GetPersons))]
        public static async Task<IActionResult> GetPersons(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "persons")] HttpRequest req,
            ILogger log)
        {
            var persons = new List<Person>
            {
                new Person{FirstName = "Todd", LastName = "Meinershagen", BirthDate = new DateTime(1972, 11 ,23)},
                new Person{FirstName = "Bill", LastName = "Willits", BirthDate = new DateTime(1980, 5 , 10)},
            };

            return await Task.FromResult(new OkObjectResult(persons));
        }

        [FunctionName(nameof(GetTimeCards))]
        public static async Task<IActionResult> GetTimeCards(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "timecards")] HttpRequest req,
            ILogger log)
        {
            var persons = new List<TimeCard>
            {
                new TimeCard{Id = Guid.NewGuid(), DueDate = DateTime.Now.Date.AddDays(2) },
                new TimeCard{Id = Guid.NewGuid(), DueDate = DateTime.Now.Date.AddDays(5) },
            };

            return await Task.FromResult(new OkObjectResult(persons));
        }

        [FunctionName(nameof(GetMatchingItems))]
        public static async Task<IActionResult> GetMatchingItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "matchingItems")] HttpRequest req,
            ILogger log)
        {
            var filterType = NumberGenerator.Next(1, 100) % 2 == 0 ? FilterType.Person : FilterType.TimeCard;
            var url = filterType == FilterType.Person
                ? "/api/persons" 
                : "/api/timecards";

            var items = await GetAsync(url);

            ISpecification specification = filterType == FilterType.Person
                ? new PersonSpecification() as ISpecification
                : new TimeCardSpecification();

            var matchingItems = items.Where(x => specification.IsSatisfiedBy(x));
            return await Task.FromResult(new OkObjectResult(matchingItems));
        }

        private static async Task<List<dynamic>> GetAsync(string url)
        {
            var uri = new Uri($"http://localhost:7071{url}");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<dynamic>>();
        }
    }
}
