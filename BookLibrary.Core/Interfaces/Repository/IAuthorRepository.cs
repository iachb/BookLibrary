using BookLibrary.Core.Entities;
namespace BookLibrary.Core.Interfaces.Repository
{
    public interface IAuthorRepository
    {
        public Task<IReadOnlyList<TAuthor>> GetAllAuthorsAsync(CancellationToken cancellationToken = default);
        public Task<TAuthor?> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<TAuthor?> GetAuthorByNameAsync(string name, CancellationToken cancellationToken = default);
        public Task<TAuthor> CreateAuthorAsync(TAuthor author, CancellationToken cancellationToken = default);
        public Task DeleteAuthorByIdAsync(int id, CancellationToken cancellationToken);
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
