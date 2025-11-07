using BookLibrary.Core.Models.Books;

namespace BookLibrary.Core.Interfaces
{
    public interface IBookService
    {
        public Task<IReadOnlyList<BookItem>> GetAllBooksAsync(CancellationToken cancellationToken);
        public Task<BookItem?> GetBookByIdAsync(int id, CancellationToken cancellationToken);
        public Task<BookItem> CreateBookAsync(BookItem book, CancellationToken cancellationToken = default);
        public Task<BookItem> UpdateBookAsync(int id, BookItem book, CancellationToken cancellationToken);
        public Task DeleteBookById(int id, CancellationToken cancellationToken);
    }
}
