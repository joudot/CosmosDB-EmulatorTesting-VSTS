using CosmosDbClient.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReviewWebsite.Core.Model;
using ReviewWebsite.Infrastructure;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CosmosDbTests
{
    public class ReviewTest : IClassFixture<CosmosDBFixture>
    {
        private readonly ITestOutputHelper _output;
        private CosmosDBFixture _cosmosDBFixture;

        public ReviewTest(ITestOutputHelper output, CosmosDBFixture cosmosDBFixture)
        {
            _cosmosDBFixture = cosmosDBFixture;
            _output = output;
        }


        [Fact]
        public async Task CheckNoDuplicateReviewIsAllowed()
        {
            _output.WriteLine($"EndPointUrl: {_cosmosDBFixture.CosmosDbConfig.EndPointUrl}");
            _output.WriteLine($"CollectionName: {_cosmosDBFixture.CosmosDbConfig.CollectionName}");
            _output.WriteLine($"DatabaseName: {_cosmosDBFixture.CosmosDbConfig.DatabaseName}");
            _output.WriteLine($"AuthorizationKey: {_cosmosDBFixture.CosmosDbConfig.AuthorizationKey}");

            var repository = new ReviewRepository(_cosmosDBFixture.CosmosDbConfig);

            var controller = new ReviewsController(repository);

            var newReview1 = new ReviewDocument
            {
                Id = "review2",
                Content = "Another review with existing Id"
            };

            var newReview2 = new ReviewDocument
            {
                Id = "review3",
                Content = "This is boring"
            };

            await controller.PostAsync(newReview1);
            await controller.PostAsync(newReview2);

            var reviews = await controller.Get();
            // We rely on the fact that we already had review2 
            // and another review in our test data
            Assert.Equal(3, reviews.Count);
        }
    }
}
