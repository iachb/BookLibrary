using AutoMapper;
using BookLibrary.Core.Entities;
using BookLibrary.Core.Interfaces.Repository;
using BookLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        public readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<IReadOnlyList<TAuthor>> GetAllAuthorsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Author
                .Include(a => a.Books)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<TAuthor?> GetAuthorByIdAsync(int id,  CancellationToken cancellationToken = default)
        {
            return await _context.Author
                .Include(a => a.Books)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<TAuthor?> GetAuthorByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_context.Author.FirstOrDefault(a => a.Name == name));
        }

        public async Task<TAuthor> CreateAuthorAsync(TAuthor author, CancellationToken cancellationToken = default)
        {
            await _context.Author.AddAsync(author, cancellationToken);
            return author;
        }

        public async Task DeleteAuthorByIdAsync(int id, CancellationToken cancellationToken)
        {
            await _context.Author.Where(a => a.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
        }


        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
