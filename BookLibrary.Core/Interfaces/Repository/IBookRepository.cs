using BookLibrary.Core.Entities;
namespace BookLibrary.Core.Interfaces.Repository
{
    public interface IBookRepository
    {
        public Task<IReadOnlyList<TBook>> GetAllBooksAsync(CancellationToken cancellationToken = default);
        public Task<TBook?> GetBookByTitleAsync(string name, CancellationToken cancellationToken = default);
        public Task<TBook?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<TBook> AddBookAsync(TBook book, CancellationToken cancellationToken = default);
        public Task DeleteBookById(TBook book, CancellationToken cancellationToken = default);
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
