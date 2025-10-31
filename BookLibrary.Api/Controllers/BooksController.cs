using AutoMapper;
using BookLibrary.Api.Models.Books;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        // GET: api/<BooksController>
        [HttpGet]
        public async Task<IReadOnlyList<BookDTO>> GetAllBooks(CancellationToken cancellationToken)
        {
            var books = await _bookService.GetAllBooksAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<BookDTO>>(books);
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook([FromRoute] int id, CancellationToken cancellationToken)
        {
            var book = await _bookService.GetBookByIdAasync(id, cancellationToken);
            if (book == null)
            {
                return NotFound();
            }
            return _mapper.Map<BookDTO>(book);
        }

        // POST api/<BooksController>
        [HttpPost]
        public async Task<ActionResult<BookDTO>> CreateBook([FromBody] CreateBookRequestDTO request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);
            var book = _mapper.Map<TBook>(request);
            var createdBook = await _bookService.CreateBookAsync(book, cancellationToken);
            var dto = _mapper.Map<BookDTO>(createdBook);
            return CreatedAtAction(nameof(GetBook), new { id = dto.Id }, _mapper.Map<BookDTO>(dto));
        }

        // PUT api/<BooksController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<BooksController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
