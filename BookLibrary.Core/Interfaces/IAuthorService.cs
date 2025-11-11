
using BookLibrary.Core.Models.Authors;

namespace BookLibrary.Core.Interfaces
{
    public interface IAuthorService
    {
        public Task<AuthorItem> CreateAuthorAsync(AuthorItem item, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<AuthorItem>> GetAllAuthorsAsync(CancellationToken cancellationToken = default);
        public Task<AuthorItem> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<AuthorItem> UpdateAuthorAsync(int id, AuthorItem item, CancellationToken cancellationToken = default);
    }
}
