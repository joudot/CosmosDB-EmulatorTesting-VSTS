using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReviewWebsite.Core.Interfaces;
using ReviewWebsite.Core.Model;

namespace CosmosDbClient.Controllers
{
    [Route("api/[controller]")]
    public class ReviewsController : Controller
    {
        private IRepository<ReviewDocument> _reviewRepository;

        public ReviewsController(IRepository<ReviewDocument> reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // GET api/values
        [HttpGet]
        public Task<List<ReviewDocument>> Get()
        {
            return _reviewRepository.ListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Task<ReviewDocument> GetAsync(int id)
        {
            return _reviewRepository.GetByIdAsync(id.ToString());
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]ReviewDocument value)
        {
            try
            {
                var result = await _reviewRepository.AddAsync(value);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Failed to add document {value.Id} - {e.Message}" );
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public Task<ReviewDocument> PutAsync(int id, [FromBody]ReviewDocument value)
        {
            return _reviewRepository.UpdateAsync(value);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public Task Delete(int id)
        {
            return _reviewRepository.DeleteAsync(id.ToString());
        }
    }
}
