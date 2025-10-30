using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces.Repository;

namespace BookLibrary.Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IReadOnlyList<TBook>> GetAllBooksAsync (CancellationToken cancellationToken)
        {
            return await _bookRepository.GetAllAsync(cancellationToken);
        }

        public async Task<TBook> CreateBookAsync(TBook book, CancellationToken cancellationToken)
        {
            var existingBook = await _bookRepository.GetBookByTitleAsync(book.Title, cancellationToken);
            if (existingBook != null)
            {
                throw new InvalidOperationException($"A book with the title '{book.Title}' already exists.");
            }
            var createdBook = _bookRepository.AddBookAsync(book, cancellationToken);
            await _bookRepository.SaveChangesAsync(cancellationToken);
            return await createdBook;
        }
    }
}
