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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BooksController>
        [HttpPost]
        public async Task<StatusCodeResult> CreateBook([FromBody] BookDTO bookDTO)
        {
            var book = _mapper.Map<TBook>(bookDTO);
            var createdBook = await _bookService.CreateBookAsync(book, CancellationToken.None);
            var createdBookDTO = _mapper.Map<BookDTO>(createdBook);
            return StatusCode(201);
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
