using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace Demo.PolicyApiClient.Functions
{
    public static partial class Functions
    {
        private static HttpClient HttpClient = new HttpClient();
        private static Random NumberGenerator = new Random(Guid.NewGuid().GetHashCode());

        [FunctionName(nameof(GetPersons))]
        public static async Task<IActionResult> GetPersons(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "persons")] HttpRequest req,
            ILogger log)
        {
            return await Task.FromResult(new OkObjectResult(Persons));
        }

        [FunctionName(nameof(GetPersonsAsReminderItems))]
        public static async Task<IActionResult> GetPersonsAsReminderItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "persons/reminderitems")] HttpRequest req,
            ILogger log)
        {
            var items = Persons.Select(x => new ReminderItem { 
                Id = Guid.NewGuid().ToString(),
                Type = "persons/reminderitems",
                ReferenceDate = x.BirthDate, 
                OwnedBy = Guid.NewGuid().ToString(), 
                LinkToItem = "" ,
                AdditionalProperties = new Dictionary<string, object> { { "FirstName", x.FirstName }, { "LastName", x.LastName } }
            });
            return await Task.FromResult(new OkObjectResult(items));
        }

        private static List<Person> Persons = new List<Person>
            {
                new Person{FirstName = "Todd", LastName = "Meinershagen", BirthDate = new DateTime(1972, 11 ,23)},
                new Person{FirstName = "Bill", LastName = "Willits", BirthDate = new DateTime(1980, 5 , 10)},
            };

        [FunctionName(nameof(GetTimeCards))]
        public static async Task<IActionResult> GetTimeCards(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "timecards")] HttpRequest req,
            ILogger log)
        {
            return await Task.FromResult(new OkObjectResult(TimeCards));
        }

        [FunctionName(nameof(GetTimeCardsAsReminderItems))]
        public static async Task<IActionResult> GetTimeCardsAsReminderItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "timecards/reminderitems")] HttpRequest req,
            ILogger log)
        {
            var items = TimeCards.Select(x => new ReminderItem { 
                Id = Guid.NewGuid().ToString(), 
                Type = "timecards/reminderitems",
                ReferenceDate = x.DueDate, 
                OwnedBy = Guid.NewGuid().ToString(), 
                LinkToItem = "",
                AdditionalProperties = new Dictionary<string, object>()
            });
            return await Task.FromResult(new OkObjectResult(items));
        }

        private static List<TimeCard> TimeCards = new List<TimeCard>
            {
                new TimeCard{Id = Guid.NewGuid(), DueDate = DateTime.Now.Date.AddDays(2) },
                new TimeCard{Id = Guid.NewGuid(), DueDate = DateTime.Now.Date.AddDays(5) },
            };

        [FunctionName(nameof(GetMatchingItemsByDynamic))]
        public static async Task<IActionResult> GetMatchingItemsByDynamic(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "matchingItems/bydynamic")] HttpRequest req,
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

        [FunctionName(nameof(GetMatchingItemsByReminderItems))]
        public static async Task<IActionResult> GetMatchingItemsByReminderItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "matchingItems/byreminderitems")] HttpRequest req,
            ILogger log)
        {
            var filterType = NumberGenerator.Next(1, 100) % 2 == 0 ? FilterType.Person : FilterType.TimeCard;
            var url = filterType == FilterType.Person
                ? "/api/persons/reminderitems"
                : "/api/timecards/reminderitems";

            var items = await GetAsync<ReminderItem>(url);

            var matchingItems = filterType == FilterType.Person
                ? items.AsQueryable().Where("ReferenceDate == @0", new DateTime(1972, 11, 23))
                : items.AsQueryable().Where("ReferenceDate == @0", DateTime.Now.Date.AddDays(2));

            return await Task.FromResult(new OkObjectResult(matchingItems));
        }

        private static async Task<List<dynamic>> GetAsync(string url)
        {
            return await GetAsync<dynamic>(url);
        }

        private static async Task<List<T>> GetAsync<T>(string url)
        {
            var uri = new Uri($"http://localhost:7071{url}");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<T>>();
        }
    }
}
