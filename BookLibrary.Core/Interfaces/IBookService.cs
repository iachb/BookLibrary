using BookLibrary.Core.Entities;

namespace BookLibrary.Core.Interfaces
{
    public interface IBookService
    {
        public Task<IReadOnlyList<TBook>> GetAllBooksAsync(CancellationToken cancellationToken);
        public Task<TBook?> GetBookByIdAasync(int id, CancellationToken cancellationToken);
        public Task<TBook> CreateBookAsync(TBook book, CancellationToken cancellationToken = default);
    }
}
