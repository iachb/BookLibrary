
using BookLibrary.Core.Models.Authors;

namespace BookLibrary.Core.Interfaces
{
    public interface IAuthorService
    {
        public Task<AuthorItem> CreateAuthorAsync(AuthorItem item, CancellationToken cancellationToken = default);
        public Task<IReadOnlyList<AuthorItem>> GetAllAuthors(CancellationToken cancellationToken = default);
    }
}
