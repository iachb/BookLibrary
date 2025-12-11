using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Core.Models.Authors;

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

        public async Task<IReadOnlyList<AuthorItem>> GetAllAuthorsAsync(CancellationToken cancellationToken = default)
        {
            var authors = await _authorRepository.GetAllAuthorsAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<AuthorItem>>(authors);
        }

        public async Task<AuthorItem> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id, cancellationToken);
            if(author == null)
            {
                throw new KeyNotFoundException($"Author with id '{id}' not found.");
            }
            return _mapper.Map<AuthorItem>(author);
        }

        public async Task<AuthorItem> CreateAuthorAsync(AuthorItem item, CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            var author = _mapper.Map<TAuthor>(item);
            var normalizedName = item.Name?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ArgumentException("An author name is required.", nameof(item));
            }
            var existingAuthor = await _authorRepository.GetAuthorByNameAsync(normalizedName, cancellationToken);
            if (existingAuthor != null)
            {
                throw new InvalidOperationException($"Author with name '{author.Name}' already exists.");
            }
            var createdAuthor = await _authorRepository.CreateAuthorAsync(author, cancellationToken);
            await _authorRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<AuthorItem>(createdAuthor);
        }

        public async Task<AuthorItem> UpdateAuthorAsync(int id, AuthorItem item, CancellationToken cancellationToken = default)
        {
            if(id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
            }

            if(item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var existingAuthor = await _authorRepository.GetAuthorByIdAsync(id, cancellationToken);

            if(existingAuthor == null)
            {
                throw new KeyNotFoundException($"Author with id '{id}' not found.");
            }

            if(!string.Equals(existingAuthor.Name, item.Name, StringComparison.OrdinalIgnoreCase))
            {
                var otherAuthor = await _authorRepository.GetAuthorByNameAsync(item.Name, cancellationToken);
                if(otherAuthor != null && otherAuthor.Id != id)
                {
                    throw new InvalidOperationException($"Another author with name '{item.Name}' already exists.");
                }
            }
            existingAuthor.Name = item.Name;
            existingAuthor.BirthDate = item.BirthDate;
            // update books tbd
            await _authorRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<AuthorItem>(existingAuthor);
        }

        public async Task DeleteAuthorAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
            }
            var author = await _authorRepository.GetAuthorByIdAsync(id, cancellationToken);
            if(author == null)
            {
                throw new KeyNotFoundException($"Author with id '{id}' not found.");
            }
            await _authorRepository.DeleteAuthorByIdAsync(id, cancellationToken);
            await _authorRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
