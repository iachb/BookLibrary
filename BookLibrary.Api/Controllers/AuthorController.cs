using AutoMapper;
using BookLibrary.Api.Models.Author;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Models.Authors;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public readonly IMapper _mapper;
        public readonly IAuthorService _authorService;

        public AuthorController(IMapper mapper, IAuthorService authorService)
        {
            this._mapper = mapper;
            this._authorService = authorService;
        }

        // GET: api/<AuthorController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthorController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AuthorController>
        [HttpPost]
        public async Task<ActionResult<AuthorDTO>> CreateAuthor([FromBody] CreateAuthorRequestDTO request, CancellationToken cancellationToken)
        {
            var item = _mapper.Map<AuthorItem>(request);
            var createdAuthor = await _authorService.CreateAuthorAsync(item, cancellationToken);
            return Ok(_mapper.Map<AuthorDTO>(createdAuthor));
        }

        // PUT api/<AuthorController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<AuthorController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
