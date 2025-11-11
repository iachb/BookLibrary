using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Models.Authors;
using BookLibrary.Core.Models.Books;
using BookLibrary.Infrastructure.Repository;

namespace BookLibrary.Infrastructure.Services
{
    public class AuthorService : IAuthorService
    {
        public readonly IAuthorRepository _authorRepository;
        public readonly IMapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, IMapper mapper)
        {
            this._authorRepository = authorRepository;
            this._mapper = mapper;
        }

        public async Task<IReadOnlyList<AuthorItem>> GetAllAuthors(CancellationToken cancellationToken = default)
        {
            var authors = await _authorRepository.GetAllAuthorsAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<AuthorItem>>(authors);
        }

        public async Task<AuthorItem> CreateAuthorAsync(AuthorItem item, CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            var author = _mapper.Map<TAuthor>(item);
            var existingAuthor = await _authorRepository.GetAuthorByNameAsync(author.Name, cancellationToken);
            if(existingAuthor != null)
            {
                throw new InvalidOperationException($"Author with name '{author.Name}' already exists.");
            }
            var createdAuthor = await _authorRepository.CreateAuthorAsync(author, cancellationToken);
            await _authorRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<AuthorItem>(createdAuthor);
        }
    }
}
