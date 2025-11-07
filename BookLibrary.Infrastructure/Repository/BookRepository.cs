using BookLibrary.Core.Entities;
using BookLibrary.Infrastructure.Data;
using BookLibrary.Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TBook>> GetAllBooksAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Books
                .Include(b => b.Author)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<TBook?> GetBookByTitleAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Books.AsNoTracking()
                .FirstOrDefaultAsync(b => b.Title == name, cancellationToken);
        }

        public async Task<TBook?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Books.Include(b => b.Author).AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<TBook> AddBookAsync(TBook book, CancellationToken cancellationToken = default)
        {
            await _context.Books.AddAsync(book, cancellationToken);
            return book;
        }

        public async Task DeleteBookById(TBook book, CancellationToken cancellationToken = default)
        {
            _context.Books.Remove(book);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}