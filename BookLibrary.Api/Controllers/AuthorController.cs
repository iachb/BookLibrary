using AutoMapper;
using BookLibrary.Api.Models.Author;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Models.Authors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookLibrary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public readonly IMapper _mapper;
        public readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService, IMapper mapper)
        {
            this._mapper = mapper;
            this._authorService = authorService;
        }

        // GET: api/<AuthorController>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AuthorDTO>>> GetAllAuthors(CancellationToken cancellationToken)
        {
            var authors = await _authorService.GetAllAuthorsAsync(cancellationToken);
            return Ok(_mapper.Map<IReadOnlyList<AuthorDTO>>(authors));
        }

        // GET api/<AuthorController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDTO>> GetAuthor([FromRoute] int id, CancellationToken cancellationToken)
        {
            var author = await _authorService.GetAuthorByIdAsync(id, cancellationToken);
            if(author == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AuthorDTO>(author));
        }

        // POST api/<AuthorController>
        [HttpPost]
        public async Task<ActionResult<AuthorDTO>> CreateAuthor([FromBody] CreateAuthorRequestDTO request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);
            var item = _mapper.Map<AuthorItem>(request);
            var createdAuthor = await _authorService.CreateAuthorAsync(item, cancellationToken);
            var dto = _mapper.Map<AuthorDTO>(createdAuthor);
            return CreatedAtAction(nameof(GetAuthor), new { id = dto.Id }, dto);
        }

        // PUT api/<AuthorController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<AuthorDTO>> UpdateAuthor(int id, [FromBody]UpdateAuthorDTO dto, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            var item = _mapper.Map<AuthorItem>(dto);
            var updatedAuthor = await _authorService.UpdateAuthorAsync(id, item, cancellationToken);
            return Ok(_mapper.Map<AuthorDTO>(updatedAuthor));
        }

        // DELETE api/<AuthorController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuthor([FromRoute]int id, CancellationToken cancellationToken)
        {
            await _authorService.DeleteAuthorAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
