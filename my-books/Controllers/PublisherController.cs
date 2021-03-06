using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using my_books.Data.Services;
using my_books.Data.ViewModels;
using my_books.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private PublisherService _publisherService;
        private readonly ILogger<PublisherController> _logger;

        public PublisherController(PublisherService publisherService, ILogger<PublisherController> logger)
        {
            _publisherService = publisherService;
            _logger = logger;
        }

        [HttpGet("get-all-publishers")]
        public IActionResult GetAllPublisers(string sortby, string searchString, int pageNumber)
        {
           try
            {
                _logger.LogInformation("This is just a log in GetAllPulishers()");
                var _result = _publisherService.GetAllPublishers(sortby, searchString, pageNumber);
                return Ok(_result);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, We could not load the publisers");
            }
            
        }

        [HttpPost("add-publisher")]
        public IActionResult AddPublisher([FromBody] PublisherVM publisher)
        {
            try
            {
                var newPublisher = _publisherService.AddPublisher(publisher);
                return Created(nameof(AddPublisher), newPublisher);
            }
            catch(PublisherNameException ex)
            {
                return BadRequest($"{ex.Message}, Publisher name : {ex.PublisherName}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [HttpGet("get-publisher-by-id/{id}")]
        public IActionResult GetPublisherById(int id)
        {
           // throw new Exception("this is an exception that will be hanlded by middleware"); 
            var _response = _publisherService.GetPublisherById(id);
            if (_response != null)
                return Ok(_response);
            else
                return NotFound();
        }

        [HttpGet("get-publisher-books-with-authors/{id}")]
        public IActionResult GetPublisherData(int id)
        {
            var _response = _publisherService.GetPublisherData(id);
            return Ok(_response);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisherById(int id)
        {
            try
            {
                _publisherService.DeletePublisherById(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
