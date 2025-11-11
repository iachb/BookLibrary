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

        public AuthorController(IMapper mapper, IAuthorService authorService)
        {
            this._mapper = mapper;
            this._authorService = authorService;
        }

        // GET: api/<AuthorController>
        [HttpGet]
        public async Task<IReadOnlyList<AuthorDTO>> Get(CancellationToken cancellationToken)
        {
            var authors = await _authorService.GetAllAuthorsAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<AuthorDTO>>(authors);
        }

        // GET api/<AuthorController>/5
        [HttpGet("{id}")]
        public async Task<AuthorDTO> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            var author = await _authorService.GetAuthorByIdAsync(id, cancellationToken);
            return _mapper.Map<AuthorDTO>(author);
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
        public async Task<ActionResult<AuthorDTO>> Put(int id, [FromBody]UpdateAuthorDTO dto, CancellationToken cancellationToken)
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
        public async Task<ActionResult> Delete([FromRoute]int id, CancellationToken cancellationToken)
        {
            await _authorService.DeleteAuthorAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
