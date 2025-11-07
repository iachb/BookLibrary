using AutoMapper;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Models.Books;

namespace BookLibrary.Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<BookItem>> GetAllBooksAsync (CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetAllBooksAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<BookItem>>(books);
        }

        public async Task<BookItem?> GetBookByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookByIdAsync(id, cancellationToken);
            return _mapper.Map<BookItem>(book);
        }

        public async Task<BookItem> CreateBookAsync(BookItem book, CancellationToken cancellationToken)
        {
            var bookEntity = _mapper.Map<TBook>(book);
            var existingBook = await _bookRepository.GetBookByTitleAsync(bookEntity.Title, cancellationToken);
            if (existingBook != null)
            {
                throw new InvalidOperationException($"A book with the title '{book.Title}' already exists.");
            }
            var createdBook = _bookRepository.AddBookAsync(bookEntity, cancellationToken);
            await _bookRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<BookItem>(createdBook);
        }

        public async Task<BookItem> UpdateBookAsync(int id, BookItem book, CancellationToken cancellationToken)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            var entity = await _bookRepository.GetBookByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new InvalidOperationException($"No book found with ID '{book.Id}'.");
            }

            if (!string.Equals(entity.Title, book.Title, StringComparison.OrdinalIgnoreCase))
            {
                var other = await _bookRepository.GetBookByTitleAsync(book.Title, cancellationToken);
                if (other != null && other.Id != book.Id)
                {
                    throw new InvalidOperationException($"A book with the title '{book.Title}' already exists.");
                }
            }

            entity.Title = book.Title;
            entity.PublishedDate = book.PublishedDate;
            entity.AuthorId = book.AuthorId;

            await _bookRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<BookItem>(entity);
        }

        public async Task DeleteBookById (int id, CancellationToken cancellationToken)
        {
            if(id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "The book ID must be a positive integer.");
            }
            var book = await _bookRepository.GetBookByIdAsync(id, cancellationToken);
            if (book == null)
            {
                throw new InvalidOperationException($"No book found with ID '{id}'.");
            }
            await _bookRepository.DeleteBookById(book, cancellationToken);
            await _bookRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
