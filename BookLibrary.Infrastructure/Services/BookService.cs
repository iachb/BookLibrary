using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Models.Books;

namespace BookLibrary.Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        public BookService(IBookRepository bookRepository, IMapper mapper, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _authorRepository = authorRepository;
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

        public async Task<BookItem> CreateBookAsync(BookItem book, CancellationToken cancellationToken = default)
        {
            if (book is null)
                throw new ArgumentNullException(nameof(book));

            var normalizedTitle = book.Title?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedTitle))
                throw new ArgumentException("Title is required.", nameof(book.Title));

            if (book.AuthorId <= 0)
                throw new ArgumentException("AuthorId has to be a positive integer.", nameof(book.AuthorId));

            var author = await _authorRepository.GetAuthorByIdAsync(book.AuthorId, cancellationToken);
            if (author is null)
                throw new InvalidOperationException($"Author with Id {book.AuthorId} not found.");

            var existing = await _bookRepository.GetBookByTitleAsync(normalizedTitle!, cancellationToken);
            if (existing != null)
                throw new InvalidOperationException($"A book with the title '{normalizedTitle}' already exists.");

            var entity = _mapper.Map<TBook>(book);
            entity.Title = normalizedTitle!;
            author.Books.Add(entity);

            await _bookRepository.AddBookAsync(entity, cancellationToken);

            await _bookRepository.SaveChangesAsync(cancellationToken);

            await _bookRepository.LoadAuthorAsync(entity, cancellationToken);

            return _mapper.Map<BookItem>(entity);
        }

        public async Task<BookItem> UpdateBookAsync(int id, BookItem book, CancellationToken cancellationToken)
        {
            if (book is null) throw new ArgumentNullException(nameof(book));

            var entity = await _bookRepository.GetBookByIdAsync(id, cancellationToken);
            if (entity is null)
                throw new InvalidOperationException($"No book found with ID '{id}'.");

            if (!string.Equals(entity.Title, book.Title, StringComparison.OrdinalIgnoreCase))
            {
                var other = await _bookRepository.GetBookByTitleAsync(book.Title, cancellationToken);
                if (other != null && other.Id != id)
                    throw new InvalidOperationException($"A book with the title '{book.Title}' already exists.");
            }

            var author = await _authorRepository.GetAuthorByIdAsync(book.AuthorId, cancellationToken);
            if (author is null)
                throw new InvalidOperationException($"Author with Id {book.AuthorId} not found.");

            entity.Title = book.Title;
            entity.PublishedDate = book.PublishedDate;
            entity.AuthorId = book.AuthorId;

            await _bookRepository.SaveChangesAsync(cancellationToken);

            // Reload author navigation for DTO mapping
            await _bookRepository.LoadAuthorAsync(entity, cancellationToken);

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
